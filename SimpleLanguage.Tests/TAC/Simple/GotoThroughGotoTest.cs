using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class GotoThroughGotoTest : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
var a;
1: if (1 < 2) goto 3;
2: goto 4;
3: a = 0;
4: a = 1;
666: a = false;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto);

            var expected = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "#t2 = !#t1",
                "if #t2 goto 4",
                "3: a = 0",
                "4: a = 1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
