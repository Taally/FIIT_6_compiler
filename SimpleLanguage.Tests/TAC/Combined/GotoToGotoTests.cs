using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Combined
{
    using Optimization = Func<IReadOnlyList<Instruction>, (bool wasChanged, IReadOnlyList<Instruction> instructions)>;

    [TestFixture]
    internal class GotoToGotoTests : OptimizationsTestBase
    {
        [TestCase(@"
var a, b;
b = 5;
if (a > b)
	goto 6;
6: a = 4;
",
            ExpectedResult = new string[]
            {
                "b = 5",
                "#t1 = a > b",
                "if #t1 goto 6",
                "goto 6",
                "L1: goto 6",
                "6: a = 4",
            },
            TestName = "GotoIfElseTACGen1")]

        [TestCase(@"
var a, b;
b = 5;
if (a > b)
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
                "goto 2",
                "L3: goto 2",
                "L1: goto 2",
                "2: a = 5",
            },
            TestName = "GototoGotoEmptyNodes")]

        public IEnumerable<string> GotoToGoto(string sourceCode, bool unreachableCodeElimination = false) =>
            ThreeAddressCodeOptimizer.Optimize(
                GenTAC(sourceCode),
                allCodeOptimizations: new List<Optimization>()
                {
                    ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
                    ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
                },
                UnreachableCodeElimination: unreachableCodeElimination)
            .Select(instruction => instruction.ToString());
    }
}
