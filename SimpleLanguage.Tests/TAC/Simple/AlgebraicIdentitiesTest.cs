using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class AlgebraicIdentitiesTest : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
var a, b;
b = a - a;
b = a * 0;
b = 0 * a;
b = 1 * a;
b = a * 1;
b = a / 1;
b = a + 0;
b = 0 + a;
b = a - 0;
b = 0 - a;
b = b / b;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities);

            var expected = new List<string>()
            {
                "#t1 = 0",
                "b = #t1",
                "#t2 = 0",
                "b = #t2",
                "#t3 = 0",
                "b = #t3",
                "#t4 = a",
                "b = #t4",
                "#t5 = a",
                "b = #t5",
                "#t6 = a",
                "b = #t6",
                "#t7 = a",
                "b = #t7",
                "#t8 = a",
                "b = #t8",
                "#t9 = a",
                "b = #t9",
                "#t10 = -a",
                "b = #t10",
                "#t11 = 1",
                "b = #t11"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
