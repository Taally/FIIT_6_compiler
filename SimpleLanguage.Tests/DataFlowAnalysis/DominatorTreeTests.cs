using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    using InOutInfo = InOutData<IEnumerable<BasicBlock>>;
    [TestFixture]
    internal class DominatorTreeTests : TACTestsBase
    {
        private (List<BasicBlock> basicBlocks, ControlFlowGraph cfg, InOutInfo inOutInfo) GenGraphAndGetInOutInfo(string program)
        {
            var TAC = GenTAC(program);
            var optResult = ThreeAddressCodeOptimizer.OptimizeAll(TAC);
            var blocks = BasicBlockLeader.DivideLeaderToLeader(optResult);
            var cfg = new ControlFlowGraph(blocks);
            var inOutInfo = new DominatorTree().Execute(cfg);
            return (blocks, cfg, inOutInfo);
        }

        [Test]
        public void EmptyProgram()
        {
            (_, _, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
");
            // no basic blocks + entry and exit
            Assert.AreEqual(2, inOutInfo.Count);
        }

        [Test]
        public void BasicTest()
        {
            (var blocks, var cfg, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
a = 1;
");
            // only one basic block + entry and exit
            Assert.AreEqual(3, inOutInfo.Count);

            Assert.AreEqual(2, inOutInfo[blocks[0]].Out.Count()); // entry block and itself
            var expected = new BasicBlock[] { blocks[0], cfg.GetCurrentBasicBlocks().First() };
            CollectionAssert.AreEquivalent(expected, inOutInfo[blocks[0]].Out);
        }

        [Test]
        public void BranchingTest()
        {
            (var blocks, var cfg, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
input(a);
1: if a == 0
    goto 2;
a = 2;
2: a = 3;
");
            /*
            input(a);
            1: if a == 0
                goto 2;
            */
            Assert.AreEqual(2, inOutInfo[blocks[0]].Out.Count()); // entry block and itself
            var expected = new BasicBlock[] { cfg.GetCurrentBasicBlocks().First(), blocks[0] };
            CollectionAssert.AreEquivalent(expected, inOutInfo[blocks[0]].Out);

            /*
            a = 2
             */
            Assert.AreEqual(3, inOutInfo[blocks[1]].Out.Count()); // entry block, itself and first block
            expected = new BasicBlock[] { cfg.GetCurrentBasicBlocks().First(), blocks[1], blocks[0] };
            CollectionAssert.AreEquivalent(expected, inOutInfo[blocks[1]].Out);

            /*
            2: a = 3;
             */
            Assert.AreEqual(3, inOutInfo[blocks[2]].Out.Count()); // entry block, itself and first block
            expected = new BasicBlock[] { cfg.GetCurrentBasicBlocks().First(), blocks[2], blocks[0] };
            CollectionAssert.AreEquivalent(expected, inOutInfo[blocks[2]].Out);

            /*
            exit block
             */
            Assert.AreEqual(4, inOutInfo[cfg.GetCurrentBasicBlocks().Last()].Out.Count()); // entry block, itself, first block and `2: a = 3;`
            expected = new BasicBlock[]
            {
                cfg.GetCurrentBasicBlocks().First(),
                cfg.GetCurrentBasicBlocks().Last(),
                blocks[0],
                blocks[2]
            };
            CollectionAssert.AreEquivalent(expected, inOutInfo[cfg.GetCurrentBasicBlocks().Last()].Out);
        }
    }
}
