using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Combined
{
    using Optimization = Func<IReadOnlyList<Instruction>, (bool wasChanged, IReadOnlyList<Instruction> instructions)>;

    [TestFixture]
    internal class FoldPropagateConstantsTests : OptimizationsTestBase
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
                "#t1 = -7",
                "y = -7",
                "#t2 = 28",
                "x = 28"
            },
            TestName = "Test1")]

        [TestCase(@"
var a;
a = 1 + 2 * 3 - 7;
",
            ExpectedResult = new string[]
            {
                "#t1 = 6",
                "#t2 = 7",
                "#t3 = 0",
                "a = 0"
            },
            TestName = "Test2")]

        public IEnumerable<string> FoldPropagateConstants(string sourceCode) =>
            ThreeAddressCodeOptimizer.Optimize(
                GenTAC(sourceCode),
                new List<Optimization>()
                {
                    ThreeAddressCodeFoldConstants.FoldConstants,
                    ThreeAddressCodeConstantPropagation.PropagateConstants
                })
            .Select(instruction => instruction.ToString());
    }
}
