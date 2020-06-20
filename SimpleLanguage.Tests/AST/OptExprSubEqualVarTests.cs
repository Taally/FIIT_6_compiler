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
            var expected = new[] {
                "var a, b;",
                "a = 0;"
            };

            var result = ApplyOpt(new OptExprSubEqualVar());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void SubIDInPrintTest()
        {
            var AST = BuildAST(@"
var a, b;
print(a - a, b - b, b - a, a - a - b);
");

            var expected = new[] {
                "var a, b;",
                "print(0, 0, (b - a), (0 - b));"
            };

            var result = ApplyOpt(new OptExprSubEqualVar());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
