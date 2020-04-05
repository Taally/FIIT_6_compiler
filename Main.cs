using System;
using System.IO;
using SimpleScanner;
using SimpleParser;
using Newtonsoft.Json;
using SimpleLang.Visitors;

namespace SimpleCompiler
{
    public class SimpleCompilerMain{
        public static void Main(){
            string FileName = @"..\..\a.txt";
            string OutputFileName = @"..\..\a.json";
            try{
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);

                var b = parser.Parse();
                if (!b) Console.WriteLine("Error");
                else{
                    Console.WriteLine("Syntax tree built");
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.All
                    };
                    string output = JsonConvert.SerializeObject(parser.root, jsonSettings);
                    File.WriteAllText(OutputFileName, output);

                    var fillParents = new FillParentsVisitor();
                    parser.root.Visit(fillParents);

                    var pp = new PrettyPrintVisitor();
                    parser.root.Visit(pp);
                    Console.WriteLine(pp.Text);

                    var optExpr = new OptExprVisitor();
                    parser.root.Visit(optExpr);

                    Console.WriteLine("\n\n");
                    pp = new PrettyPrintVisitor();
                    parser.root.Visit(pp);
                    Console.WriteLine(pp.Text);
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