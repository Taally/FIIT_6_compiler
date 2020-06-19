using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleParser;
using SimpleScanner;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    public class ASTTestsBase
    {
        protected Parser BuildAST(string sourceCode) {
            SymbolTable.vars.Clear();
            var scanner = new Scanner();
            scanner.SetSource(sourceCode, 0);
            var parser = new Parser(scanner);
            parser.Parse();
            var fillParents = new FillParentsVisitor();
            parser.root.Visit(fillParents);
            return parser;
        }
    }
}
