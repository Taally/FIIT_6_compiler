
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    public class OptimalSpeedIterativeAlgorithmTests : OptimizationsTestBase
    {
        [Test]
        public void AvailableExpressionsTest()
        {
            var program = @"
var a, b, c, d, x, u, e,g, y,zz,i; 
2: a = x + y;
g = c + d;
3: zz = 1;
goto 1;
1: if(a < b) 
    c = 1; 
b = c + d;
goto 3;
e = zz + i;";
            var graph = GenCFG(program);
            var algorithm = new AvailableExpressions();

            algorithm.Execute(graph);
            var iterationsFast = algorithm.Iterations;
            algorithm.Execute(graph, false);
            var iterationsSlow = algorithm.Iterations;

            Assert.LessOrEqual(iterationsFast, iterationsSlow);
        }

        [Test]
        public void ConstPropagationTest()
        {
            var program = @"
var a, x, c;
input(c);
if c > 5
    x = 10;
else
    input(c);
if c > 5
    x = 20;
a = x;";
            var graph = GenCFG(program);
            var algorithm = new ConstPropagation();

            algorithm.Execute(graph);
            var iterationsFast = algorithm.Iterations;
            algorithm.Execute(graph, false);
            var iterationsSlow = algorithm.Iterations;

            Assert.LessOrEqual(iterationsFast, iterationsSlow);
        }

        [Test]
        public void LiveVariableTest()
        {
            var program = @"
var a,b,c,i;

for i = 1,b {
	input (a);
	c = c + a;
	print(c);
	if c < b
		c = c + 1;
	else {
		b = b - 1;
		print(b);
		print(c);
	}
}

print (c+a+b);";
            var graph = GenCFG(program);
            var algorithm = new LiveVariables();

            algorithm.Execute(graph);
            var iterationsFast = algorithm.Iterations;
            algorithm.Execute(graph, false);
            var iterationsSlow = algorithm.Iterations;

            Assert.LessOrEqual(iterationsFast, iterationsSlow);
        }

        [Test]
        public void ReachingDefinitionsTest()
        {
            var program = @"
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
}";
            var graph = GenCFG(program);
            var algorithm = new LiveVariables();

            algorithm.Execute(graph);
            var iterationsFast = algorithm.Iterations;
            algorithm.Execute(graph, false);
            var iterationsSlow = algorithm.Iterations;

            Assert.LessOrEqual(iterationsFast, iterationsSlow);
        }

        [Test]
        public void DominatorTreeTest()
        {
            var program = @"
var a;
input(a);
1: if a == 0
    goto 2;
if a == 1
    goto 2;
a = 2;
2: a = 3;
";
            var graph = GenCFG(program);
            var algorithm = new DominatorTree();

            algorithm.Execute(graph);
            var iterationsFast = algorithm.Iterations;
            algorithm.Execute(graph, false);
            var iterationsSlow = algorithm.Iterations;

            Assert.LessOrEqual(iterationsFast, iterationsSlow);
        }
    }
}
