using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class GotoToGotoTests : OptimizationsTestBase
    {
        [TestCase(@"
var a, b;
1: goto 2;
2: goto 5;
3: goto 6;
4: a = 1;
5: goto 6;
6: a = b;
",
            true,
            ExpectedResult = new string[]
            {
                "1: goto 6",
                "2: goto 6",
                "5: goto 6",
                "6: a = b",
            },
            TestName = "MultiGoTo")]

        [TestCase(@"
var a, b;
b = 5;
if(a > b)
    goto 6;
6: a = 4;
",
            ExpectedResult = new string[]
            {
                "b = 5",
                "#t1 = a > b",
                "if #t1 goto 6",
                "goto L2",
                "L1: goto 6",
                "L2: noop",
                "6: a = 4",
            },
            TestName = "GotoIfElseTACGen1")]

        [TestCase(@"
var a;
goto 1;
1: goto 2;
2: goto 3;
3: goto 4;
4: a = 4;
",
            ExpectedResult = new string[]
            {
                "goto 4",
                "1: goto 4",
                "2: goto 4",
                "3: goto 4",
                "4: a = 4",
            },
            TestName = "GoToLabel")]

        [TestCase(@"
var a, b;
b = 5;
if(a > b)
    goto 6;
else
    goto 4;
6: a = 4;
4: a = 6;
",
            true,
            ExpectedResult = new string[]
            {
                "b = 5",
                "#t1 = a > b",
                "if #t1 goto 6",
                "goto 4",
                "L1: goto 6",
                "6: a = 4",
                "4: a = 6",
            },
            TestName = "GotoIfElseTACGen2")]

        [TestCase(@"
var a, b;
goto 1;
a = 1;
1: if (true)
    goto 2;
2: a = 5;
",
            true,
            ExpectedResult = new string[]
            {
                "if True goto 2",
                "goto L3",
                "L3: noop",
                "goto L2",
                "L1: goto 2",
                "L2: noop",
                "2: a = 5",
            },
            TestName = "OptimizationLabelIf")]

        [TestCase(@"
1: goto 2;
2: goto 3;
3: goto 1;
",
            ExpectedResult = new string[]
            {
                "1: goto 1",
                "2: goto 1",
                "3: goto 1",
            },
            TestName = "InfiniteLoop")]

        [TestCase(@"
var a, b;
goto 1;
a = 1;
1: if (true)
    goto 4;
else
3: a=5;
4: b = 2;
",
            true,
            ExpectedResult = new string[]
            {
                "if True goto 4",
                "goto 3",
                "noop",
                "3: a = 5",
                "goto L2",
                "L1: goto 4",
                "L2: noop",
                "4: b = 2",
            },
            TestName = "Task3")]

        public IEnumerable<string> TestGotoToGoto(
            string sourceCode,
            bool unreachableCodeElimination = false) =>
            TestTACOptimization(
                sourceCode,
                allCodeOptimization: ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
                unreachableCodeElimination: unreachableCodeElimination);
    }
}
