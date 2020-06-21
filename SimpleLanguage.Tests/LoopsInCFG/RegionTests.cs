using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.LoopsInCFG
{
    [TestFixture]
    internal class RegionTests : TACTestsBase
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
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            Assert.AreEqual(0, actual.Count);
            var result = new CFGregions(cfg);
            Assert.AreEqual(7, result.Regions.Count);
            Assert.AreEqual(6, result.Regions.Last().edges.Count);
            Assert.AreEqual(6, result.Regions.Last().includedRegions.Count);
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
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            Assert.AreEqual(1, actual.Count);
            var result = new CFGregions(cfg);
            Assert.AreEqual(9, result.Regions.Count);
            Assert.AreEqual(4, result.Regions[result.Regions.Count - 1].edges.Count);
            Assert.AreEqual(5, result.Regions[result.Regions.Count - 1].includedRegions.Count);
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
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            Assert.AreEqual(2, actual.Count);
            var result = new CFGregions(cfg);
            Assert.AreEqual(14, result.Regions.Count);
            Assert.AreEqual(6, result.Regions[result.Regions.Count - 1].edges.Count);
            Assert.AreEqual(7, result.Regions[result.Regions.Count - 1].includedRegions.Count);
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
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            Assert.AreEqual(3, actual.Count);
            var result = new CFGregions(cfg);
            Assert.AreEqual(19, result.Regions.Count);
            Assert.AreEqual(4, result.Regions[result.Regions.Count - 1].edges.Count);
            Assert.AreEqual(5, result.Regions[result.Regions.Count - 1].includedRegions.Count);
        }
    }
}
