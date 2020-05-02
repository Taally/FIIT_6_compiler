using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class GotoToGotoTest : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
var a, b;
1: goto 2;
2: goto 5;
3: goto 6;
4: a = 1;
5: goto 6;
6: a = b;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto);

            var expected = new List<string>()
            {
                "1: goto 6",
                "2: goto 6",
                "3: goto 6",
                "4: a = 1",
                "5: goto 6",
                "6: a = b",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
