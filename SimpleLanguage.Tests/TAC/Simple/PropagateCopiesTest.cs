using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class PropagateCopiesTest : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
var a, b, c, d, e, x, y, k;
a = b;
c = b - a;
d = c + 1;
e = d * a;
a = x - y;
k = c + a;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeCopyPropagation.PropagateCopies);

            var expected = new List<string>()
            {
                "a = b",
                "#t1 = b - b",
                "c = #t1",
                "#t2 = c + 1",
                "d = #t2",
                "#t3 = d * b",
                "e = #t3",
                "#t4 = x - y",
                "a = #t4",
                "#t5 = c + a",
                "k = #t5"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
