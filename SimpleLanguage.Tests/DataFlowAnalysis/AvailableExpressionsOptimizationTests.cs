using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;
using SimpleLang.DataFlowAnalysis;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class AvailableExpressionsOptimizationTests : OptimizationsTestBase
    {
        [TestCase(@"
var a;
",
            ExpectedResult = new string[0],
            TestName = "EmptyProgram")]

        [TestCase(@"
var a, b, c, d, x, e, g;
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
}
",
            ExpectedResult = new[]
            {
                "#t1 = a > b",
                "if #t1 goto L1",
                "e = g",
                "2: a = d",
                "goto 1",
                "L1: c = d",
                "1: x = e",
                "goto 2",
            },
            TestName = "NoOptimizationInNotReducibleGraph")] // hardest

        [TestCase(@"
var a, b, x, y, z, p, q;
x = y + z;
if (a < b)
{
    p = y + z;
}
q = y + z;
",
            ExpectedResult = new[]
            {
                "#t6 = y + z",
                "#t5 = #t6",
                "#t1 = #t5",
                "x = #t1",
                "#t2 = a >= b",
                "if #t2 goto L1",
                "#t3 = #t5",
                "p = #t3",
                "L1: noop",
                "#t4 = #t6",
                "q = #t4",
            },
            TestName = "SimpleProgram")]

        [TestCase(@"
var x, y, z, p, q;
x = y + z;
goto 1;
1: p = y + z;
goto 2;
2: q = y + z;
",
            ExpectedResult = new[]
            {
                "#t5 = y + z",
                "#t4 = #t5",
                "#t1 = #t4",
                "x = #t1",
                "goto 1",
                "1: #t2 = #t4",
                "p = #t2",
                "goto 2",
                "2: #t3 = #t5",
                "q = #t3",
            },
            TestName = "SimpleProgramWithGoto")]

        [TestCase(@"
var a, b, d, x, zz, i;
zz = i + x;
for i = 2, 7
{
    x = x + d;
    a = a + b;
}
",
            ExpectedResult = new[]
            {
                "#t1 = i + x",
                "zz = #t1",
                "i = 2",
                "L1: #t2 = i >= 7",
                "if #t2 goto L2",
                "#t3 = x + d",
                "x = #t3",
                "#t4 = a + b",
                "a = #t4",
                "i = i + 1",
                "goto L1",
                "L2: noop",
            },
            TestName = "ProgramWithLoopForNoOptimization1")]

        [TestCase(@"
var a, b, d, x, e, zz, i;
e = x + d;
zz = i + x;
for i = 2, 7
{
    x = x + d;
    a = a + b;
}
",
            ExpectedResult = new[]
            {
                "#t1 = x + d",
                "e = #t1",
                "#t2 = i + x",
                "zz = #t2",
                "i = 2",
                "L1: #t3 = i >= 7",
                "if #t3 goto L2",
                "#t4 = x + d",
                "x = #t4",
                "#t5 = a + b",
                "a = #t5",
                "i = i + 1",
                "goto L1",
                "L2: noop",
            },
            TestName = "ProgramWithLoopForNoOptimization2")]

        [TestCase(@"
var a, b, x, y, z, p, q;
x = y + z;
y = 1;
if (a < b)
{
    z = 2;
    p = y + z;
}
z = 1;
q = y + z;
",
            true,
            ExpectedResult = new[]
            {
                "#t1 = y + z",
                "x = #t1",
                "y = 1",
                "#t2 = a >= b",
                "if #t2 goto L1",
                "z = 2",
                "#t3 = y + 2",
                "p = #t3",
                "L1: z = 1",
                "#t4 = y + 1",
                "q = #t4",
            },
            TestName = "NoOptimizationAfterAllOptimizations")]

        public IEnumerable<string> TestAvailableExpressionsOptimization(string sourceCode, bool optimizeAll = false)
        {
            var cfg = optimizeAll ? BuildTACOptimizeCFG(sourceCode) : GenCFG(sourceCode);
            AvailableExpressionsOptimization.Execute(cfg, new AvailableExpressions().Execute(cfg));
            return cfg.GetInstructions().Select(x => x.ToString());
        }
    }
}
