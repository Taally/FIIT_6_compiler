using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    public class OptWhileFalseTests : ASTTestsBase
    {
        [TestCase(@"
var a;
while false
    a = true;
",
            ExpectedResult = new[]
            {
                "var a;"
            },
            TestName = "ShouldCreateNoop")]

        [TestCase(@"
var a;
a = false;
while a
    a = true;
",
            ExpectedResult = new[]
            {
                "var a;",
                "a = false;",
                "while a",
                "    a = true;"
            },
            TestName = "ShouldNotCreateNoop")]

        public string[] TestOptWhileFalse(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptWhileFalseVisitor());
    }
}
