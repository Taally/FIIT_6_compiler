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
                new BasicBlock(new List<Instruction>(){new Instruction("", "goto", "3", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("", "assign", "54", "", "a")}),
                new BasicBlock(new List<Instruction>(){new Instruction("3", "assign", "11", "", "b")}),
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
                new BasicBlock(new List<Instruction>(){new Instruction("", "goto", "4", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("", "assign", "54", "", "a"), new Instruction("3", "assign", "11", "", "b")}),
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
                new BasicBlock(new List<Instruction>(){new Instruction("", "assign", "54", "", "a"), new Instruction("", "assign", "11", "", "b")}),
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
                new BasicBlock(new List<Instruction>(){new Instruction("", "goto", "3", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("", "goto", "4", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("", "assign", "54", "", "a"), new Instruction("", "goto", "5", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("", "assign", "11", "", "b")}),
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
                new BasicBlock(new List<Instruction>(){new Instruction("", "goto", "1", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("1", "goto", "1", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("2", "goto", "1", "", "")}),
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
                var tmpe = expected[i].GetInstructions();
                var tmpa = actual[i].GetInstructions();
                Assert.AreEqual(tmpe.Count, tmpa.Count);

                for (int j = 0; j < tmpe.Count; j++)
                {
                    var a = tmpe[j].ToString();
                    var b = tmpa[j].ToString();
                    Assert.AreEqual(a, b);
                }
            }
        }
    }
}
