using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.LoopsInCFG
{
    [TestFixture]
    internal class BackEdgesTest : TACTestsBase
    {
        private ControlFlowGraph BuildCFG(string program)
        {
            var TAC = GenTAC(program);
            var optResult = ThreeAddressCodeOptimizer.OptimizeAll(TAC);
            var blocks = BasicBlockLeader.DivideLeaderToLeader(optResult);
            return new ControlFlowGraph(blocks);
        }
        [Test]
        public void EmptyGraph()
        {
            var graph = BuildCFG(@"
var a;
");
            var backEdges = new BackEdges(graph).BackEdgesFromGraph;
            Assert.AreEqual(0, backEdges.Count);
        }
        [Test]
        public void SimpleTestBackEdges()
        {
            var graph = BuildCFG(@"
var a, b;
2: a = 1;
goto 1;
1: b = 2;
goto 2;
");
            var backEdges = new BackEdges(graph).BackEdgesFromGraph;
            Assert.AreEqual(1, backEdges.Count);
        }
        [Test]
        public void SimpleNoBackEdgesWithoutGoTo()
        {
            var graph = BuildCFG(@"var a, b, c, d, f;
a = 1;
b = 5;
c = a + b;
d = 7;
f = 0;
if d > c
{
    a = 0;
    c = d + 1;
}
else
{
    f = 5;
}
a = f + c;");
            var backEdges = new BackEdges(graph).BackEdgesFromGraph;
            Assert.AreEqual(0, backEdges.Count);
        }
        [Test]
        public void BackEdgesWithGoTo()
        {
            var graph = BuildCFG(@"var a, b, c, d, e, f;
1: a = 1;
5: b = 2;
goto 2;
2: c = 3;
d = 4;
goto 3;
3: e = 5;
goto 4;
4: f = 6;
goto 5;");
            var backEdges = new BackEdges(graph).BackEdgesFromGraph;
            Assert.AreEqual(1, backEdges.Count);
        }
        [Test]
        public void SimpleNoBackEdgesWithGoTo()
        {
            var graph = BuildCFG(@"var a, b, c, d, e, f;
1: a = 1;
b = 2;
goto 2;
2: c = 3;
d = 4;
goto 3;
3: e = 5;
goto 4;
4: f = 6;");

            var backEdges = new BackEdges(graph).BackEdgesFromGraph;
            Assert.AreEqual(0, backEdges.Count);
        }
        [Test]
        public void TrashProgram()
        {
            var graph = BuildCFG(@"var a, b, c, d, e, y;
input(a);
b = 5 + a;
c = 6 + a;
5: y = 7;
1: if a == 4
    c = b;
2: if a == 5
    goto 5;
3: d = c + b;
goto 4;
4: if d > c
    goto 6;
6: e = a + d;");
            var backEdges = new BackEdges(graph).BackEdgesFromGraph;
            Assert.AreEqual(1, backEdges.Count);
        }
        [Test]
        public void ProgramWithLoop()
        {
            var graph = BuildCFG(@"var a, b, i, j, p, x;
input(a);
1: for i = 1, 5
{
    b = b + a * a;
    a = a + 1;
}
2: for j = 1, 5
{
    p = b + a * a;
    x = a + 1;
}");
            var backEdges = new BackEdges(graph).BackEdgesFromGraph;
            Assert.AreEqual(2, backEdges.Count);
        }
    }
}
