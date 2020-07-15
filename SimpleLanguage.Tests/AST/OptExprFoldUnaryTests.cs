using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprFoldUnaryTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
b = !a == !a;
b = !a != !a;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "b = true;",
                "b = false;"
            },
            TestName = "EqualID")]

        [TestCase(@"
var a, b;
b = !a == a;
b = !a != a;
b = a == !a;
b = a != !a;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "b = false;",
                "b = true;",
                "b = false;",
                "b = true;"
            },
            TestName = "LeftRightUnary")]

        public string[] TestOptExprFoldUnary(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprFoldUnary());
    }
}
