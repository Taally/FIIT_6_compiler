using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.LoopsInCFG
{
    [TestFixture]
    internal class RegionTests : OptimizationsTestBase
    {
        [Test]
        public void WithoutCycles()
        {
            var TAC = GenTAC(@"
var a, b;
a = 5;
if b != 2
{
    a = 6;
}
a = 8;
");
            var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
            var cfg = new ControlFlowGraph(blocks);
            var result = new CFGregions(cfg);
            result.Regions.Last().Print();
            var actual = result.Regions.Select(x => (x.edges?.Count ?? 0, x.includedRegions?.Count ?? 0)).ToArray();
            var expected = new[]{
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (6, 6),
            };
            Assert.AreEqual(7, result.Regions.Count);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void OneCycle()
        {
            var TAC = GenTAC(@"
var a, b, x, c;
for x=1,10
{
    a = 2;
}
c = a + b;
");
            var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
            var cfg = new ControlFlowGraph(blocks);
            var result = new CFGregions(cfg);
            result.Regions.Last().Print();
            var actual = result.Regions.Select(x => (x.edges?.Count ?? 0, x.includedRegions?.Count ?? 0)).ToArray();
            var expected = new[]{
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (1, 1),
                (1, 2),
                (4, 5)
            };
            Assert.AreEqual(9, result.Regions.Count);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void TwoCycles()
        {
            var TAC = GenTAC(@"
var a, b, x, c;
for x=1,10
{
    a = 2;
}
for x=1,10
{
    b = 55;
}
c = a + b;
");
            var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
            var cfg = new ControlFlowGraph(blocks);
            var result = new CFGregions(cfg);
            result.Regions.Last().Print();
            var actual = result.Regions.Select(x => (x.edges?.Count ?? 0, x.includedRegions?.Count ?? 0)).ToArray();
            var expected = new[]{
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (1, 1),
                (1, 2),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (1, 1),
                (1, 2),
                (6, 7)
            };
            Assert.AreEqual(14, result.Regions.Count);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void TwoNestedCycles()
        {
            var TAC = GenTAC(@"
var a, b, c, x;
for x=1,10
{
    for a=1,10
    {
        c = 2;
    }
    for b = 1,10
    {
        c = 4;        
    }
}
");
            var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
            var cfg = new ControlFlowGraph(blocks);
            var loops = NaturalLoop.GetAllNaturalLoops(cfg);
            Assert.AreEqual(3, loops.Count);
            var result = new CFGregions(cfg);
            result.Regions.Last().Print();
            var actual = result.Regions.Select(x => (x.edges?.Count ?? 0, x.includedRegions?.Count ?? 0)).ToArray();
            var expected = new[]{
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (0, 0),
                (1, 2),
                (1, 1),
                (1, 2),
                (1, 1),
                (5, 6),
                (1, 1),
                (4, 5)
            };
            Assert.AreEqual(19, result.Regions.Count);
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
