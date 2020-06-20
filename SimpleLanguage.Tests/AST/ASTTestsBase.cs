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
        private Parser AST;

        protected Parser BuildAST(string sourceCode) {
            AST = null;
            SymbolTable.vars.Clear();
            var scanner = new Scanner();
            scanner.SetSource(sourceCode, 0);
            var parser = new Parser(scanner);
            parser.Parse();
            var fillParents = new FillParentsVisitor();
            parser.root.Visit(fillParents);
            AST = parser;
            return parser;
        }

        protected string[] ApplyOpt(Parser AST, ChangeVisitor opt) {
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            return pp.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
