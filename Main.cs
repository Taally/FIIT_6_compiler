using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang.Visitors;
using SimpleLang;
using SimpleLang.ThreeAddrOpt;

namespace SimpleCompiler{
    public class SimpleCompilerMain{
        public static void Main(){
            string FileName = @"..\..\a.txt";
            try{
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);

                var b = parser.Parse();
                if (!b) Console.WriteLine("Error");
                else{
                    var fp = new FillParentVisitor();
                    parser.root.Visit(fp);
                    
                    var opt1 = new OptStat1Visitor();
                    parser.root.Visit(opt1);

                    var opt2 = new OptExpr1Visitor();
                    parser.root.Visit(opt2);

                    var pp1 = new PrettyPrintVisitor();
                    parser.root.Visit(pp1);
                    Console.WriteLine(pp1.Text);

                    var threeAddr = new ThreeAddrGen();
                    parser.root.Visit(threeAddr);

                    Console.WriteLine("------");

                    threeAddr.table.Insert(5, new Command("#t0001", "a", "b", "+", ""));

                    foreach (var c in threeAddr.table)
                        Console.WriteLine(c.ToString());

                    Console.WriteLine("---After opt---");

                            
                    var check = DefUseOpt.DeleteDeadCode(threeAddr.table);
                    //var check = ThreeAdrOpt.ConvСonstants(threeAddr.table);
                    foreach (var c in check)
                        Console.WriteLine(c.ToString());


                }
            }
            catch (FileNotFoundException){
                Console.WriteLine("File {0} not found", FileName);
            }
            catch (LexException e){
                Console.WriteLine("Lex Error. " + e.Message);
            }
            catch (SyntaxException e){
                Console.WriteLine("Syntax Error. " + e.Message);
            }
            Console.ReadLine();
        }
    }
}