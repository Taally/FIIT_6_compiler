using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprMultDivByOneTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
a = b * 1;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;"
            },
            TestName = "MultByRightOne")]

        [TestCase(@"
var a, b;
a = 1 * b;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;"
            },
            TestName = "MultByLeftOne")]

        [TestCase(@"
var a, b;
a = b / 1;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;"
            },
            TestName = "DivByRightOne")]

        [TestCase(@"
var a, b;
a = 1 * a * 1 + (1 * b / 1) * 1 / 1;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = (a + b);"
            },
            TestName = "MultAndDivByLeftRightOne")]

        [TestCase(@"
var a;
a = 1 / a;
",
            ExpectedResult = new[]
            {
                "var a;",
                "a = (1 / a);"
            },
            TestName = "OneDivBySomething")]

        public string[] TestOptExprMultDivByOne(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprMultDivByOne());
    }
}
