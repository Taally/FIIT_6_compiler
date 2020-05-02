using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests
{
    [TestFixture]
    public class TACSimpleTests : TACTestsBase
    {
        [Test]
        public void FoldConstantsTest()
        {
            var TAC = GenTAC(@"
var a;
a = 1 - 20;
a = 4 * 2;
a = 10 / 5;
a = 9 + 3;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);

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
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void PropagateConstantsTest()
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

        [Test]
        public void PropagateCopiesTest()
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

        [Test]
        public void DeleteDeadCodeWithDeadVarsTest()
        {
            var TAC = GenTAC(@"
var a, b, c;
a = 1;
a = 2;
b = 11;
b = 22;
a = 3;
a = b;
c = 1;
a = b + c;
b = -c;
c = 1;
b = a - c;
a = -b;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(DeleteDeadCodeWithDeadVars.DeleteDeadCode);

            var expected = new List<string>()
            {
                "noop",
                "noop",
                "noop",
                "b = 22",
                "noop",
                "noop",
                "c = 1",
                "#t1 = b + c",
                "a = #t1",
                "noop",
                "noop",
                "c = 1",
                "#t3 = a - c",
                "b = #t3",
                "#t4 = -b",
                "a = #t4",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void DeleteDeadCodeWithDeadVarsTempVarsTest()
        {
            var TAC = GenTAC(@"
var a;
a = -a;
a = 1;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(DeleteDeadCodeWithDeadVars.DeleteDeadCode);

            var expected = new List<string>()
            {
                "noop",
                "noop",
                "a = 1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void DeleteDeadCodeWithDeadVarsNegationTest()
        {
            var TAC = GenTAC(@"
var a;
a = true;
a = !a;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(DeleteDeadCodeWithDeadVars.DeleteDeadCode);

            var expected = new List<string>()
            {
                "a = True",
                "#t1 = !a",
                "a = #t1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }


        [Test]
        public void DefUseTest()
        {
            var TAC = GenTAC(@"
var a, b, c;
a = 1;
a = 2;
b = 11;
b = 22;
a = 3;
a = b;
c = 1;
a = b + c;
b = -c;
c = 1;
b = a - c;
a = -b;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeDefUse.DeleteDeadCode);

            var expected = new List<string>()
            {
                "noop",
                "noop",
                "noop",
                "b = 22",
                "noop",
                "noop",
                "c = 1",
                "#t1 = b + c",
                "a = #t1",
                "noop",
                "noop",
                "c = 1",
                "#t3 = a - c",
                "b = #t3",
                "#t4 = -b",
                "a = #t4",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GotoToGotoTest()
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

        [Test]
        public void RemoveNoopTest()
        {
            var TAC = new List<Instruction>
            {
                new Instruction("1", "noop", null, null, null),
                new Instruction("2", "noop", null, null, null),
                new Instruction("3", "assign", "1", "", "a"),
                new Instruction("4", "noop", null, null, null),
                new Instruction("5", "noop", null, null, null),
                new Instruction("6", "assign", "a", "", "b"),
                new Instruction("7", "noop", null, null, null),
                new Instruction("8", "noop", null, null, null),
            };
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeRemoveNoop.RemoveEmptyNodes);

            var expected = new List<string>()
            {
                "3: a = 1",
                "6: b = a",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void AlgebraicIdentitiesTest()
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


        // TODO: add more cases in this test
        [Test]
        public void GotoThroughGotoTest()
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