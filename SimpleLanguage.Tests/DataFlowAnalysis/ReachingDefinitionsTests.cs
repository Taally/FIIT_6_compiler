using NUnit.Framework;
using SimpleLang;
using System.Collections.Generic;
using System.Linq;
using static SimpleLang.ReachingDefinitions;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    class ReachingDefinitionsTests : TACTestsBase
    {
        private (List<BasicBlock> basicBlocks, InOutInfo inOutInfo) GenGraphAndGetInOutInfo(string program)
        {
            var TAC = GenTAC(program);
            var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
            var cfg = new ControlFlowGraph(blocks);
            var inOutInfo = new ReachingDefinitions().Execute(cfg);
            return (blocks, inOutInfo);
        }

        [Test]
        public void EmptyProgram()
        {
            (_, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
");
            // no basic blocks
            Assert.AreEqual(0, inOutInfo.In.Count);
            Assert.AreEqual(0, inOutInfo.Out.Count);
        }

        [Test]
        public void BasicTest()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
a = 1;
");
            // only one basic block
            Assert.AreEqual(1, inOutInfo.In.Count);
            Assert.AreEqual(1, inOutInfo.Out.Count);

            Assert.AreEqual(0, inOutInfo.In[blocks[0]].Count());
            Assert.AreEqual(1, inOutInfo.Out[blocks[0]].Count());
            Assert.AreEqual(blocks[0].GetInstructions(), inOutInfo.Out[blocks[0]]);
        }

        [Test]
        public void KillingDefinitionWithinBlock()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
a = 1;
a = 2;
");
            // only one basic block
            Assert.AreEqual(1, inOutInfo.In.Count);
            Assert.AreEqual(1, inOutInfo.Out.Count);

            Assert.AreEqual(0, inOutInfo.In[blocks[0]].Count());
            Assert.AreEqual(1, inOutInfo.Out[blocks[0]].Count());

            // only second instruction
            Assert.AreEqual(blocks[0].GetInstructions().Skip(1), inOutInfo.Out[blocks[0]]);
        }

        [Test]
        public void SeveralDefinitionsWithinBlock()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a, b, c;
b = 3;
a = 1;
a = 2;
b = a;
c = a;
c = b;
");
            // only one basic block
            Assert.AreEqual(1, inOutInfo.In.Count);
            Assert.AreEqual(1, inOutInfo.Out.Count);

            Assert.AreEqual(0, inOutInfo.In[blocks[0]].Count());
            Assert.AreEqual(3, inOutInfo.Out[blocks[0]].Count());

            var expected = blocks[0].GetInstructions()
                .Skip(2).Take(2)
                .Append(blocks[0].GetInstructions().Last());
            CollectionAssert.AreEquivalent(expected, inOutInfo.Out[blocks[0]]);
        }

        [Test]
        public void TwoBasicBlocksWithoutKilling()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a, b;
a = 1;
goto 1;
1: b = 2;
");
            Assert.AreEqual(2, inOutInfo.In.Count);
            Assert.AreEqual(2, inOutInfo.Out.Count);

            Assert.AreEqual(1, inOutInfo.In[blocks[1]].Count());
            Assert.AreEqual(blocks[0].GetInstructions().Take(1), inOutInfo.In[blocks[1]]);

            Assert.AreEqual(2, inOutInfo.Out[blocks[1]].Count());
            var expected = blocks[0].GetInstructions().Take(1).Concat(blocks[1].GetInstructions());
            CollectionAssert.AreEquivalent(expected, inOutInfo.Out[blocks[1]]);
        }

        [Test]
        public void KillingDefinitionBetweenBlocks()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
