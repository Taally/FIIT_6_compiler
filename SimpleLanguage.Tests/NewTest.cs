using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests
{
    [TestFixture]
    public class NewTest : OptimizationsTestBase
    {
        [Test]
        public void Test()
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
            var before = cfg.GetCurrentBasicBlocks().SelectMany(z => z.GetInstructions().Select(t => t.ToString()));
            LiveVariableAnalysisOptimization.DeleteDeadCode(cfg);
            var after = cfg.GetCurrentBasicBlocks().SelectMany(z => z.GetInstructions().Select(t => t.ToString()));
        }
    }
}
