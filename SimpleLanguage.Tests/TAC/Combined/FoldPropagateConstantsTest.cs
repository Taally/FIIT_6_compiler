using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Combined
{
    using Optimization = Func<List<Instruction>, (bool, List<Instruction>)>;

    [TestFixture]
    internal class FoldPropagateConstantsTest : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
var x, y;
x = 14;
y = 7 - x;
x = x + x;
");
            var optimizations = new List<Optimization>
            {
                ThreeAddressCodeFoldConstants.FoldConstants,
                ThreeAddressCodeConstantPropagation.PropagateConstants,
            };

            var expected = new List<string>()
            {
                "x = 14",
                "#t1 = -7",
                "y = -7",
                "#t2 = 28",
                "x = 28"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test2()
        {
            var TAC = GenTAC(@"
var a;
a = 1 + 2 * 3 - 7;
");
            var optimizations = new List<Optimization>
            {
                ThreeAddressCodeFoldConstants.FoldConstants,
                ThreeAddressCodeConstantPropagation.PropagateConstants,
            };

            var expected = new List<string>()
            {
                "#t1 = 6",
                "#t2 = 7",
                "#t3 = 0",
                "a = 0"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
