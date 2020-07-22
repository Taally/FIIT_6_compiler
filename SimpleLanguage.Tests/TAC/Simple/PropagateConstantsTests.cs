using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class PropagateConstantsTests : OptimizationsTestBase
    {
        [TestCase(@"
var x, y;
x = 14;
y = 7 - x;
x = x + x;
",
            ExpectedResult = new string[]
            {
                "x = 14",
                "#t1 = 7 - 14",
                "y = #t1",
                "#t2 = 14 + 14",
                "x = #t2"
            },
            TestName = "Test1")]

        [TestCase(@"
var x, y, b;
y = 5;
x = b;
y = 7;
x = y + y;
",
            ExpectedResult = new string[]
            {
                "y = 5",
                "x = b",
                "y = 7",
                "#t1 = 7 + 7",
                "x = #t1"
            },
            TestName = "Test2")]

        [TestCase(@"
var x, y, b;
x = 5;
x = x;
",
            ExpectedResult = new string[]
            {
                "x = 5",
                "x = 5",
            },
            TestName = "Test3")]

        public IEnumerable<string> TestPropagateConstants(string sourceCode) =>
            TestTACOptimization(sourceCode, ThreeAddressCodeConstantPropagation.PropagateConstants);
    }
}
