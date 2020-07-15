using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprSumZeroTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
a = b + 0;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;"
            },
            TestName = "SumWithRightZero")]

        [TestCase(@"
var a, b;
a = 0 + b;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;"
            },
            TestName = "SumWithLeftZero")]

        [TestCase(@"
var a, b;
a = 0 + (0 + b + b * a + 0);
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = (b + (b * a));"
            },
            TestName = "SumWithRightLeftZero")]

        public string[] TestOptExprSumZero(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprSumZero());
    }
}
