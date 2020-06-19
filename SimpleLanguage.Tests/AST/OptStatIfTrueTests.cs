using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptStatIfTrueTests : ASTTestsBase
    {
        [Test]
        public void IfTrueTest()
        {
            var AST = BuildAST(@"
var a, b;
if true
a = b;
else
a = 1;
");
            var expected = @"var a, b;
a = b;";

            var opt = new OptStatIfTrue();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void IfTrueBlockTest()
        {
            var AST = BuildAST(@"
var a, b;
if true {
a = b;
b = 1;
}
else
a = 1;
");
            var expected = @"var a, b;
a = b;
b = 1;";

            var opt = new OptStatIfTrue();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void IfTrueComplexTest()
        {
            var AST = BuildAST(@"
var a, b;
if true
if true {
a = b;
b = 1;
}
else
a = 1;

if a > b{
a = b;
if true{
b = b + 1;
b = b / 5;
}
}
");
            var expected = @"var a, b;
a = b;
b = 1;
if (a > b) {
  a = b;
  b = (b + 1);
  b = (b / 5);
}";

            var opt = new OptStatIfTrue();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
}
