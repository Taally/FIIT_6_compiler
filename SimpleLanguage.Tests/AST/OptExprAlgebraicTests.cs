using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprAlgebraicTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
a = 1 + 41;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 42;"
            },
            TestName = "SumNum")]

        [TestCase(@"
var a, b;
a = 6 * 7;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 42;"
            },
            TestName = "MultNum")]

        [TestCase(@"
var a, b;
a = 55 - 13;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 42;"
            },
            TestName = "SubNum")]

        [TestCase(@"
var a, b;
a = 546 / 13;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 42;"
            },
            TestName = "DivNum")]

        [TestCase(@"
var a, b;
a = 546 > 13;
b = 546 < 13;
a = 546 != 13;
b = 42 == 13;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = (546 > 13);",
                "b = (546 < 13);",
                "a = (546 != 13);",
                "b = (42 == 13);"
            },
            TestName = "NotFold")]

        [TestCase(@"
var a, b;
a = 42 / 6 * 3 - 3 * (1 + 1) - 2;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 13;"
            },
            TestName = "FoldComplex")]

        [TestCase(@"
var a, b;
print(4 + 5, 2 - 1, 6 / 3, 2 * 5);
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "print(9, 1, 2, 10);"
            },
            TestName = "FoldInPrint")]

        public string[] TestOptExprAlgebraic(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprAlgebraic());
    }
}
