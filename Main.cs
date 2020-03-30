using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using Newtonsoft.Json;
using SimpleLang.Visitors;

namespace SimpleCompiler{
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
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                    jsonSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonSettings.TypeNameHandling = TypeNameHandling.All;
                    string output = JsonConvert.SerializeObject(parser.root, jsonSettings);
                    File.WriteAllText(OutputFileName, output);

                    Console.WriteLine("Before opt");
                    var pp = new PrettyPrintVisitor();
                    parser.root.Visit(pp);
                    Console.WriteLine(pp.Text);

                    var fp = new FillParentVisitor();
                    parser.root.Visit(fp);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("After opt");

                    var opt = new OptVisitor();
                    parser.root.Visit(opt);

                    var pp1 = new PrettyPrintVisitor();
                    parser.root.Visit(pp1);
                    Console.WriteLine(pp1.Text);
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