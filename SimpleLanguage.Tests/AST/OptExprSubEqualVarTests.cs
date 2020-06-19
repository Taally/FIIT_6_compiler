using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprSubEqualVarTests : ASTTestsBase
    {
        [Test]
        public void SubIDTest()
        {
            var AST = BuildAST(@"
var a, b;
a = b - b;
");
            var expected = @"var a, b;
a = 0;";

            var opt = new OptExprSubEqualVar();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void SubIDInPrintTest()
        {
            var AST = BuildAST(@"
var a, b;
print(a - a, b - b, b - a, a - a - b);
");
            var expected = @"var a, b;
print(0, 0, (b - a), (0 - b));";

            var opt = new OptExprSubEqualVar();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
}
