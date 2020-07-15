using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprSimilarNotEqualTests : ASTTestsBase
    {
        [TestCase(@"
var a, b, d, k, c;
c = a>a;
b = k<k;
d = a != a;
d = 1 > 1;
",
            ExpectedResult = new[]
            {
                "var a, b, d, k, c;",
                "c = false;",
                "b = false;",
                "d = false;",
                "d = false;",
            },
            TestName = "SimilarNotEqual")]

        public string[] TestOptExprSimilarNotEqual(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprSimilarNotEqual());
    }
}
