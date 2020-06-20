using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.CFG
{
    [TestFixture]
    internal class CFGUnreachableCodeEliminationTest : TACTestsBase
    {
        [Test]
        public void EliminationdOneBlockTest()
        {
            var TAC = GenTAC(@"
var a, b, c;

goto 1;
goto 2;
2: a = 42;
1: b = 3;
c = 5;
");
            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));

            var actual = cfg.GetCurrentBasicBlocks().ToList();

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){new Instruction("", "goto", "1", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("1", "assign", "3", "", "b"), new Instruction("", "assign", "5", "", "c")}),
            };


            AssertSet(expected, actual.Skip(1).Take(actual.Count - 2).ToList());

        }

        [Test]
        public void EliminationdMultyBlocksTest1()
        {
            var TAC = GenTAC(@"
var a, b, c;

goto 1;
111:a = 1; 
goto 55; 
55: goto 10; 
10: goto 111; 
if a>a goto 10; 
else goto 111; 
 
c = c; 
if a==b 
	b = b; 
 
a = -1; 
b = -a; 
c = -(a+b); 
a = !b; 
c = !(a == b); 
1: b = 3;
");
            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));

            var actual = cfg.GetCurrentBasicBlocks().ToList();

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){new Instruction("", "goto", "1", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("1", "assign", "3", "", "b")}),
            };


            AssertSet(expected, actual.Skip(1).Take(actual.Count - 2).ToList());

        }


        [Test]
        public void EliminationdMultyBlocksTest2()
        {
            var TAC = GenTAC(@"
var a, b, c;

goto 1;
111:a = 1; 
goto 55; 
55: goto 10; 
10: goto 111; 
if a>a goto 10; 
else goto 111; 
 
c = c; 
if a==b 
	b = b; 

2: a = -1; 
b = -a; 
c = -(a+b); 
a = !b; 
c = !(a == b); 
1: b = 3;
goto 2;
");
            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));

            var actual = cfg.GetCurrentBasicBlocks().ToList();

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){TAC[0]}),
                new BasicBlock(new List<Instruction>(){TAC[17],TAC[18],TAC[19],TAC[20],TAC[21],TAC[22],TAC[23],TAC[24],TAC[25],TAC[26],TAC[27],TAC[28] }),
                new BasicBlock(new List<Instruction>(){TAC[29], TAC[30]}),
            };


            AssertSet(expected, actual.Skip(1).Take(actual.Count - 2).ToList());

        }

        [Test]
        public void EliminationdMultyBlocksTest3()
        {
            var TAC = GenTAC(@"
var a, b, c;
2: goto 1;
1: goto 2;
goto 3;
3: a = 5;

");
            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));

            var actual = cfg.GetCurrentBasicBlocks().ToList();

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){TAC[0]}),
                new BasicBlock(new List<Instruction>(){TAC[1]}),
            };


            AssertSet(expected, actual.Skip(1).Take(actual.Count - 2).ToList());

        }

        [Test]
        public void NotEliminationBlocksTest()
        {
            var TAC = GenTAC(@"
var a, b, c;

goto 1;
1: b = 3;
c = 5;
goto 2;
2: b = 53;
c = b == 53;
");
            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));

            List<BasicBlock> actual = cfg.GetCurrentBasicBlocks().ToList();

            var expected = new List<BasicBlock>()
            {
                new BasicBlock(new List<Instruction>(){new Instruction("", "goto", "1", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("1", "assign", "3", "", "b"), new Instruction("", "assign", "5", "", "c"),new Instruction("", "goto", "2", "", "")}),
                new BasicBlock(new List<Instruction>(){new Instruction("2", "assign", "53", "", "b"), new Instruction("", "EQUAL", "b", "53", "#t1"),new Instruction("", "assign", "#t1", "", "c")}),
            };


            AssertSet(expected, actual.Skip(1).Take(actual.Count - 2).ToList());

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
