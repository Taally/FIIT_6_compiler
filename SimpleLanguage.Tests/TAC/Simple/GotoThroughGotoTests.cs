using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class GotoThroughGotoTests : OptimizationsTestBase
    {
        [TestCase(@"
var a;
if (1 < 2)
    goto 3;
2: goto 4;
3: a = 0;
4: a = 1;
666: a = false;
",
            ExpectedResult = new string[]
            {
                "#t1 = 1 >= 2",
                "#t2 = !#t1",
                "if #t2 goto 3",
                "2: goto 4",
                "3: a = 0",
                "4: a = 1",
                "666: a = False"
            },
            TestName = "Optimization")]

        [TestCase(@"
var a;
if (1 < 2)
    goto 3;
3: a = 3;
",
            ExpectedResult = new string[]
            {
                "#t1 = 1 >= 2",
                "if #t1 goto 3",
                "goto 3",
                "3: a = 3",
            },
            TestName = "NoOptimizationShortIf")]

        [TestCase(@"
var a;
1: if (1 < 2)
    a = 4 + 5 * 6;
else
{
    goto 4;
    a = 5;
}
4: a = 4;
",
            ExpectedResult = new string[]
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto 4",
                "a = 5",
                "goto 4",
                "L1: #t2 = 5 * 6",
                "#t3 = 4 + #t2",
                "a = #t3",
                "4: a = 4"
            },
            TestName = "NoOptimizationFullIf")]

        public IEnumerable<string> TestGotoThroughGoto(string sourceCode) =>
            TestTACOptimization(sourceCode, allCodeOptimization: ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto);
    }
}
