using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class RemoveNoopTest : TACTestsBase
    {
        public Tuple<bool, List<Instruction>> OptimizeLocal(List<Instruction> tac)
        {
            return ThreeAddressCodeRemoveNoop.RemoveEmptyNodes(tac);
        }

        public void AssertChanged(Tuple<bool, List<Instruction>> result, List<String> expected)
        {
            Assert.IsTrue(result.Item1);
            CollectionAssert.AreEqual(result.Item2.Select(x => x.ToString()), expected);
        }

        public void AssertNotChanged(Tuple<bool, List<Instruction>> result, List<String> expected)
        {
            Assert.IsFalse(result.Item1);
            CollectionAssert.AreEqual(result.Item2.Select(x => x.ToString()), expected);
        }
        
        [Test]
        public void ShouldNotRemoveLastNoop()
        {
            var TAC = new List<Instruction>
            {
                new Instruction("6", "assign", "b", "", "a"),
                new Instruction("", "noop", null, null, null)
            };
            var result = OptimizeLocal(TAC);
            var expected = new List<String>
            {
                "6: a = b",
                "noop"
            };
            
            AssertNotChanged(result, expected);
        }

        [Test]
        public void ShouldRemoveOnlyOneNoop()
        {
            var TAC = new List<Instruction>
            {
                new Instruction("6", "assign", "b", "", "a"),
                new Instruction("1", "noop", null, null, null),
                new Instruction("9", "assign", "a", "", "b"),
                new Instruction("", "noop", null, null, null),
            };

            var result = OptimizeLocal(TAC);
            
            var expected = new List<string>()
            {
                "6: a = b",
                "9: b = a",
                "noop"
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

            var expected = new List<String>
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
           
            var expected = new List<string>()
            {
                "3: a = 1",
                "6: b = a",
                "8: noop"
            };
           
            AssertChanged(OptimizeLocal(TAC), expected);
        }
    }
}
