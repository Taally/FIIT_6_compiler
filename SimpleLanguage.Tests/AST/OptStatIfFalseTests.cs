using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptStatIfFalseTests : ASTTestsBase
    {
        [TestCase(@"
var a, b;
if false
a = b;
else
a = 1;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 1;"
            },
            TestName = "IfFalse")]

        [TestCase(@"
var a, b;
if false {
    a = b;
    b = 1;
}
else
    a = 1;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 1;"
            },
            TestName = "IfFalseBlock")]

        [TestCase(@"
var a, b;
if false
a =1;
else
if false {
    a = b;
    b = 1;
}
else
    a = 1;

if a > b{
    a = b;
    if false{
        b = b + 1;
        b = b / 5;
    }
}
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = 1;",
                "if (a > b) {",
                "    a = b;",
                "}"
            },
            TestName = "IfFalseComplex")]

        public string[] TestOptStatIfFalse(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptStatIfFalse());
    }
}
