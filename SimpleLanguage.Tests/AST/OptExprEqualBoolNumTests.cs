using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    public class OptExprEqualBoolNumTests : ASTTestsBase
    {
        [TestCase(@"
var b, c, d;
b = true == true;
while (5 == 5)
    c = true == false;
d = 7 == 8;
",
            ExpectedResult = new[]
            {
                "var b, c, d;",
                "b = true;",
                "while true",
                "    c = false;",
                "d = false;"
            },
            TestName = "SumNum")]

        public string[] TestOptExprEqualBoolNum(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprEqualBoolNum());
    }
}
