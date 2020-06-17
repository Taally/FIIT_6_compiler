using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class BasicBlockLeaderTest : TACTestsBase
    {
        [Test]
        public void LabelAliveTest()
        {
            var TAC = GenTAC(@"
var a, b, c;
goto 3;
a = 54;
3: b = 11;
");

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){new Instruction("3", "", "", "goto", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("54", "", "", "assign", "a")}),
                new BasicBlock(new List<Instruction>(){new Instruction("11", "3", "", "assign", "b")}),
            };
            var actual = BasicBlockLeader.DivideLeaderToLeader(TAC);

            AssertSet(expected, actual);
        }

        [Test]
        public void LabelNotAliveTest()
        {
            var TAC = GenTAC(@"
var a, b, c;
goto 4;
a = 54;
3: b = 11;
");

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){new Instruction("4", "", "", "goto", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("54", "", "", "assign", "a"), new Instruction("11", "3", "", "assign", "b")}),
            };
            var actual = BasicBlockLeader.DivideLeaderToLeader(TAC);

            AssertSet(expected, actual);
        }

        [Test]
        public void OneBlockTest()
        {
            var TAC = GenTAC(@"
var a, b, c;
a = 54;
b = 11;
");

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){new Instruction("54", "", "", "assign", "a"), new Instruction("11", "", "", "assign", "b")}),
            };
            var actual = BasicBlockLeader.DivideLeaderToLeader(TAC);

            AssertSet(expected, actual);
        }

        [Test]
        public void StandartGotoTest()
        {
            var TAC = GenTAC(@"
var a, b, c;
goto 3;
goto 4;
a = 54;
goto 5;
b = 11;
");

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){new Instruction("3", "", "", "goto", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("4", "", "", "goto", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("54", "", "", "assign", "a"), new Instruction("5", "", "", "goto", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("11", "", "", "assign", "b")}),
            };
            var actual = BasicBlockLeader.DivideLeaderToLeader(TAC);

            AssertSet(expected, actual);
        }

        [Test]
        public void DivBBGotoTest()
        {
            var TAC = GenTAC(@"
goto 1;
1: goto 1;
2: goto 1;
");

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){new Instruction("1", "", "", "goto", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("1", "", "1", "goto", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("1", "", "2", "goto", "")}),
            };
            var actual = BasicBlockLeader.DivideLeaderToLeader(TAC);

            AssertSet(expected, actual);
        }

        private void AssertSet(
            List<BasicBlock> expected,
            List<BasicBlock> actual)
        {
            for (var i = 0; i < expected.Count; ++i)
            {
                Assert.True(expected[i].ToString() == actual[i].ToString());
            }
        }
    }
}
