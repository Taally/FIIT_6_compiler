using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class RemoveNoopTest : TACTestsBase
    {
        [Test]
        public void Test1()
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
    }
}
