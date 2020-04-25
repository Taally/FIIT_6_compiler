using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang;
using Newtonsoft.Json;

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
                if (!b)
                    Console.WriteLine("Ошибка");
                else
                {
                    Console.WriteLine("Синтаксическое дерево построено");
                    //foreach (var st in parser.root.StList)
                    //Console.WriteLine(st);
                    
                    AssignCountVisitor acv = new AssignCountVisitor();
                    parser.root.Visit(acv);
                    Console.WriteLine(acv.Count);

                    var prettyPrinter = new PrettyPrinterVisitor();
                    parser.root.Visit(prettyPrinter);
                    Console.WriteLine("\n----Pretty Printer----");
                    Console.WriteLine(prettyPrinter.Text);
                    Console.WriteLine("----------------------");

                    var parentVisitor = new FillParentVisitor();
                    parser.root.Visit(parentVisitor);
                    //var multOptimization = new MultOptimizationVisitor();
                    //parser.root.Visit(multOptimization);

                    //var sumOptimization = new SumOptimizationVisitor();
                    //parser.root.Visit(sumOptimization);

                    //parser.root.Visit(multOptimization);

                    prettyPrinter = new PrettyPrinterVisitor();
                    parser.root.Visit(prettyPrinter);
                    Console.WriteLine("\n----Pretty Printer----");
                    Console.WriteLine(prettyPrinter.Text);
                    Console.WriteLine("----------------------");

                    Console.WriteLine("\n----Three address code----");
                    var threeAddressCode = new ThreeAddressCode();
                    parser.root.Visit(threeAddressCode);
                    threeAddressCode.Print();
                    Console.WriteLine("----------------------");

                    Console.WriteLine("\n----Constant convolution----");
                    threeAddressCode.ConsntantConvolution();
                    threeAddressCode.Print();
                    Console.WriteLine("----------------------");

                    Console.WriteLine("\n----CheckA lgebraic Identities----");
                    threeAddressCode.CheckAlgebraicIdentities();
                    threeAddressCode.Print();
                    Console.WriteLine("----------------------");
                }                  
            }            
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine("Лексическая ошибка. " + e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Синтаксическая ошибка. " + e.Message);
            }
            
            Console.ReadLine();
        }

    }
}
