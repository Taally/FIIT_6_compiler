using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class CommonExprEliminationTest : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
var a, b, c, d, e, f, g, h, k;
a = b + c;
c = a + g;
k = b + c;
");
            
            var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
            Assert.IsFalse(ok);
            var expected = new List<string>()
            {

                "#t1 = b + c",
                "a = #t1",
                "#t2 = a + g",
                "c = #t2",
                "#t3 = b + c",
                "k = #t3"
            };
            var actual = instructions.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test2()
        {
            var TAC = GenTAC(@"
var a, b, c, d, e, f, g, h, k;
a = b + c;
k = b + c;
");

            var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
            Assert.IsTrue(ok);
            var expected = new List<string>()
            {
                "#t1 = b + c",
                "a = #t1",
                "#t2 = #t1",
                "k = #t2"
            };
            var actual = instructions.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void CommutativeOpTest()
        {
            var TAC = GenTAC(@"
var a, b, c, k;
a = b + c;
k = c + b;
");

            var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
            Assert.IsTrue(ok);
            var expected = new List<string>()
            {
                "#t1 = b + c",
                "a = #t1",
                "#t2 = #t1",
                "k = #t2"
            };
            var actual = instructions.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void NotCommutativeOpTest()
        {
            var TAC = GenTAC(@"
var a, b, c, k;
a = b - c;
k = c - b;
");

            var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
            Assert.IsFalse(ok);
            var expected = new List<string>()
            {
                "#t1 = b - c",
                "a = #t1",
                "#t2 = c - b",
                "k = #t2"
            };
            var actual = instructions.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test5()
        {
            var TAC = GenTAC(@"
var a, b, c, k;
a = b * c;
b = b * c;
k = b * c;
");

            var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
            Assert.IsTrue(ok);
            var expected = new List<string>()
            {
                "#t1 = b * c",
                "a = #t1",
                "#t2 = #t1",
                "b = #t2",
                "#t3 = b * c",
                "k = #t3"
            };
            var actual = instructions.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void UnarOp()
        {
            var TAC = GenTAC(@"
var a, b, c, k;
a = -b;
k = -b;
");

            var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
            Assert.IsTrue(ok);
            var expected = new List<string>()
            {
                "#t1 = -b",
                "a = #t1",
                "#t2 = #t1",
                "k = #t2"
            };
            var actual = instructions.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void NotUnarOp()
        {
            var TAC = GenTAC(@"
var a, b, c, k;
a = b;
k = b;
");

            var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
            Assert.IsFalse(ok);
            var expected = new List<string>()
            {
                "a = b",
                "k = b"
            };
            var actual = instructions.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Constants()
        {
            var TAC = GenTAC(@"
var a, b, c, k;
a = 5;
k = 5;
");

            var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
            Assert.IsFalse(ok);
            var expected = new List<string>()
            {
                "a = 5",
                "k = 5"
            };
            var actual = instructions.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
