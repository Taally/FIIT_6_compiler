using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;

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
		            Console.WriteLine("Error");
                else Console.WriteLine("Abstract syntax tree was successfully constructed.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File {0} not found", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine("Lex error. " + e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Syntax error. " + e.Message);
            }

            Console.ReadLine();
        }

    }
}