a = 1;
goto 1;
1: a = 2;
");
            Assert.AreEqual(2, inOutInfo.In.Count);
            Assert.AreEqual(2, inOutInfo.Out.Count);

            Assert.AreEqual(1, inOutInfo.In[blocks[1]].Count());
            Assert.AreEqual(blocks[0].GetInstructions().Take(1), inOutInfo.In[blocks[1]]);
            Assert.AreEqual(1, inOutInfo.Out[blocks[1]].Count());
            Assert.AreEqual(blocks[1].GetInstructions(), inOutInfo.Out[blocks[1]]);
        }

        [Test]
        public void BranchingCode()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a, b;
input(a);
if a > 0
	a = 0;
else
	a = 1;
b = a;
");
            Assert.AreEqual(4, inOutInfo.In.Count);
            Assert.AreEqual(4, inOutInfo.Out.Count);

            var falseBranch = blocks[1].GetInstructions().Take(1); // a = 1;
            var trueBranch = blocks[2].GetInstructions().Take(1); // a = 0;
            var lastBlock = blocks[3].GetInstructions().Skip(1); // b = a;
            CollectionAssert.AreEquivalent(falseBranch.Concat(trueBranch), inOutInfo.In[blocks[3]]);
            CollectionAssert.AreEquivalent(falseBranch.Concat(trueBranch).Concat(lastBlock), inOutInfo.Out[blocks[3]]);
        }

        [Test]
        public void KillingDefinitionInOneBranch()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a, b;
input(a);
if a > 0
	b = 0;
else
	a = 1;
b = a;
");
            Assert.AreEqual(4, inOutInfo.In.Count);
            Assert.AreEqual(4, inOutInfo.Out.Count);

            var initialDef = blocks[0].GetInstructions().Take(1); // input(a);
            var falseBranch = blocks[1].GetInstructions().Take(1); // a = 1;
            var trueBranch = blocks[2].GetInstructions().Take(1); // b = 0;
            var lastBlock = blocks[3].GetInstructions().Skip(1); // b = a;
            CollectionAssert.AreEquivalent(initialDef.Concat(falseBranch).Concat(trueBranch), inOutInfo.In[blocks[3]]);
            CollectionAssert.AreEquivalent(initialDef.Concat(falseBranch).Concat(lastBlock), inOutInfo.Out[blocks[3]]);
        }

        [Test]
        public void ForCycle()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var i, k;
for k = 0, 2
	i = i + 1;
