using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class PropagateConstantsTest : TACTestsBase
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
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);

            var expected = new List<string>()
            {
                "x = 14",
                "#t1 = 7 - 14",
                "y = #t1",
                "#t2 = 14 + 14",
                "x = #t2"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}