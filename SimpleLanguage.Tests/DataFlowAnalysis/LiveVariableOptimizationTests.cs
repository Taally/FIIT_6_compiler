using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class LiveVariableOptimizationTests : OptimizationsTestBase
    {
        [Test]
        public void SimpleTest()
        {
            var program = @"
var a,b,c;
input (b);
a = b + 1;
c = 6;
if a < b
    c = b - a;
else
    c = b + a;
print (c);
";
            var cfg = GenCFG(program);
            LiveVariableAnalysisOptimization.DeleteDeadCode(cfg);

            var actual = cfg.GetCurrentBasicBlocks().SelectMany(z => z.GetInstructions().Select(t => t.ToString()));
            var expected = new List<string>()
            {
                "#in: noop",
                "input b",
                "#t1 = b + 1",
                "a = #t1",
                "noop",
                "#t2 = a < b",
                "if #t2 goto L1",
                "#t3 = b + a",
                "c = #t3",
                "goto L2",
                "L1: #t4 = b - a",
                "c = #t4",
                "L2: noop",
                "print c",
                "#out: noop",
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ComplexTest()
        {
            var program = @"
var a, b, c, d;
input(a);
b = a * 2;
c = 123456789;
goto 2;

2: a = 128;
c = b * a + 5;
goto 3;

3: d = a;
c = b * b;
a = 111111111;
goto 4;

4: a = 0;
";
            var cfg = GenCFG(program);
            LiveVariableAnalysisOptimization.DeleteDeadCode(cfg);

            var actual = cfg.GetCurrentBasicBlocks().SelectMany(z => z.GetInstructions().Select(t => t.ToString()));
            var expected = new List<string>()
            {
                "#in: noop",
                "input a",
                "#t1 = a * 2",
                "b = #t1",
                "noop",
                "goto 2",
                "2: a = 128",
                "noop",
                "noop",
                "noop",
                "goto 3",
                "3: noop",
                "noop",
                "noop",
                "noop",
                "goto 4",
                "4: noop",
                "#out: noop",
            };

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
