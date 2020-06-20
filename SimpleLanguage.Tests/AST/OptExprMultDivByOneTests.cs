using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprMultDivByOneTests : ASTTestsBase
    {
        [Test]
        public void MultByRightOne() {
            var AST = BuildAST(@"
var a, b;
a = b * 1;
");
            var expected = @"var a, b;
a = b;";

            var opt = new OptExprMultDivByOne();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void MultByLeftOne()
        {
            var AST = BuildAST(@"
var a, b;
a = 1 * b;
");
            var expected = @"var a, b;
a = b;";

            var opt = new OptExprMultDivByOne();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void DivByRightOne()
        {
            var AST = BuildAST(@"
var a, b;
a = b / 1;
");
            var expected = @"var a, b;
a = b;";

            var opt = new OptExprMultDivByOne();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void MultAndDivByLeftRightOne()
        {
            var AST = BuildAST(@"
var a, b;
a = 1 * a * 1 + (1 * b / 1) * 1 / 1;
");
            var expected = @"var a, b;
a = (a + b);";

            var opt = new OptExprMultDivByOne();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
}
