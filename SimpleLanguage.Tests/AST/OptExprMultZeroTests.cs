using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprMultZeroTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
a = b * 0;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 0;"
            },
            TestName = "MultWithRightZero")]

        [TestCase(@"
var a, b;
a = 0 * b;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 0;"
            },
            TestName = "MultWithLeftZero")]

        [TestCase(@"
var a, b;
a = 0 * b + b * a * 0 + 5;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = ((0 + 0) + 5);"
            },
            TestName = "MultWithRightLeftZero")]

        public string[] TestOptExprMultZero(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprMultZero());
    }
}
