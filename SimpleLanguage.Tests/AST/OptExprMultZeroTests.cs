using System;
using System.Collections.Generic;
using SimpleLang.Visitors;
using NUnit.Framework;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprMultZeroTests : ASTTestsBase
    {
        [Test]
        public void MultWithRightZero()
        {
            var AST = BuildAST(@"
var a, b;
a = b * 0;
");
            var expected = new[] {
                "var a, b;",
                "a = 0;"
            };

            var result = ApplyOpt(AST, new OptExprMultZero());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void MultWithLeftZero()
        {
            var AST = BuildAST(@"
var a, b;
a = 0 * b;
");
            var expected = new[] {
                "var a, b;",
                "a = 0;"
            };

            var result = ApplyOpt(AST, new OptExprMultZero());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void MultWithRightLeftZero()
        {
            var AST = BuildAST(@"
var a, b;
a = 0 * b + b * a * 0 + 5;
");

            var expected = new[] {
                "var a, b;",
                "a = ((0 + 0) + 5);"
            };

            var result = ApplyOpt(AST, new OptExprMultZero());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
