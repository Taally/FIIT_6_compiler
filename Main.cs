using System;
using System.IO;
using System.Linq;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;
using SimpleScanner;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {
            var FileName = @"../../a.txt";
            try
            {
                var Text = File.ReadAllText(FileName);

                var scanner = new Scanner();
                scanner.SetSource(Text, 0);

                var parser = new Parser(scanner);

                var b = parser.Parse();
                if (!b)
                {
                    Console.WriteLine("Error");
                }
                else
                {
                    Console.WriteLine("Syntax tree built");

                    var fillParents = new FillParentsVisitor();
                    parser.root.Visit(fillParents);

                    var pp = new PrettyPrintVisitor();
                    parser.root.Visit(pp);
                    Console.WriteLine(pp.Text);

                    ASTOptimizer.Optimize(parser);
                    Console.WriteLine("\n\nAfter AST optimizations");
                    pp = new PrettyPrintVisitor();
                    parser.root.Visit(pp);
                    Console.WriteLine(pp.Text);

                    Console.WriteLine("\n\nThree address code");
                    var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
                    parser.root.Visit(threeAddrCodeVisitor);
                    var threeAddressCode = threeAddrCodeVisitor.Instructions;
                    foreach (var instruction in threeAddressCode)
                    {
                        Console.WriteLine(instruction);
                    }

                    Console.WriteLine("\n\nOptimized three address code");
                    var optResult = ThreeAddressCodeOptimizer.OptimizeAll(threeAddressCode);
                    foreach (var x in optResult)
                    {
                        Console.WriteLine(x);
                    }

                    Console.WriteLine("\n\nDivided three address code");
                    var divResult = BasicBlockLeader.DivideLeaderToLeader(optResult);


                    foreach (var x in divResult)
                    {
                        foreach (var y in x.GetInstructions())
                        {
                            Console.WriteLine(y);
                        }
                        Console.WriteLine("--------------");
                    }

                    var cfg = new ControlFlowGraph(divResult);

                    foreach (var block in cfg.GetCurrentBasicBlocks())
                    {
                        Console.WriteLine($"{cfg.VertexOf(block)}  {block.GetInstructions()[0]}");
                        var children = cfg.GetChildrenBasicBlocks(cfg.VertexOf(block));
                        var childrenStr = string.Join(" | ", children.Select(v => v.block.GetInstructions()[0].ToString()));
                        Console.WriteLine($" children: {childrenStr}");

                        var parents = cfg.GetParentsBasicBlocks(cfg.VertexOf(block));
                        var parentsStr = string.Join(" | ", parents.Select(v => v.block.GetInstructions()[0].ToString()));
                        Console.WriteLine($" parents: {parentsStr}");
                    }

                    ///
                    /// LiveVariableAnalysisAlgorithm
                    ///
                    Console.WriteLine("------------");
                    Console.WriteLine();
                    var activeVariable = new LiveVariableAnalysis();
                    var resActiveVariable = activeVariable.Execute(cfg);

                    foreach (var x in resActiveVariable)
                    {
                        foreach (var y in x.Value.In)
                        {
                            Console.WriteLine("In " + y);
                        }
                        Console.WriteLine();
                        foreach (var y in x.Value.Out)
                        {
                            Console.WriteLine("Out " + y);
                        }
                    }


                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("NatLoop");
                    var natLoops = NaturalLoop.GetAllNaturalLoops(cfg);
                    foreach (var x in natLoops)
                    {
                        if (x.Count == 0)
                        {
                            continue;
                        }
                        //Console.WriteLine("Loop");
                        for (var i = 0; i < x.Count; i++)
                        {
                            Console.WriteLine("NumberBlock:" + i);
                            foreach (var xfrom in x[i].GetInstructions())
                            {
                                Console.WriteLine(xfrom.ToString());
                            }
                        }
                        Console.WriteLine();
                        Console.WriteLine("-------------");
                    }

                    Console.WriteLine(" \nDone");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File {0} not found", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine("Lex Error. " + e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Syntax Error. " + e.Message);
            }
            Console.ReadLine();
        }
    }
}
