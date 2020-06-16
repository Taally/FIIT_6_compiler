using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    using Optimization = Func<List<Instruction>, (bool, List<Instruction>)>;

    [TestFixture]
    internal class GotoThroughGotoTest : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            // instructions changed
            var TAC = GenTAC(@"
var a;
1: if (1 < 2) goto 3;
2: goto 4;
3: a = 0;
4: a = 1;
666: a = false;
");
            var expectedTAC = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto L2",
                "L1: goto 3",
                "L2: noop",
                "2: goto 4",
                "3: a = 0",
                "4: a = 1",
                "666: a = False"
            };
            var expectedOptimize = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "#t2 = !#t1",
                "if #t2 goto L2",
                "goto 3",
                "L2: noop",
                "2: goto 4",
                "3: a = 0",
                "4: a = 1",
                "666: a = False"
            };

            CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var optimizations = new List<Optimization> { ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto };

            CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expectedOptimize, actual);
        }

        [Test]
        public void Test2()
        {
            // instructions aren't changed
            var TAC = GenTAC(@"
1:  if (1 < 2) 
        goto 3;
    else
        goto 5;
");
            var expectedTAC = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto 5",
                "goto L2",
                "L1: goto 3",
                "L2: noop"
            };
            var expectedOptimize = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto 5",
                "goto L2",
                "L1: goto 3",
                "L2: noop"
            };

            CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var optimizations = new List<Optimization> { ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto };

            CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expectedOptimize, actual);
        }

        [Test]
        public void Test3()
        {
            // instructions aren't changed
            var TAC = GenTAC(@"
var a;
1:  if (1 < 2) 
        a = 4 + 5 * 6;
    else
        goto 4;
");
            var expectedTAC = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto 4",
                "goto L2",
                "L1: #t2 = 5 * 6",
                "#t3 = 4 + #t2",
                "a = #t3",
                "L2: noop"
            };
            var expectedOptimize = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto 4",
                "goto L2",
                "L1: #t2 = 5 * 6",
                "#t3 = 4 + #t2",
                "a = #t3",
                "L2: noop"
            };

            CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var optimizations = new List<Optimization> { ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto };

            CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expectedOptimize, actual);
        }

        [Test]
        public void Test4()
        {
            // instructions aren't changed
            var TAC = GenTAC(@"
var a;
1:  if (1 < 2) 
        a = 4 + 5 * 6;
");
            var expectedTAC = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto L2",
                "L1: #t2 = 5 * 6",
                "#t3 = 4 + #t2",
                "a = #t3",
                "L2: noop"
            };
            var expectedOptimize = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto L2",
                "L1: #t2 = 5 * 6",
                "#t3 = 4 + #t2",
                "a = #t3",
                "L2: noop"
            };

            CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var optimizations = new List<Optimization> { ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto };

            CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expectedOptimize, actual);
        }
    }
}
