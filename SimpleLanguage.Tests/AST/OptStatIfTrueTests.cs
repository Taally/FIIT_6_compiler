using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptStatIfTrueTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
if true
a = b;
else
a = 1;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;"
            },
            TestName = "IfTrue")]

        [TestCase(@"
var a, b;
if true {
    a = b;
    b = 1;
}
else
    a = 1;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;",
                "b = 1;"
            },
            TestName = "IfTrueBlock")]

        [TestCase(@"
var a, b;
if true
if true {
    a = b;
    b = 1;
}
else
    a = 1;

if a > b{
    a = b;
    if true{
        b = b + 1;
        b = b / 5;
    }
}
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;",
                "b = 1;",
                "if (a > b) {",
                "    a = b;",
                "    b = (b + 1);",
                "    b = (b / 5);",
                "}"
            },
            TestName = "IfTrueComplex")]

        public string[] TestOptStatIfTrue(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptStatIfTrue());
    }
}
