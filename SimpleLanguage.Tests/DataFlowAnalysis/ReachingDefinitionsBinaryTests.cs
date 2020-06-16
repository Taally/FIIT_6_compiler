using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class ReachingDefinitionsBinaryTests : TACTestsBase
    {
        private (List<BasicBlock> basicBlocks, InOutData<IEnumerable<Instruction>> inOutInfo) GenGraphAndGetInOutInfo(string program)
        {
            var TAC = GenTAC(program);
            var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
            var cfg = new ControlFlowGraph(blocks);
            var inOutInfo = new ReachingDefinitionBinary().Execute(cfg);
            return (blocks, inOutInfo);
        }

        [Test]
        public void EmptyProgram()
        {
            (_, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
");
            // no basic blocks + entry and exit
            Assert.AreEqual(2, inOutInfo.Count);
        }

        [Test]
        public void BasicTest()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
a = 1;
");
            // only one basic block + entry and exit
            Assert.AreEqual(3, inOutInfo.Count);

            Assert.AreEqual(0, inOutInfo[blocks[0]].In.Count());
            Assert.AreEqual(1, inOutInfo[blocks[0]].Out.Count());
            Assert.AreEqual(blocks[0].GetInstructions(), inOutInfo[blocks[0]].Out);
        }

        [Test]
        public void KillingDefinitionWithinBlock()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
a = 1;
a = 2;
");
            // only one basic block + entry and exit
            Assert.AreEqual(3, inOutInfo.Count);

            Assert.AreEqual(0, inOutInfo[blocks[0]].In.Count());
            Assert.AreEqual(1, inOutInfo[blocks[0]].Out.Count());

            // only second instruction
            Assert.AreEqual(blocks[0].GetInstructions().Skip(1), inOutInfo[blocks[0]].Out);
        }

        [Test]
        public void InputTest()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var a;
