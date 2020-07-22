using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprSubEqualVarTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
a = b - b;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 0;"
            },
            TestName = "SubID")]

        [TestCase(@"
var a, b;
print(a - a, b - b, b - a, a - a - b);
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "print(0, 0, (b - a), (0 - b));"
            },
            TestName = "SubIDInPrint")]

        public string[] TestOptExprSubEqualVar(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprSubEqualVar());
    }
}
