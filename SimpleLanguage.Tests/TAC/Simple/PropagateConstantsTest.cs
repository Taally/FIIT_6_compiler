using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    using Optimization = Func<List<Instruction>, (bool, List<Instruction>)>;

    [TestFixture]
    internal class PropagateConstantsTest : TACTestsBase
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
            var optimizations = new List<Optimization> { ThreeAddressCodeConstantPropagation.PropagateConstants };

            var expected = new List<string>()
            {
                "x = 14",
                "#t1 = 7 - 14",
                "y = #t1",
                "#t2 = 14 + 14",
                "x = #t2"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test2()
        {
            var TAC = GenTAC(@"
                var x, y, b;
                y = 5;
                x = b;
                y = 7;
                x = y + y;
                ");
            var optimizations = new List<Optimization> { ThreeAddressCodeConstantPropagation.PropagateConstants };

            var expected = new List<string>()
            {
                "y = 5",
                "x = b",
                "y = 7",
                "#t1 = 7 + 7",
                "x = #t1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test3()
        {
            var TAC = GenTAC(@"
                var x, y, b;
                x = 5;
                x = x;
                ");
            var optimizations = new List<Optimization> { ThreeAddressCodeConstantPropagation.PropagateConstants };

            var expected = new List<string>()
            {
                "x = 5",
                "x = 5",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