");
            Assert.AreEqual(5, inOutInfo.In.Count);
            Assert.AreEqual(5, inOutInfo.Out.Count);

            // 2nd block
            var expectedIn = new List<Instruction>()
            {
                blocks[0].GetInstructions()[0], // k = 0;
                blocks[3].GetInstructions()[1], // i = #t2
                blocks[3].GetInstructions()[2], // k = k + 1
            };
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo.In[blocks[1]]);
            CollectionAssert.AreEquivalent(inOutInfo.In[blocks[1]], inOutInfo.Out[blocks[1]]);

            // 4th block with k = k + 1
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo.In[blocks[3]]);
            var expectedOut = blocks[3].GetInstructions().Skip(1).Take(2); // i = #t2; k = k + 1;
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo.Out[blocks[3]]);
        }

        [Test]
        public void DisconnectedGraph()
        {
            // supposedly in the future unreachable code will be removed but for now test that definitions don't reach it
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
1: a = 1;
goto 1;
a = 4;
");
            Assert.AreEqual(2, inOutInfo.In.Count);
            Assert.AreEqual(2, inOutInfo.Out.Count);

            CollectionAssert.AreEquivalent(blocks[0].GetInstructions().Take(1), inOutInfo.Out[blocks[0]]);

            Assert.AreEqual(0, inOutInfo.In[blocks[1]].Count());
            CollectionAssert.AreEquivalent(blocks[1].GetInstructions(), inOutInfo.Out[blocks[1]]);
        }

        [Test]
        public void ComplexTest()
        {
            // Example from slide 4 in Reaching Definitions lecture
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var i, m, j, n, a, u1, u2, u3, k;
1: i = m - 1;
2: j = n;
3: a = u1;

for k = 0, 1
{
    i = i + 1;
    j = j - 1;

    if i < j
        a = u2;
    i = u3;
}
");

            Assert.AreEqual(8, inOutInfo.In.Count);
            Assert.AreEqual(8, inOutInfo.Out.Count);

            // 1st block
            /*
1: #t1 = m - 1
i = #t1
2: j = n
3: a = u1
k = 0
             */
            var expectedOut = blocks[0].GetInstructions().Skip(1);
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo.Out[blocks[0]]);

            // 2nd block
            /*
L1: #t2 = k < 1
if #t2 goto L2
             */
            var expectedIn = inOutInfo.Out[blocks[0]].Union(inOutInfo.Out[blocks[6]]);
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo.In[blocks[1]]);
            CollectionAssert.AreEquivalent(inOutInfo.In[blocks[1]], inOutInfo.Out[blocks[1]]);

            // 3rd block
            /*
goto L3
             */
            CollectionAssert.AreEquivalent(inOutInfo.Out[blocks[1]], inOutInfo.In[blocks[2]]);
            CollectionAssert.AreEquivalent(inOutInfo.In[blocks[2]], inOutInfo.Out[blocks[2]]);

            // 4th block
            /*
L2: #t3 = i + 1
i = #t3
#t4 = j - 1
j = #t4
#t5 = i < j
if #t5 goto L4
             */
            expectedIn = inOutInfo.Out[blocks[1]].Union(inOutInfo.Out[blocks[2]]);
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo.In[blocks[3]]);
            expectedOut = new List<Instruction>()
            {
                blocks[3].GetInstructions()[1], // i = #t3
                blocks[3].GetInstructions()[3], // j = #t4
                blocks[0].GetInstructions()[3], // 3: a = u1
                blocks[0].GetInstructions()[4], // k = 0
                blocks[5].GetInstructions()[0], // L4: a = u2
                blocks[6].GetInstructions()[2], // k = k + 1
            };
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo.Out[blocks[3]]);

            // 5th block
            /*
goto L5
             */
            CollectionAssert.AreEquivalent(inOutInfo.Out[blocks[3]], inOutInfo.In[blocks[4]]);
            CollectionAssert.AreEquivalent(inOutInfo.In[blocks[4]], inOutInfo.Out[blocks[4]]);

            // 6th block
            /*
L4: a = u2
             */
            CollectionAssert.AreEquivalent(inOutInfo.Out[blocks[3]], inOutInfo.In[blocks[5]]);
            expectedOut = new List<Instruction>()
            {
                blocks[3].GetInstructions()[1], // i = #t3
                blocks[3].GetInstructions()[3], // j = #t4
                blocks[0].GetInstructions()[4], // k = 0
                blocks[5].GetInstructions()[0], // L4: a = u2
                blocks[6].GetInstructions()[2], // k = k + 1

            };
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo.Out[blocks[5]]);

            // 7th block
            /*
L5: noop
i = u3
k = k + 1
goto L1
             */
            expectedIn = inOutInfo.Out[blocks[4]].Union(inOutInfo.Out[blocks[5]]);
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo.In[blocks[6]]);
            expectedOut = new List<Instruction>()
            {
                blocks[6].GetInstructions()[1], // i = u3
                blocks[6].GetInstructions()[2], // k = k + 1
                blocks[3].GetInstructions()[3], // j = #t4
                blocks[0].GetInstructions()[3], // 3: a = u1
                blocks[5].GetInstructions()[0], // L4: a = u2

            };
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo.Out[blocks[6]]);

            // 8th block
            /*
L3: noop
             */
            CollectionAssert.AreEquivalent(inOutInfo.Out[blocks[2]], inOutInfo.In[blocks[7]]);
            CollectionAssert.AreEquivalent(inOutInfo.In[blocks[7]], inOutInfo.Out[blocks[7]]);
        }
    }
}
