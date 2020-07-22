using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptAssignEqualityTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
a = a;
{ b = b; }
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "{",
                "}"
            },
            TestName = "RemoveNode")]

        [TestCase(@"
var a;
a = a + 0;
a = a - 0;
a = a * 1;
",
            ExpectedResult = new[]
            {
                "var a;",
                "a = (a + 0);",
                "a = (a - 0);",
                "a = (a * 1);"
            },
            TestName = "WithoutRemoveConstants")]

        public string[] TestOptAssignEquality(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptAssignEquality());
    }
}
