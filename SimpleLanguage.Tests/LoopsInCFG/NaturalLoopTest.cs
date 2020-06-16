using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.LoopsInCFG
{
    [TestFixture]
    internal class NaturalLoopTest : TACTestsBase
    {
        [Test]
        public void IntersectLoopsTest()
        {
            var TAC = GenTAC(@"
var a, b;

54: a = 5;
55: a = 6;
b = 6;
goto 54;
goto 55;
");

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            var expected = new List<List<BasicBlock>>()
            {
                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[1], TAC[2], TAC[3] }),
                    new BasicBlock(new List<Instruction>(){ TAC[4] })
                }
            };

            AssertSet(expected, actual);
        }

        [Test]
        public void NestedLoopsTest()
        {
            var TAC = GenTAC(@"
var a, b;

54: a = 5;
55: a = 6;
b = 6;
goto 55;
goto 54;

");

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            var expected = new List<List<BasicBlock>>()
            {
                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[1], TAC[2], TAC[3] })
                },
                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[0] }),
                    new BasicBlock(new List<Instruction>(){ TAC[1], TAC[2], TAC[3] }),
                    new BasicBlock(new List<Instruction>(){ TAC[4] })
                },


            };

            AssertSet(expected, actual);
        }

        [Test]
        public void OneRootLoopsTest()
        {
            var TAC = GenTAC(@"
var a, b;

54: a = 5;
b = 6;
goto 54;
goto 54;

");

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            var expected = new List<List<BasicBlock>>()
            {
                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[0], TAC[1], TAC[2] })
                },


                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[0], TAC[1], TAC[2] }),
                    new BasicBlock(new List<Instruction>(){ TAC[3] })
                }
            };

            AssertSet(expected, actual);
        }

        private void AssertSet(
            List<List<BasicBlock>> expected,
            List<List<BasicBlock>> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                for (var j = 0; j < expected[i].Count; j++)
                {
                    var e = expected[i][j].GetInstructions();
                    var a = actual[i][j].GetInstructions();

                    Assert.AreEqual(a.Count, e.Count);

                    foreach (var pair in a.Zip(e, (x, y) => (actual: x, expected: y)))
                    {
                        Assert.AreEqual(pair.actual.ToString(), pair.expected.ToString());
                    }
                }
            }
        }
    }
}
