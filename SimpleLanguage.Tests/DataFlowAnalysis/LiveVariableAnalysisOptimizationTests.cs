using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    using Optimization = Func<IReadOnlyList<Instruction>, (bool, IReadOnlyList<Instruction>)>;

    [TestFixture]
    internal class LiveVariableAnalysisOptimizationTests : OptimizationsTestBase
    {
        /*[Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
                var a,b,c;
input (b);
a = b + 1;
c = 6;
if a < c
	c = b - a;
else
	c = b + a;
print (c);");

            var optimizations = new List<Optimization> { LiveVariableAnalysisOptimization.LiveVariableDeleteDeadCode };

            var expected = new List<string>()
            {
                "input b",
                "#t1 = b + 1",
                "a = #t1",
                "#t2 = a < c",
                "if #t2 goto L1",
                "#t3 = b + a",
                "c = #t3",
                "goto L2",
                "L1: #t4 = b - a",
                "c = #t4",
                "L2: noop",
                "print c"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }*/
    }
}
