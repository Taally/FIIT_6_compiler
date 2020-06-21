using System;
using SimpleLang.Visitors;
using SimpleParser;
using SimpleScanner;

namespace SimpleLanguage.Tests.AST
{
    public class ASTTestsBase
    {
        protected Parser BuildAST(string sourceCode)
        {
            SymbolTable.vars.Clear();
            var scanner = new Scanner();
            scanner.SetSource(sourceCode, 0);
            var parser = new Parser(scanner);
            parser.Parse();
            var fillParents = new FillParentsVisitor();
            parser.root.Visit(fillParents);
            return parser;
        }

        protected string[] ApplyOpt(Parser AST, ChangeVisitor opt)
        {
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            return pp.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
