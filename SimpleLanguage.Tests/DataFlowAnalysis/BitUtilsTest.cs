using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;


namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class BitUtilsTest
    {

        [Test]
        public void ToBitsTestEmptyMap()
        {
            var instructions = new List<Instruction> {
                new Instruction("a", "", "", "", ""),
                new Instruction("b", "", "", "", ""),
                new Instruction("c", "", "", "", ""),
            };

            var map = new Dictionary<Instruction, int> { };

            var result = BitUtils.TurnIntoBits(instructions, map);

            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void ToBitsTestEmptyInstructions()
        {
            var instructions = new List<Instruction> { };

            var map = new Dictionary<Instruction, int>
            {
                { new Instruction("a", "", "", "", ""), 0 }
            };

            var result = BitUtils.TurnIntoBits(instructions, map);

            Assert.IsTrue(result.Count == 1);
            Assert.IsFalse(result[0]);
        }

        [Test]
        public void ToBitsTestAllTrue()
        {
            var instructions = new List<Instruction> {
               new Instruction("a", "", "", "", ""),
               new Instruction("b", "", "", "", ""),
               new Instruction("c", "", "", "", ""),
            };

            var map = new Dictionary<Instruction, int>
            {
                { instructions[0], 0 },
                { instructions[1], 1 },
                { instructions[2], 2 },
            };


            var result = BitUtils.TurnIntoBits(instructions, map);

            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result[0]);
            Assert.IsTrue(result[1]);
            Assert.IsTrue(result[2]);
        }

        [Test]
        public void ToBitsTestAllFalse()
        {
            var instructions = new List<Instruction> {
                new Instruction("a", "", "", "", ""),
                new Instruction("b", "", "", "", ""),
                new Instruction("c", "", "", "", ""),
            };

            var map = new Dictionary<Instruction, int>
            {
                { instructions[0], 0 },
                { instructions[1], 1 },
                { instructions[2], 2 },
            };

            var instructionsToTestAgainst = new List<Instruction> {
                new Instruction("d", "", "", "", ""),
                new Instruction("e", "", "", "", ""),
                new Instruction("f", "", "", "", ""),
            };


            var result = BitUtils.TurnIntoBits(instructionsToTestAgainst, map);

            Assert.IsTrue(result.Count == 3);
            Assert.IsFalse(result[0]);
            Assert.IsFalse(result[1]);
            Assert.IsFalse(result[2]);
        }

        [Test]
        public void ToBitsBasicTest()
        {
            var instructions = new List<Instruction> {
                new Instruction("a", "", "", "", ""),
                new Instruction("b", "", "", "", ""),
                new Instruction("c", "", "", "", ""),
            };

            var map = new Dictionary<Instruction, int>
            {
                { instructions[0], 0 },
                { instructions[1], 1 },
                { instructions[2], 2 },
                { new Instruction("z", "", "", "", ""), 3}
            };

            var instructionsToTestAgainst = new List<Instruction> {
                instructions[2],
                new Instruction("e", "", "", "", ""),
                instructions[0]
            };

            var result = BitUtils.TurnIntoBits(instructionsToTestAgainst, map);

            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result[0]);
            Assert.IsFalse(result[1]);
            Assert.IsTrue(result[2]);
            Assert.IsFalse(result[3]);
        }

        [Test]
        public void ToInstructionsTestEmpty()
        {
            var instructions = new List<Instruction> {
                new Instruction("a", "", "", "", ""),
                new Instruction("b", "", "", "", ""),
                new Instruction("c", "", "", "", ""),
            };

            var bits = new BitArray(0);

            var result = BitUtils.TurnIntoInstructions(bits, instructions);

            Assert.IsTrue(result.ToList().Count == 0);
        }

        [Test]
        public void ToInstructionsTestOneTrue()
        {
            var instructions = new List<Instruction> {
                new Instruction("a", "", "", "", ""),
            };

            var bits = new BitArray(new[] { true });

            var result = BitUtils.TurnIntoInstructions(bits, instructions).ToList();

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0] == instructions[0]);
        }

        [Test]
        public void ToInstructionsTestOneFalse()
        {
            var instructions = new List<Instruction> {
                new Instruction("a", "", "", "", ""),
            };

            var bits = new BitArray(new[] { false });

            var result = BitUtils.TurnIntoInstructions(bits, instructions).ToList();

            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void ToInstructionsTestBasic()
        {
            var instructions = new List<Instruction> {
                new Instruction("a", "", "", "", ""),
                new Instruction("b", "", "", "", ""),
                new Instruction("c", "", "", "", ""),
            };

            var bits = new BitArray(new[] { true, false, true, false });

            var result = BitUtils.TurnIntoInstructions(bits, instructions).ToList();

            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result[0] == instructions[0]);
            Assert.IsTrue(result[1] == instructions[2]);
        }

        [Test]
        public void ExceptTestEmpty()
        {
            var a = new BitArray(0);
            var b = new BitArray(0);
            Assert.IsEmpty(BitUtils.Except(a, b));
        }

        [Test]
        public void ExceptTest1()
        {
            var a = new BitArray(new[] { true });
            var b = new BitArray(new[] { false });
            Assert.IsTrue(BitUtils.Except(a, b)[0]);
        }

        [Test]
        public void ExceptTest2()
        {
            var a = new BitArray(new[] { false });
            var b = new BitArray(new[] { true });
            Assert.IsFalse(BitUtils.Except(a, b)[0]);
        }

        [Test]
        public void ExceptTest3()
        {
            var a = new BitArray(new[] { true });
            var b = new BitArray(new[] { true });
            Assert.IsFalse(BitUtils.Except(a, b)[0]);
        }

        [Test]
        public void ExceptTest4()
        {
            var a = new BitArray(new[] { false });
            var b = new BitArray(new[] { false });
            Assert.IsFalse(BitUtils.Except(a, b)[0]);
        }

        [Test]
        public void ExceptTestBasic()
        {
            var a = new BitArray(new[] { true, true, true, false, false, false, false });
            var b = new BitArray(new[] { true, true, false, false, false, false, true });

            var result = BitUtils.Except(a, b);

            Assert.IsFalse(result[0]);
            Assert.IsFalse(result[1]);
            Assert.IsTrue(result[2]);
            Assert.IsFalse(result[3]);
            Assert.IsFalse(result[4]);
            Assert.IsFalse(result[5]);
            Assert.IsFalse(result[6]);
        }

        [Test]
        public void TestGrouping()
        {
            var instructions = new List<Instruction> {
                new Instruction("a", "", "", "", ""),
                new Instruction("b", "", "", "", ""),
                new Instruction("c", "", "", "", ""),
                new Instruction("d", "", "", "", ""),
                new Instruction("e", "", "", "", "")
            };

            var idsByInstruction = instructions
                .Select((value, index) => (value, index))
                .ToDictionary(x => x.value, x => x.index);

            var block1 = new BasicBlock(new List<Instruction>() { instructions[0], instructions[1] });
            var block2 = new BasicBlock(new List<Instruction>() { instructions[2], instructions[3] });

            var defs = new List<DefinitionInfo>
            {
                new DefinitionInfo { BasicBlock = block1, Instruction = instructions[0] },
                new DefinitionInfo { BasicBlock = block1, Instruction = instructions[1] },
                new DefinitionInfo { BasicBlock = block2, Instruction = instructions[2] },
                new DefinitionInfo { BasicBlock = block2, Instruction = instructions[3] },
            };

            var result = BitUtils.GroupByBlockAndTurnIntoInstructions(defs, idsByInstruction);

            var block1Instructions = result[block1];
            var block2Instructions = result[block2];

            CollectionAssert.AreEqual(block1Instructions, new BitArray(new[] { true, true, false, false, false }));
            CollectionAssert.AreEqual(block2Instructions, new BitArray(new[] { false, false, true, true, false }));
        }
    }
}
