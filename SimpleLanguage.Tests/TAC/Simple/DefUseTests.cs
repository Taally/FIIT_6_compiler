using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    using Optimization = Func<List<Instruction>, (bool, List<Instruction>)>;

    [TestFixture]
    internal class DefUseTests : TACTestsBase
    {
        [Test]
        public void VarAssignSimple()
        {
            var TAC = GenTAC(@"
var a, b, x;
x = a;
x = b;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "x = b"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void NegVarAssignSimple1()
        {
            var TAC = GenTAC(@"
var a, b, x;
x = -a;
x = b;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "noop",
                "x = b"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void NegVarAssignSimple2()
        {
            var TAC = GenTAC(@"
var a, b, x;
x = a;
x = -b;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "#t1 = -b",
                "x = #t1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ContAssignSimple()
        {
            var TAC = GenTAC(@"
var x;
x = 1;
x = 2;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "x = 2"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void NegContAssignSimple1()
        {
            var TAC = GenTAC(@"
var x;
x = -1;
x = 2;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "noop",
                "x = 2"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void NegContAssignSimple2()
        {
            var TAC = GenTAC(@"
var x;
x = 1;
x = -2;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "#t1 = -2",
                "x = #t1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void BoolNoDead()
        {
            var TAC = GenTAC(@"
var a, b, x;
x = a or b;
x = !x;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "#t1 = a or b",
                "x = #t1",
                "#t2 = !x",
                "x = #t2"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void BoolDead()
        {
            var TAC = GenTAC(@"
var a, b, x;
x = a or b;
x = !a;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "noop",
                "#t2 = !a",
                "x = #t2"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void NoDeadCode()
        {
            var TAC = GenTAC(@"
var a, b, c;
a = 2;
b = a + 4;
c = a * b;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "a = 2",
                "#t1 = a + 4",
                "b = #t1",
                "#t2 = a * b",
                "c = #t2"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void DeadBeforeInput()
        {
            var TAC = GenTAC(@"
var a, b;
a = 1;
input(a);
b = a + 1;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "input a",
                "#t1 = a + 1",
                "b = #t1"
            };

            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void NoDeadInputPrint()
        {
            var TAC = GenTAC(@"
var a, b;
a = 1;
print(a);
input(a);
b = a + 1;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "a = 1",
                "print a",
                "input a",
                "#t1 = a + 1",
                "b = #t1"
            };

            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void DeadInput()
        {
            var TAC = GenTAC(@"
var a, b;
input(a);
input(a);
b = a + 1;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "input a",
                "#t1 = a + 1",
                "b = #t1"
            };

            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ArithmeticOpsNoDead()
        {
            var TAC = GenTAC(@"
var a, b, c, x;
a = (2 + x) - a;
b = (c * 3) - (b / 4);
c = (a * 10 + b * 2) / 3;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "#t1 = 2 + x",
                "#t2 = #t1 - a",
                "a = #t2",
                "#t3 = c * 3",
                "#t4 = b / 4",
                "#t5 = #t3 - #t4",
                "b = #t5",
                "#t6 = a * 10",
                "#t7 = b * 2",
                "#t8 = #t6 + #t7",
                "#t9 = #t8 / 3",
                "c = #t9"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ArithmeticOpsDead()
        {
            var TAC = GenTAC(@"
var a, b, c, x;
a = (2 + x) - a;
b = (c * 3) - (b / 4);
c = (a * 10 + b * 2) / 3;
a = 1;
b = 1;
c = 1;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "a = 1",
                "b = 1",
                "c = 1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GotoNoDead()
        {
            var TAC = GenTAC(@"
var a, b, c;
a = a or b;
goto 777;
b = 2;
777: 
c = 1;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "#t1 = a or b",
                "a = #t1",
                "goto 777",
                "b = 2",
                "777: c = 1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GotoDead()
        {
            var TAC = GenTAC(@"
var a, b, c;
a = a or b;
goto 777;
777: c = 1;
c = 2;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "#t1 = a or b",
                "a = #t1",
                "goto 777",
                "777: noop",
                "c = 2"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }


        [Test]
        public void IfNoDead()
        {
            var TAC = GenTAC(@"
var a, b, c;
a = 2;
if a
{
b = 1;
c = b + 3;
}
a = 3;
");
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

            var expected = new List<string>()
            {
                "a = 2",
                "if a goto L1",
                "goto L2",
                "L1: b = 1",
                "#t1 = b + 3",
                "c = #t1",
                "L2: noop",
                "a = 3"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
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
            var optimizations = new List<Optimization> { ThreeAddressCodeDefUse.DeleteDeadCode };

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
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
