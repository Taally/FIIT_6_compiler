using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.LoopsInCFG
{
    [TestFixture]
    internal class BackEdgesTests : OptimizationsTestBase
    {
        [Test]
        public void EmptyGraph()
        {
            var graph = BuildTACOptimizeCFG(@"
var a;
");
            Assert.AreEqual(0, graph.GetBackEdges().Count);
        }

        [Test]
        public void SimpleTestBackEdges()
        {
            var graph = BuildTACOptimizeCFG(@"
var a, b;
2: a = 1;
goto 1;
1: b = 2;
goto 2;
");
            Assert.AreEqual(1, graph.GetBackEdges().Count);
        }

        [Test]
        public void SimpleNoBackEdgesWithoutGoTo()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, c, d, f;
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
            Assert.AreEqual(0, graph.GetBackEdges().Count);
        }

        [Test]
        public void BackEdgesWithGoTo()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, c, d, e, f;
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
            Assert.AreEqual(1, graph.GetBackEdges().Count);
        }

        [Test]
        public void SimpleNoBackEdgesWithGoTo()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, c, d, e, f;
1: a = 1;
b = 2;
goto 2;
2: c = 3;
d = 4;
goto 3;
3: e = 5;
goto 4;
4: f = 6;");

            Assert.AreEqual(0, graph.GetBackEdges().Count);
        }

        [Test]
        public void TrashProgram()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, c, d, e, y;
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
            Assert.AreEqual(1, graph.GetBackEdges().Count);
        }

        [Test]
        public void ProgramWithLoop()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, i, j, p, x;
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
            Assert.AreEqual(2, graph.GetBackEdges().Count);
        }

        // Тесты на приводимость / неприводимость графа потока управления (cfg)
        [Test]
        public void CheckIfGraphIsReducible1()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, i, j, p, x;
input(a);
1: for i = 1, 5
{
    b = b + a * a;
    a = a + 1;
}
goto 1;
2: for j = 1, 5
{
    p = b + a * a;
    x = a + 1;
}
goto 2;
");
            Assert.AreEqual(true, graph.IsReducibleGraph());
        }

        [Test]
        public void CheckIfGraphIsReducible2()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, c, d, i, j, p, x;
a = b + 2;
if a < b
{
    c = d + a;
}
else 
{
    a = d + c;
}
for i = 1, 5
{
    b = b + a * a;
    a = a + 1;
}
while (c < b)
{
    a = a + 1;
    b = b + 1;
}
while (a < b)
{
    x = a + b;
    goto 2;
}
2: for i = 1, 5
{
    d = d + b * a;
    b = b + 1;
goto 3;
while (a < b)
{
    x = a + b;
    goto 2;
}
3: for i = 1, 5
{
    d = d + b * a;
    b = b + 1;
}
}");
            Assert.AreEqual(true, graph.IsReducibleGraph());
        }

        [Test]
        public void CheckIfGraphIsNotReducible1()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, c, d, x, u, e,g, y,zz,i; 
if a > b
{
    c = d;
    1:  x = e;
    goto 2;
}
else
{
    e = g;
    2: a = d;
    goto 1;
}");
            Assert.AreEqual(false, graph.IsReducibleGraph());
        }

        [Test]
        public void CheckIfGraphIsNotReducible2()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, c, d, x, u, e,g, y,zz,i; 
for i = 1, 10
{
    a = b;
    1: c = d;
    goto 2;
}
2: x = u;
goto 1;");
            Assert.AreEqual(false, graph.IsReducibleGraph());
        }

        [Test]
        public void CheckIfGraphIsNotReducible3()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, c, d, x, u, e,g, y,zz,i; 
while (zz == i) 
{    
    a = b;
    1: c = d;
    goto 2;
}
2: x = u;
goto 1;
");
            Assert.AreEqual(false, graph.IsReducibleGraph());
        }
    }
}
