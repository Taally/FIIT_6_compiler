using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    using Optimization = Func<List<Instruction>, (bool, List<Instruction>)>;

    [TestFixture]
    internal class FoldConstantsTest : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
var a;
a = 1 - 20;
a = 4 * 2;
a = 10 / 5;
a = 9 + 3;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeFoldConstants.FoldConstants };

            var expected = new List<string>()
            {
                "#t1 = -19",
                "a = #t1",
                "#t2 = 8",
                "a = #t2",
                "#t3 = 2",
                "a = #t3",
                "#t4 = 12",
                "a = #t4"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
