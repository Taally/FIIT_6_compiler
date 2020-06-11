using System;
using System.Collections.Generic;

namespace SimpleParser
{
    public class LexException : Exception
    {
        public LexException(string msg) : base(msg) { }
    }
    public class SyntaxException : Exception
    {
        public SyntaxException(string msg) : base(msg) { }
    }
    // Класс глобальных описаний и статических методов
    // для использования различными подсистемами парсера и сканера
    public static class ParserHelper
    {
    }
    public enum type { tint, tbool, tundefined };

    public static class SymbolTable { 
        public static Dictionary<string, type> vars = new Dictionary<string, type>();
        public static void NewVarDef(string name){
            if (vars.ContainsKey(name))
                throw new Exception("Variable " + name + " was described");
            else vars.Add(name,type.tundefined);
        }
    }
}