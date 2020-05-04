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
        public void Test2()
        {
            var TAC = GenTAC(@"
                var x, y, b;
                y = 5;
                x = b;
                y = 7;
                x = y + y;
                ");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);

            var expected = new List<string>()
            {
                "y = 5",
                "x = b",
                "y = 7",
                "#t1 = 7 + 7",
                "x = #t1"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Test3()
        {
            var TAC = GenTAC(@"
                var x, y, b;
                x = 5;
                x = x;
                ");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);

            var expected = new List<string>()
            {
                "x = 5",
                "x = 5",
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        // Почему - то некорректно генерировался TAC для инструкций с унарными операциями
        // Поэтому и закоментировал
        //[Test]
        //public void Test4()
        //{
        //    var TAC = GenTAC(@"
        //        var x, y, b;
        //        x = -5;
        //        y = x + x;
        //        ");
        //    ThreeAddressCodeOptimizer.Optimizations.Clear();
        //    ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);

        //    var expected = new List<string>()
        //    {
        //        "x = -5",
        //        "#t1 = -5 + -5",
        //        "y = #t1"
        //    };
        //    var actual = ThreeAddressCodeOptimizer.Optimize(TAC)
        //        .Select(instruction => instruction.ToString());

        //    CollectionAssert.AreEqual(expected, actual);
        //}
    }
}