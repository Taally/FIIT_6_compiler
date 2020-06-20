using System;
using SimpleLang.Visitors;
using NUnit.Framework;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprSumZeroTests : ASTTestsBase
    {
        [Test]
        public void SumWithRightZero()
        {
            var AST = BuildAST(@"
var a, b;
a = b + 0;
");
            var expected = @"var a, b;
a = b;";

            var opt = new OptExprSumZero();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void SumWithLeftZero()
        {
            var AST = BuildAST(@"
var a, b;
a = 0 + b;
");
            var expected = @"var a, b;
a = b;";

            var opt = new OptExprSumZero();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void SumWithRightLeftZero()
        {
            var AST = BuildAST(@"
var a, b;
a = 0 + (0 + b + b * a + 0);
");
            var expected = @"var a, b;
a = (b + (b * a));";

            var opt = new OptExprSumZero();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
}
