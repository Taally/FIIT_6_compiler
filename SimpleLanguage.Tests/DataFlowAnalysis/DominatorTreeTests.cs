using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    using DominatorInfo = Dictionary<BasicBlock, IEnumerable<BasicBlock>>;

    [TestFixture]
    internal class DominatorTreeTests : TACTestsBase
    {
        private (List<BasicBlock> basicBlocks, ControlFlowGraph cfg, DominatorInfo dominators, DominatorTree.Tree tree) GenGraphAndGetInOutInfo(string program)
        {
            var TAC = GenTAC(program);
            var optResult = ThreeAddressCodeOptimizer.OptimizeAll(TAC);
            var blocks = BasicBlockLeader.DivideLeaderToLeader(optResult);
            var cfg = new ControlFlowGraph(blocks);
            var dt = new DominatorTree();
            var inOutInfo = dt.GetDominators(cfg);
            var tree = dt.Execute(cfg);
            return (blocks, cfg, inOutInfo, tree);
        }

        [Test]
        public void EmptyProgram()
        {
            (_, _, var dominators, var tree) = GenGraphAndGetInOutInfo(@"
var a;
");
            // no basic blocks + entry and exit
            Assert.AreEqual(2, dominators.Count);
        }

        [Test]
        public void BasicTest()
        {
            (var blocks, var cfg, var dominators, var tree) = GenGraphAndGetInOutInfo(@"
var a;
a = 1;
");
            // only one basic block + entry and exit
            Assert.AreEqual(3, dominators.Count);

            Assert.AreEqual(2, dominators[blocks[0]].Count()); // entry block and itself
            var expected = new BasicBlock[] { blocks[0], cfg.GetCurrentBasicBlocks().First() };
            CollectionAssert.AreEquivalent(expected, dominators[blocks[0]]);
        }

        [Test]
        public void BranchingTest()
        {
            (var blocks, var cfg, var dominators, var tree) = GenGraphAndGetInOutInfo(@"
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
            Assert.AreEqual(2, dominators[blocks[0]].Count()); // entry block and itself
            var expected = new BasicBlock[] { cfg.GetCurrentBasicBlocks().First(), blocks[0] };
            CollectionAssert.AreEquivalent(expected, dominators[blocks[0]]);

            /*
            a = 2
             */
            Assert.AreEqual(3, dominators[blocks[1]].Count()); // entry block, itself and first block
            expected = new BasicBlock[] { cfg.GetCurrentBasicBlocks().First(), blocks[1], blocks[0] };
            CollectionAssert.AreEquivalent(expected, dominators[blocks[1]]);

            /*
            2: a = 3;
             */
            Assert.AreEqual(3, dominators[blocks[2]].Count()); // entry block, itself and first block
            expected = new BasicBlock[] { cfg.GetCurrentBasicBlocks().First(), blocks[2], blocks[0] };
            CollectionAssert.AreEquivalent(expected, dominators[blocks[2]]);

            /*
            exit block
             */
            Assert.AreEqual(4, dominators[cfg.GetCurrentBasicBlocks().Last()].Count()); // entry block, itself, first block and `2: a = 3;`
            expected = new BasicBlock[]
            {
                cfg.GetCurrentBasicBlocks().First(),
                cfg.GetCurrentBasicBlocks().Last(),
                blocks[0],
                blocks[2]
            };
            CollectionAssert.AreEquivalent(expected, dominators[cfg.GetCurrentBasicBlocks().Last()]);
        }
    }
}
