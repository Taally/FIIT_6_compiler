using NUnit.Framework;
using SimpleLang.Visitors;
using System;
using System.Linq;

namespace SimpleLanguage.Tests.AST
{
    public class OptExprEqualBoolNumTests: ASTTestsBase
    {
        [Test]
        public void SumNumTest()
        {
            var AST = BuildAST(@"
var b, c, d;
b = true == true;
while (5 == 5)
  c = true == false;
d = 7 == 8;");
            var expected = new[] {
                "var b, c, d;",
                "b = true;",
                "while true",
                "  c = false;",
                "d = false;"
            };

            var result = ApplyOpt(new OptExprEqualBoolNum());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