a = 1;
input(a);
");
            // only one basic block + entry and exit
            Assert.AreEqual(3, inOutInfo.Count);

            Assert.AreEqual(0, inOutInfo[blocks[0]].In.Count());
            Assert.AreEqual(1, inOutInfo[blocks[0]].Out.Count());

            // only second instruction
            Assert.AreEqual(blocks[0].GetInstructions().Skip(1), inOutInfo[blocks[0]].Out);
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
            // only one basic block + entry and exit
            Assert.AreEqual(3, inOutInfo.Count);

            Assert.AreEqual(0, inOutInfo[blocks[0]].In.Count());
            Assert.AreEqual(3, inOutInfo[blocks[0]].Out.Count());

            var expected = blocks[0].GetInstructions()
                .Skip(2).Take(2)
                .Append(blocks[0].GetInstructions().Last());
            CollectionAssert.AreEquivalent(expected, inOutInfo[blocks[0]].Out);
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
            // two basic blocks + entry and exit
            Assert.AreEqual(4, inOutInfo.Count);

            Assert.AreEqual(1, inOutInfo[blocks[1]].In.Count());
            Assert.AreEqual(blocks[0].GetInstructions().Take(1), inOutInfo[blocks[1]].In);

            Assert.AreEqual(2, inOutInfo[blocks[1]].Out.Count());
            var expected = blocks[0].GetInstructions().Take(1).Concat(blocks[1].GetInstructions());
            CollectionAssert.AreEquivalent(expected, inOutInfo[blocks[1]].Out);
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
            // two basic blocks + entry and exit
            Assert.AreEqual(4, inOutInfo.Count);

            Assert.AreEqual(1, inOutInfo[blocks[1]].In.Count());
            Assert.AreEqual(blocks[0].GetInstructions().Take(1), inOutInfo[blocks[1]].In);
            Assert.AreEqual(1, inOutInfo[blocks[1]].Out.Count());
            Assert.AreEqual(blocks[1].GetInstructions(), inOutInfo[blocks[1]].Out);
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
            // four basic blocks + entry and exit
            Assert.AreEqual(6, inOutInfo.Count);

            var falseBranch = blocks[1].GetInstructions().Take(1); // a = 1;
            var trueBranch = blocks[2].GetInstructions().Take(1); // a = 0;
            var lastBlock = blocks[3].GetInstructions().Skip(1); // b = a;
            CollectionAssert.AreEquivalent(falseBranch.Concat(trueBranch), inOutInfo[blocks[3]].In);
            CollectionAssert.AreEquivalent(falseBranch.Concat(trueBranch).Concat(lastBlock), inOutInfo[blocks[3]].Out);
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
            // four basic blocks + entry and exit
            Assert.AreEqual(6, inOutInfo.Count);

            var initialDef = blocks[0].GetInstructions().Take(1); // input(a);
            var falseBranch = blocks[1].GetInstructions().Take(1); // a = 1;
            var trueBranch = blocks[2].GetInstructions().Take(1); // b = 0;
            var lastBlock = blocks[3].GetInstructions().Skip(1); // b = a;
            CollectionAssert.AreEquivalent(initialDef.Concat(falseBranch).Concat(trueBranch), inOutInfo[blocks[3]].In);
            CollectionAssert.AreEquivalent(initialDef.Concat(falseBranch).Concat(lastBlock), inOutInfo[blocks[3]].Out);
        }

        [Test]
        public void ForLoop()
        {
            (var blocks, var inOutInfo) = GenGraphAndGetInOutInfo(@"
var i, k;
for k = 0, 2
    i = i + 1;
");
            // four basic blocks + entry and exit
            Assert.AreEqual(6, inOutInfo.Count);

            // 1st block is a single k = 0

            // 2nd block
            /*
            L1: #t1 = k >= 2
            if #t1 goto L2
             */
            var expectedIn = new List<Instruction>()
            {
                blocks[0].GetInstructions()[0], // k = 0;
                blocks[2].GetInstructions()[1], // i = #t2
                blocks[2].GetInstructions()[2], // k = k + 1
            };
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo[blocks[1]].In);
            CollectionAssert.AreEquivalent(inOutInfo[blocks[1]].In, inOutInfo[blocks[1]].Out);

            // 3rd block
            /*
            #t2 = i + 1
            i = #t2
            k = k + 1
            goto L1
             */
            var expectedOut = new List<Instruction>()
            {
                blocks[2].GetInstructions()[1], // i = #t2
                blocks[2].GetInstructions()[2], // k = k + 1
            };
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo[blocks[2]].In);
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo[blocks[2]].Out);

            // 4th block
            /*
            L2: noop
             */
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo[blocks[3]].In);
            CollectionAssert.AreEquivalent(inOutInfo[blocks[3]].In, inOutInfo[blocks[3]].Out);
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
            // two basic blocks + entry and exit
            Assert.AreEqual(4, inOutInfo.Count);

            CollectionAssert.AreEquivalent(blocks[0].GetInstructions().Take(1), inOutInfo[blocks[0]].Out);

            Assert.AreEqual(0, inOutInfo[blocks[1]].In.Count());
            CollectionAssert.AreEquivalent(blocks[1].GetInstructions(), inOutInfo[blocks[1]].Out);
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
            // seven basic blocks + entry and exit
            Assert.AreEqual(9, inOutInfo.Count);

            // 1st block
            /*
1: #t1 = m - 1
i = #t1
2: j = n
3: a = u1
k = 0
             */
            var expectedOut = blocks[0].GetInstructions().Skip(1);
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo[blocks[0]].Out);

            // 2nd block
            /*
L1: #t2 = k >= 1
if #t2 goto L2
             */
            var expectedIn = inOutInfo[blocks[0]].Out.Union(inOutInfo[blocks[6]].Out);
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo[blocks[1]].In);
            CollectionAssert.AreEquivalent(inOutInfo[blocks[1]].In, inOutInfo[blocks[1]].Out);

            // 3rd block
            /*
#t3 = i + 1
i = #t3
#t4 = j - 1
j = #t4
#t5 = i < j
if #t5 goto L3
             */
            expectedIn = inOutInfo[blocks[1]].Out;
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo[blocks[2]].In);
            expectedOut = new List<Instruction>()
            {
                blocks[2].GetInstructions()[1], // i = #t3
                blocks[2].GetInstructions()[3], // j = #t4
                blocks[0].GetInstructions()[3], // 3: a = u1
                blocks[0].GetInstructions()[4], // k = 0
                blocks[4].GetInstructions()[0], // L3: a = u2
                blocks[5].GetInstructions()[2], // k = k + 1
            };
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo[blocks[2]].Out);

            // 4th block
            /*
goto L4
             */
            CollectionAssert.AreEquivalent(inOutInfo[blocks[2]].Out, inOutInfo[blocks[3]].In);
            CollectionAssert.AreEquivalent(inOutInfo[blocks[3]].In, inOutInfo[blocks[3]].Out);

            // 5th block
            /*
L3: a = u2
             */
            CollectionAssert.AreEquivalent(inOutInfo[blocks[2]].Out, inOutInfo[blocks[4]].In);
            expectedOut = new List<Instruction>()
            {
                blocks[2].GetInstructions()[1], // i = #t3
                blocks[2].GetInstructions()[3], // j = #t4
                blocks[0].GetInstructions()[4], // k = 0
                blocks[4].GetInstructions()[0], // L3: a = u2
                blocks[5].GetInstructions()[2], // k = k + 1
            };
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo[blocks[4]].Out);

            // 6th block
            /*
L4: noop
i = u3
k = k + 1
goto L1
             */
            expectedIn = inOutInfo[blocks[3]].Out.Union(inOutInfo[blocks[4]].Out);
            CollectionAssert.AreEquivalent(expectedIn, inOutInfo[blocks[5]].In);
            expectedOut = new List<Instruction>()
            {
                blocks[5].GetInstructions()[1], // i = u3
                blocks[5].GetInstructions()[2], // k = k + 1
                blocks[2].GetInstructions()[3], // j = #t4
                blocks[0].GetInstructions()[3], // 3: a = u1
                blocks[4].GetInstructions()[0], // L3: a = u2
            };
            CollectionAssert.AreEquivalent(expectedOut, inOutInfo[blocks[5]].Out);

            // 7th block
            /*
L2: noop
             */
            CollectionAssert.AreEquivalent(inOutInfo[blocks[1]].Out, inOutInfo[blocks[6]].In);
            CollectionAssert.AreEquivalent(inOutInfo[blocks[6]].In, inOutInfo[blocks[6]].Out);
        }
    }
}
