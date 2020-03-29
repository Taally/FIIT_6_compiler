using System;
using System.Collections.Generic;

namespace SimpleParser
{
    public enum type { tint, tbool };

    public static class SymbolTable // Таблица символов
    {
        public static Dictionary<string, type> vars = new Dictionary<string, type>(); // таблица символов
        public static void NewVarDef(string name, type t)
        {
            if (vars.ContainsKey(name))
                throw new Exception("Variable " + name + " is already defined");
            else vars.Add(name, t);
        }
    }

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
}