using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class FoldConstantsTests : OptimizationsTestBase
    {
        [TestCase(@"
var a;
a = 1 - 20;
a = 4 * 2;
a = 10 / 5;
a = 9 + 3;
",
            ExpectedResult = new string[]
            {
                "#t1 = -19",
                "a = #t1",
                "#t2 = 8",
                "a = #t2",
                "#t3 = 2",
                "a = #t3",
                "#t4 = 12",
                "a = #t4"
            },
            TestName = "Test")]

        public IEnumerable<string> TestFoldConstants(string sourceCode) =>
            TestTACOptimization(sourceCode, ThreeAddressCodeFoldConstants.FoldConstants);
    }
}
