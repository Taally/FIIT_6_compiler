using NUnit.Framework;
using SimpleLang.Visitors;

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
            var expected = new[] {
                "var a, b;",
                "a = b;"
            };

            var result = ApplyOpt(AST, new OptExprSumZero());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void SumWithLeftZero()
        {
            var AST = BuildAST(@"
var a, b;
a = 0 + b;
");
            var expected = new[] {
                "var a, b;",
                "a = b;"
            };

            var result = ApplyOpt(AST, new OptExprSumZero());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void SumWithRightLeftZero()
        {
            var AST = BuildAST(@"
var a, b;
a = 0 + (0 + b + b * a + 0);
");

            var expected = new[] {
                "var a, b;",
                "a = (b + (b * a));"
            };

            var result = ApplyOpt(AST, new OptExprSumZero());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
