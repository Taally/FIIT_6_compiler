using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    using Optimization = Func<List<Instruction>, (bool, List<Instruction>)>;

    [TestFixture]
    internal class PropagateCopiesTest : TACTestsBase
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

            var optimizations = new List<Optimization> { ThreeAddressCodeCopyPropagation.PropagateCopies };

            var expected = new List<string>()
            {
                "a = b",
                "#t1 = b - b",
                "c = #t1",
                "#t2 = #t1 + 1",
                "d = #t2",
                "#t3 = #t2 * b",
                "e = #t3",
                "#t4 = x - y",
                "a = #t4",
                "#t5 = #t1 + #t4",
                "k = #t5"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test2()
        {
            var TAC = GenTAC(@"
                var a, b, c, d, e, x, y, k;
                b = x;
                x = 5;
                c = b + 5;
                d = c;
                e = d;
                ");
            var optimizations = new List<Optimization> { ThreeAddressCodeCopyPropagation.PropagateCopies };

            var expected = new List<string>()
            {
                "b = x",
                "x = 5",
                "#t1 = b + 5",
                "c = #t1",
                "d = #t1",
                "e = #t1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test3()
        {
            var TAC = GenTAC(@"
                var a, b, c, d, e, x, y, k;
                a = b;
                b = d + a;
                e = a;
                ");
            var optimizations = new List<Optimization> { ThreeAddressCodeCopyPropagation.PropagateCopies };

            var expected = new List<string>()
            {
                "a = b",
                "#t1 = d + b",
                "b = #t1",
                "e = a",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
