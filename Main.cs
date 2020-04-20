using System;
using System.IO;
using SimpleScanner;
using SimpleParser;
using SimpleLang.Visitors;
using SimpleLang.ThreeAddressCodeOptimizations;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {
            string FileName = @"..\..\a.txt";
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

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

                    var optExpr = new OptExprVisitor();
                    parser.root.Visit(optExpr);
                    var optStat = new OptStatVisitor();
                    parser.root.Visit(optStat);

                    //Console.WriteLine("\n\n");
                    //pp = new PrettyPrintVisitor();
                    //parser.root.Visit(pp);
                    //Console.WriteLine(pp.Text);

                    Console.WriteLine("\n\n");
                    var threeAddrCode = new ThreeAddrGenVisitor();
                    parser.root.Visit(threeAddrCode);
                    foreach (var instruction in threeAddrCode.Instructions)
                        Console.WriteLine(instruction);

                    Console.WriteLine("\n\n");
                    var optInstructions = ConstantFolding.FoldConstants(threeAddrCode.Instructions);
                    foreach (var instruction in optInstructions)
                        Console.WriteLine(instruction);

                    Console.WriteLine("\n\n");
                    foreach (var instruction in DeleteDeadCodeWithDeadVars.Execute(optInstructions))
                        Console.WriteLine(instruction);
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