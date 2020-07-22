using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    public class CommonExprEliminationTests : OptimizationsTestBase
    {
        [TestCase(@"
var a, b, c, g, k;
a = b + c;
c = a + g;
k = b + c;
",
            ExpectedResult = new string[]
            {
                "#t1 = b + c",
                "a = #t1",
                "#t2 = a + g",
                "c = #t2",
                "#t3 = b + c",
                "k = #t3"
            },
            TestName = "NoCommonExpressions")]

        [TestCase(@"
var a, b, c, k;
a = b + c;
k = b + c;
",
            ExpectedResult = new string[]
            {
                "#t1 = b + c",
                "a = #t1",
                "#t2 = #t1",
                "k = #t2"
            },
            TestName = "SimplestCase")]

        [TestCase(@"
var a, b, c, k;
a = b + c;
k = c + b;
",
            ExpectedResult = new string[]
            {
                "#t1 = b + c",
                "a = #t1",
                "#t2 = #t1",
                "k = #t2"
            },
            TestName = "CommutativeOperation")]

        [TestCase(@"
var a, b, c, k;
a = b - c;
k = c - b;
",
            ExpectedResult = new string[]
            {
                "#t1 = b - c",
                "a = #t1",
                "#t2 = c - b",
                "k = #t2"
            },
            TestName = "NotCommutativeOperation")]

        [TestCase(@"
var a, b, c, k;
a = b * c;
b = b * c;
k = b * c;
",
            ExpectedResult = new string[]
            {
                "#t1 = b * c",
                "a = #t1",
                "#t2 = #t1",
                "b = #t2",
                "#t3 = b * c",
                "k = #t3"
            },
            TestName = "UsingItselfInExpression")]

        [TestCase(@"
var a, b, c, k;
a = -b;
k = -b;
",
            ExpectedResult = new string[]
            {
                "#t1 = -b",
                "a = #t1",
                "#t2 = #t1",
                "k = #t2"
            },
            TestName = "UnaryOperation")]

        public IEnumerable<string> TestCommonExprElimination(string sourceCode) =>
            TestTACOptimization(sourceCode, ThreeAddressCodeCommonExprElimination.CommonExprElimination);
    }
}
