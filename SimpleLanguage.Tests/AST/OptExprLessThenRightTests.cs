using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprLeftLessThenRightTests : ASTTestsBase
    {
        [Test]
        public void SimpleTestWithInts1()
        {
            var AST = BuildAST(@"
var c;
c = 4 < 13;
");
            var expected = @"var c;
c = true;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void SimpleTestWithInts2()
        {
            var AST = BuildAST(@"
var c;
c = 15 < 13;
");
            var expected = @"var c;
c = false;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void SimpleTestWithBools1()
        {
            var AST = BuildAST(@"
var c;
c = true < true;
");
            var expected = @"var c;
c = false;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void SimpleTestWithBools2()
        {
            var AST = BuildAST(@"
var c;
c = false < true;
");
            var expected = @"var c;
c = true;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void TestWithIntAndBools1()
        {
            var AST = BuildAST(@"
var b;
b = (3 < 4) < (true < true);
");
            var expected = @"var b;
b = false;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void TestWithIntAndBools2()
        {
            var AST = BuildAST(@"
var b;
b = (5 < 4) < (false < true);
");
            var expected = @"var b;
b = true;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void TestWithIntAndBools3()
        {
            var AST = BuildAST(@"
var a;
a = (((3 < 1) < (5 < 2)) < true);
");
            var expected = @"var a;
a = true;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void TestWithIntAndBools4()
        {
            var AST = BuildAST(@"
var b;
b = (5 < 4) < false;
");
            var expected = @"var b;
b = false;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }        

        [Test]
        public void TestWithIntAndBools5()
        {
            var AST = BuildAST(@"
var c;
c = (((3 < 4) < (5 < 7)) < true) < (5 < 6);
");
            var expected = @"var с;
c = false;";

            var opt = new OptExprLeftLessRight();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
}
