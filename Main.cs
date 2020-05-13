using System;
using System.IO;
using SimpleScanner;
using SimpleParser;
using SimpleLang.Visitors;
using SimpleLang;
using NUnit.Framework;
using System.Collections.Generic;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {
            string FileName = @"../../a.txt";
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                /*Console.WriteLine(" \nDefUseSet");
                var livev = new LiveVariableAnalysis();
                livev.FillDefUse();
                Console.WriteLine(livev.ToString());
                */


                Parser parser = new Parser(scanner);

                var b = parser.Parse();
                if (!b) Console.WriteLine("Error");
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
                        Console.WriteLine(instruction);


                    Console.WriteLine("\n\n Local optimized three address code for blocks");

                    var bBlocks = ThreeAddressCodeOptimizer.Optimize(threeAddressCode);

                    foreach (var x in bBlocks)
                    {
                        Console.WriteLine("---------------------------");
                        for (int i = 0; i < x.GetInstructions().Count; i++)
                        {
                            Console.WriteLine(x.GetInstructions()[i]);
                        }
                    }


                    // var optResult = ThreeAddressCodeOptimizer.OptimizeBlocks(bBlocks);
                    // Console.WriteLine("\n\nOptimized three address code");
                    /*foreach (var instruction in optResult)
                        Console.WriteLine(instruction);*/

                    // foreach (var x in optResult)
                    // {
                    //    Console.WriteLine(x.Key + "-------------");
                    //    foreach (var y in x.Value)
                    //    {
                    //         Console.WriteLine(y);
                    //    }
                    // }

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