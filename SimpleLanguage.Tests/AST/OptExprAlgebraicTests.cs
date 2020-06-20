﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprAlgebraicTests : ASTTestsBase
    {
        [Test]
        public void SumNumTest()
        {
            var AST = BuildAST(@"
var a, b;
a = 1 + 41;
");
            var expected = new[] {
                "var a, b;",
                "a = 42;"
            };

            var result = ApplyOpt(AST, new OptExprAlgebraic());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void MultNumTest()
        {
            var AST = BuildAST(@"
var a, b;
a = 6 * 7;
");
            var expected = new[] {
                "var a, b;",
                "a = 42;"
            };

            var result = ApplyOpt(AST, new OptExprAlgebraic());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void SubNumTest()
        {
            var AST = BuildAST(@"
var a, b;
a = 55 - 13;
");
            var expected = new[] {
                "var a, b;",
                "a = 42;"
            };

            var result = ApplyOpt(AST, new OptExprAlgebraic());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void DivNumTest()
        {
            var AST = BuildAST(@"
var a, b;
a = 546 / 13;
");
            var expected = new[] {
                "var a, b;",
                "a = 42;"
            };

            var result = ApplyOpt(AST, new OptExprAlgebraic());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void NotFoldTest()
        {
            var AST = BuildAST(@"
var a, b;
a = 546 > 13;
b = 546 < 13;
a = 546 != 13;
b = 42 == 13;
");

            var expected = new[] {
                "var a, b;",
                "a = (546 > 13);",
                "b = (546 < 13);",
                "a = (546 != 13);",
                "b = (42 == 13);"
            };

            var result = ApplyOpt(AST, new OptExprAlgebraic());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void FoldComplexTest()
        {
            var AST = BuildAST(@"
var a, b;
a = 42 / 6 * 3 - 3 * (1 + 1) - 2;
");
            var expected = new[] {
                "var a, b;",
                "a = 13;"
            };

            var result = ApplyOpt(AST, new OptExprAlgebraic());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void FoldInPrintTest()
        {
            var AST = BuildAST(@"
var a, b;
print(4 + 5, 2 - 1, 6 / 3, 2 * 5);
");

            var expected = new[] {
                "var a, b;",
                "print(9, 1, 2, 10);"
            };

            var result = ApplyOpt(AST, new OptExprAlgebraic());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
