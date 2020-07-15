using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprWithOperationsBetweenConstsTests : ASTTestsBase
    {
        [TestCase(@"
var c;
c = 3 < 15;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = true;"
            },
            TestName = "OpLessTrue")]

        [TestCase(@"
var c;
c = 3 < 2;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = false;"
            },
            TestName = "OpLessFalse")]

        [TestCase(@"
var c;
c = 3 > 2;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = true;"
            },
            TestName = "OpGreaterTrue")]

        [TestCase(@"
var c;
c = 3 > 4;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = false;"
            },
            TestName = "OpGreaterFalse")]

        [TestCase(@"
var c;
c = 3 >= 2;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = true;"
            },
            TestName = "OpEQGREATERTrue1")]

        [TestCase(@"
var c;
c = 3 >= 4;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = false;"
            },
            TestName = "OpEQGREATERFalse")]

        [TestCase(@"
var c;
c = 3 >= 3;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = true;"
            },
            TestName = "OpEQGREATERTrue2")]

        [TestCase(@"
var c;
c = 3 <= 4;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = true;"
            },
            TestName = "OpEQLESSTrue1")]

        [TestCase(@"
var c;
c = 3 <= 2;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = false;"
            },
            TestName = "OpEQLESSFalse")]

        [TestCase(@"
var c;
c = 3 <= 3;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = true;"
            },
            TestName = "OpEQLESSTrue2")]

        [TestCase(@"
var c;
c = 3 != 2;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = true;"
            },
            TestName = "OpNOTEQUALTrue1")]

        [TestCase(@"
var c;
c = false != true;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = true;"
            },
            TestName = "OpNOTEQUALTrue2")]

        [TestCase(@"
var c;
c = 3 != 3;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = false;"
            },
            TestName = "OpNOTEQUALFalse1")]

        [TestCase(@"
var c;
c = true != true;
c = false != false;
",
            ExpectedResult = new[]
            {
                "var c;",
                "c = false;",
                "c = false;"
            },
            TestName = "OpNOTEQUALFalse2")]

        public string[] TestOptExprWithOperationsBetweenConsts(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprWithOperationsBetweenConsts());
    }
}
