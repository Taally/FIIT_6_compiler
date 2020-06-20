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

            var expected = new[] {
                "var a, b;",
                "a = b;"
            };
            
            var result = ApplyOpt(AST, new OptExprMultDivByOne());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void MultByLeftOne()
        {
            var AST = BuildAST(@"
var a, b;
a = 1 * b;
");
            var expected = new[] {
                "var a, b;",
                "a = b;"
            };

            var result = ApplyOpt(AST, new OptExprMultDivByOne());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void DivByRightOne()
        {
            var AST = BuildAST(@"
var a, b;
a = b / 1;
");
            var expected = new[] {
                "var a, b;",
                "a = b;"
            };

            var result = ApplyOpt(AST, new OptExprMultDivByOne());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void MultAndDivByLeftRightOne()
        {
            var AST = BuildAST(@"
var a, b;
a = 1 * a * 1 + (1 * b / 1) * 1 / 1;
");

            var expected = new[] {
                "var a, b;",
                "a = (a + b);"
            };

            var result = ApplyOpt(AST, new OptExprMultDivByOne());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
