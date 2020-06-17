using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.LoopsInCFG
{
    using ChildrenDictionary = Dictionary<int, IEnumerable<BasicBlock>>;
    using DominatorDictionary = Dictionary<int, IEnumerable<BasicBlock>>;
    using ParentsDictionary = Dictionary<int, BasicBlock>;

    [TestFixture]
    internal class DominatorTreeTests : TACTestsBase
    {
        private ControlFlowGraph BuildCFG(string program)
        {
            var TAC = GenTAC(program);
            var optResult = ThreeAddressCodeOptimizer.OptimizeAll(TAC);
            var blocks = BasicBlockLeader.DivideLeaderToLeader(optResult);
            return new ControlFlowGraph(blocks);
        }

        private void TestInternal(ControlFlowGraph graph,
            DominatorDictionary expectedDoms,
            ParentsDictionary expectedParents,
            ChildrenDictionary expectedChildren)
        {
            var dt = new DominatorTree();
            var dominators = dt.GetDominators(graph);
            var tree = dt.Execute(graph);
            var blocks = graph.GetCurrentBasicBlocks();

            Assert.AreEqual(blocks.Count, dominators.Count(), "Dominators count and blocks count are different");

            for (var i = 0; i < blocks.Count(); ++i)
            {
                CollectionAssert.AreEquivalent(expectedDoms[i], dominators[blocks[i]], $"Check dominators: error for block #{i}");
            }

            for (var i = 0; i < blocks.Count(); ++i)
            {
                Assert.AreEqual(expectedParents[i], tree.Parent(blocks[i]), $"Check parents: error for block #{i}");
                CollectionAssert.AreEquivalent(expectedChildren[i], tree.Children(blocks[i]), $"Check children: error for block #{i}");
            }
        }

        [Test]
        public void EmptyProgram()
        {
            var graph = BuildCFG(@"
var a;
");
            var blocks = graph.GetCurrentBasicBlocks();

            var expectedDoms = new DominatorDictionary
            {
                [0] = new[] { blocks[0] },
                [1] = new[] { blocks[0], blocks[1] }
            };
            var expectedParents = new ParentsDictionary
            {
                [0] = null,
                [1] = blocks[0]
            };
            var expectedChildren = new ChildrenDictionary
            {
                [0] = new[] { blocks[1] },
                [1] = Enumerable.Empty<BasicBlock>()
            };

            TestInternal(graph, expectedDoms, expectedParents, expectedChildren);
        }

        [Test]
        public void BasicTest()
        {
            var graph = BuildCFG(@"
var a;
a = 1;
");
            var blocks = graph.GetCurrentBasicBlocks();

            var expectedDoms = new DominatorDictionary
            {
                [0] = new[] { blocks[0] },
                [1] = new[] { blocks[0], blocks[1] },
                [2] = new[] { blocks[0], blocks[1], blocks[2] }
            };
            var expectedParents = new ParentsDictionary
            {
                [0] = null,
                [1] = blocks[0],
                [2] = blocks[1]
            };
            var expectedChildren = new ChildrenDictionary
            {
                [0] = new[] { blocks[1] },
                [1] = new[] { blocks[2] },
                [2] = Enumerable.Empty<BasicBlock>()
            };

            TestInternal(graph, expectedDoms, expectedParents, expectedChildren);
        }

        [Test]
        public void LinearStructTest()
        {
            var graph = BuildCFG(@"
var a, b, c, d, e, f;
1: a = 1;
b = 2;
goto 2;
2: c = 3;
d = 4;
goto 3;
3: e = 5;
goto 4;
4: f = 6;
");
            var blocks = graph.GetCurrentBasicBlocks();

            var expectedDoms = new DominatorDictionary
            {
                [0] = new[] { blocks[0] },
                [1] = new[] { blocks[0], blocks[1] },
                [2] = new[] { blocks[0], blocks[1], blocks[2] },
                [3] = new[] { blocks[0], blocks[1], blocks[2], blocks[3] },
                [4] = new[] { blocks[0], blocks[1], blocks[2], blocks[3], blocks[4] },
                [5] = new[] { blocks[0], blocks[1], blocks[2], blocks[3], blocks[4], blocks[5] }
            };
            var expectedParents = new ParentsDictionary
            {
                [0] = null,
                [1] = blocks[0],
                [2] = blocks[1],
                [3] = blocks[2],
                [4] = blocks[3],
                [5] = blocks[4]
            };
            var expectedChildren = new ChildrenDictionary
            {
                [0] = new[] { blocks[1] },
                [1] = new[] { blocks[2] },
                [2] = new[] { blocks[3] },
                [3] = new[] { blocks[4] },
                [4] = new[] { blocks[5] },
                [5] = Enumerable.Empty<BasicBlock>()
            };

            TestInternal(graph, expectedDoms, expectedParents, expectedChildren);
        }

        [Test]
        public void SimpleBranchingTest()
        {
            var graph = BuildCFG(@"
var a;
input(a);
1: if a == 0
    goto 2;
a = 2;
2: a = 3;
");
            var blocks = graph.GetCurrentBasicBlocks();

            var expectedDoms = new DominatorDictionary
            {
                [0] = new[] { blocks[0] },
                [1] = new[] { blocks[0], blocks[1] },
                [2] = new[] { blocks[0], blocks[1], blocks[2] },
                [3] = new[] { blocks[0], blocks[1], blocks[3] },
                [4] = new[] { blocks[0], blocks[4], blocks[1], blocks[3] }
            };
            var expectedParents = new ParentsDictionary
            {
                [0] = null,
                [1] = blocks[0],
                [2] = blocks[1],
                [3] = blocks[1],
                [4] = blocks[3]
            };
            var expectedChildren = new ChildrenDictionary
            {
                [0] = new[] { blocks[1] },
                [1] = new[] { blocks[2], blocks[3] },
                [2] = Enumerable.Empty<BasicBlock>(),
                [3] = new[] { blocks[4] },
                [4] = Enumerable.Empty<BasicBlock>()
            };

            TestInternal(graph, expectedDoms, expectedParents, expectedChildren);
        }

        [Test]
        public void DoubleBranchingTest()
        {
            var graph = BuildCFG(@"
var a;
input(a);
1: if a == 0
    goto 2;
if a == 1
    goto 2;
a = 2;
2: a = 3;
");
            var blocks = graph.GetCurrentBasicBlocks();

            var expectedDoms = new DominatorDictionary
            {
                [0] = new[] { blocks[0] },
                [1] = new[] { blocks[0], blocks[1] },
                [2] = new[] { blocks[0], blocks[1], blocks[2] },
                [3] = new[] { blocks[0], blocks[1], blocks[3] },
                [4] = new[] { blocks[0], blocks[1], blocks[3], blocks[4] },
                [5] = new[] { blocks[0], blocks[1], blocks[5] },
                [6] = new[] { blocks[0], blocks[1], blocks[5], blocks[6] }
            };
            var expectedParents = new ParentsDictionary
            {
                [0] = null,
                [1] = blocks[0],
                [2] = blocks[1],
                [3] = blocks[1],
                [4] = blocks[3],
                [5] = blocks[1],
                [6] = blocks[5]
            };
            var expectedChildren = new ChildrenDictionary
            {
                [0] = new[] { blocks[1] },
                [1] = new[] { blocks[2], blocks[3], blocks[5] },
                [2] = Enumerable.Empty<BasicBlock>(),
                [3] = new[] { blocks[4] },
                [4] = Enumerable.Empty<BasicBlock>(),
                [5] = new[] { blocks[6] },
                [6] = Enumerable.Empty<BasicBlock>()
            };

            TestInternal(graph, expectedDoms, expectedParents, expectedChildren);
        }

        [Test]
        public void BranchingAtTheEndTest()
        {
            var graph = BuildCFG(@"
var a, b, c;
input(a);
b = a * a;
if b == 25
    c = 0;
else
    c = 1;
");
            var blocks = graph.GetCurrentBasicBlocks();

            var expectedDoms = new DominatorDictionary
            {
                [0] = new[] { blocks[0] },
                [1] = new[] { blocks[0], blocks[1] },
                [2] = new[] { blocks[0], blocks[1], blocks[2] },
                [3] = new[] { blocks[0], blocks[1], blocks[3] },
                [4] = new[] { blocks[0], blocks[1], blocks[4] },
                [5] = new[] { blocks[0], blocks[1], blocks[4], blocks[5] }
            };
            var expectedParents = new ParentsDictionary
            {
                [0] = null,
                [1] = blocks[0],
                [2] = blocks[1],
                [3] = blocks[1],
                [4] = blocks[1],
                [5] = blocks[4]
            };
            var expectedChildren = new ChildrenDictionary
            {
                [0] = new[] { blocks[1] },
                [1] = new[] { blocks[2], blocks[3], blocks[4] },
                [2] = Enumerable.Empty<BasicBlock>(),
                [3] = Enumerable.Empty<BasicBlock>(),
                [4] = new[] { blocks[5] },
                [5] = Enumerable.Empty<BasicBlock>()
            };

            TestInternal(graph, expectedDoms, expectedParents, expectedChildren);
        }

        [Test]
        public void SimpleLoopTest()
        {
            var graph = BuildCFG(@"
var a, b, i;
input(a);
for i = 1, 10
{
    b = b + a * a;
    a = a + 1;
}
");
            var blocks = graph.GetCurrentBasicBlocks();

            var expectedDoms = new DominatorDictionary
            {
                [0] = new[] { blocks[0] },
                [1] = new[] { blocks[0], blocks[1] },
                [2] = new[] { blocks[0], blocks[1], blocks[2] },
                [3] = new[] { blocks[0], blocks[1], blocks[2], blocks[3] },
                [4] = new[] { blocks[0], blocks[1], blocks[2], blocks[4] },
                [5] = new[] { blocks[0], blocks[1], blocks[2], blocks[4], blocks[5] },
            };
            var expectedParents = new ParentsDictionary
            {
                [0] = null,
                [1] = blocks[0],
                [2] = blocks[1],
                [3] = blocks[2],
                [4] = blocks[2],
                [5] = blocks[4],
            };
            var expectedChildren = new ChildrenDictionary
            {
                [0] = new[] { blocks[1] },
                [1] = new[] { blocks[2] },
                [2] = new[] { blocks[3], blocks[4] },
                [3] = Enumerable.Empty<BasicBlock>(),
                [4] = new[] { blocks[5] },
                [5] = Enumerable.Empty<BasicBlock>()
            };

            TestInternal(graph, expectedDoms, expectedParents, expectedChildren);
        }
    }
}
