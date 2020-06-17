using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Combined
{
    using Optimization = Func<List<Instruction>, (bool, List<Instruction>)>;

    [TestFixture]
    internal class GotoToGotoTest : TACTestsBase
    {
        [Test]
        public void TestGotoIfElseTACGen1()
        {
            var TAC = GenTAC(@"
var a,b;
b = 5;
if(a > b)
	goto 6;
6: a = 4;
");
            var optimizations = new List<Optimization>
            {
                ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
                ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
            };

            var expected = new List<string>()
            {
                "b = 5",
                "#t1 = a > b",
                "if #t1 goto 6",
                "goto 6",
                "L1: goto 6",
                "6: a = 4",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGotoIfElseTACGen2()
        {
            var TAC = GenTAC(@"
var a,b;
b = 5;
if(a > b)
	goto 6;
else
    goto 4;
6: a = 4;
4: a = 6;

");
            var optimizations = new List<Optimization>
            {
                ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
                ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
            };

            var expected = new List<string>()
            {
                "b = 5",
                "#t1 = a > b",
                "if #t1 goto 6",
                "goto 4",
                "goto 6",
                "L1: goto 6",
                "6: a = 4",
                "4: a = 6",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGototoGotoEmptyNodes()
        {
            var TAC = GenTAC(@"
var a, b;
goto 1;
1: if (true) 
    goto 2;
2: a = 5;
");
            var optimizations = new List<Optimization>
            {
                ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
                ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
            };

            var expected = new List<string>()
            {
                "goto 2",
                "1: if True goto 2",
                "goto 2",
                "L1: goto 2",
                "2: a = 5",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
