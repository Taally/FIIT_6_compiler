using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class RemoveNoopTest : TACTestsBase
    {
        public (bool wasChanged, List<Instruction> instructions) OptimizeLocal(List<Instruction> tac) => ThreeAddressCodeRemoveNoop.RemoveEmptyNodes(tac);

        public void AssertChanged((bool wasChanged, List<Instruction> instructions) result, List<string> expected)
        {
            Assert.IsTrue(result.wasChanged);
            CollectionAssert.AreEqual(result.instructions.Select(x => x.ToString()), expected);
        }

        public void AssertNotChanged((bool wasChanged, List<Instruction> instructions) result, List<string> expected)
        {
            Assert.IsFalse(result.wasChanged);
            CollectionAssert.AreEqual(result.instructions.Select(x => x.ToString()), expected);
        }

        [Test]
        public void ShouldWorkWithEmptyList()
        {
            var TAC = new List<Instruction> { };
            var (wasChanged, instructions) = OptimizeLocal(TAC);
            CollectionAssert.AreEqual(instructions, TAC);
            Assert.IsFalse(wasChanged);
        }

        [Test]
        public void ShouldNotRemoveLastNoopIfItHasLabel()
        {
            var TAC = new List<Instruction>
            {
                new Instruction("6", "assign", "b", "", "a"),
                new Instruction("L1", "noop", null, null, null)
            };
            var result = OptimizeLocal(TAC);
            var expected = new List<string>
            {
                "6: a = b",
                "L1: noop"
            };

            AssertNotChanged(result, expected);
        }

        [Test]
        public void ShouldRemoveLastNoopIfItHasNoLabel()
        {
            var TAC = new List<Instruction>
            {
                new Instruction("6", "assign", "b", "", "a"),
                new Instruction("", "noop", null, null, null)
            };
            var result = OptimizeLocal(TAC);
            var expected = new List<string>
            {
                "6: a = b"
            };

            AssertChanged(result, expected);
        }

        [Test]
        public void ShouldRemoveOnlyOneNoop()
        {
            var TAC = new List<Instruction>
            {
                new Instruction("6", "assign", "b", "", "a"),
                new Instruction("1", "noop", null, null, null),
                new Instruction("9", "assign", "a", "", "b"),
                new Instruction("L1", "noop", null, null, null),
            };

            var result = OptimizeLocal(TAC);

            var expected = new List<string>()
            {
                "6: a = b",
                "9: b = a",
                "L1: noop"
            };

            AssertChanged(result, expected);
        }

        [Test]
        public void ShouldConcatNoopWithLabelWithNextOpIfItHasNoLabel()
        {
            var TAC = new List<Instruction>
            {
                new Instruction("L1", "noop", "", "", ""),
                new Instruction("", "assign", "b", "", "a")
            };

            var expected = new List<string>()
            {
                "L1: a = b"
            };

            AssertChanged(OptimizeLocal(TAC), expected);
        }

        [Test]
        public void ShouldRenameGotosToNoopWithLabelWhenNextIsLabeled()
        {
            var TAC = new List<Instruction>
            {
                new Instruction("", "goto", "old_label", "", ""),
                new Instruction("old_label", "noop", "", "", ""),
                new Instruction("new_label", "assign", "a", "", "b"),
                new Instruction("", "goto", "old_label", "", "")
            };

            var expected = new List<string>
            {
                "goto new_label",
                "new_label: b = a",
                "goto new_label",
            };

            AssertChanged(OptimizeLocal(TAC), expected);
        }

        [Test]
        public void ShouldRemoveAllTheNoops()
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
            var optimizations = new List<Func<List<Instruction>, (bool, List<Instruction>)>>
            {
                ThreeAddressCodeRemoveNoop.RemoveEmptyNodes
            };

            var expected = new List<string>()
            {
                "3: a = 1",
                "6: b = a",
                "8: noop"
             };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);

            AssertChanged(OptimizeLocal(TAC), expected);
        }
    }
}
