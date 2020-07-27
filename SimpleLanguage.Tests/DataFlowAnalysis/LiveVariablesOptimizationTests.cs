using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class LiveVariablesOptimizationTests : OptimizationsTestBase
    {
        [TestCase(@"
var a,b,c;
input (b);
a = b + 1;
c = 6;
if a < b
    c = b - a;
else
    c = b + a;
print (c);
",
            ExpectedResult = new[]
            {
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
            },
            TestName = "Simple")]

        [TestCase(@"
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
",
            ExpectedResult = new[]
            {
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
            },
            TestName = "Complex")]

        public IEnumerable<string> TestLiveVariablesOptimization(string sourceCode)
        {
            var cfg = GenCFG(sourceCode);
            LiveVariablesOptimization.DeleteDeadCode(cfg);
            return cfg.GetInstructions().Select(x => x.ToString());
        }
    }
}
