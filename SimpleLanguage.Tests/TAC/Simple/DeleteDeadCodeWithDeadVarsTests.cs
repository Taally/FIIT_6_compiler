using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class DeleteDeadCodeWithDeadVarsTests : OptimizationsTestBase
    {
        [TestCase(@"
var a, b, c;
a = 1;
a = 2;
b = 11;
b = 22;
a = 3;
a = b;
c = 1;
a = b + c;
b = -c;
c = 1;
b = a - c;
a = -b;
",
            ExpectedResult = new string[]
            {
                "noop",
                "noop",
                "noop",
                "b = 22",
                "noop",
                "noop",
                "c = 1",
                "#t1 = b + c",
                "a = #t1",
                "noop",
                "noop",
                "c = 1",
                "#t3 = a - c",
                "b = #t3",
                "#t4 = -b",
                "a = #t4",
            },
            TestName = "Test")]

        [TestCase(@"
var a;
a = -a;
a = 1;
",
            ExpectedResult = new string[]
            {
                "noop",
                "noop",
                "a = 1"
            },
            TestName = "TempVars")]

        [TestCase(@"
var a;
a = true;
a = !a;
",
            ExpectedResult = new string[]
            {
                "a = True",
                "#t1 = !a",
                "a = #t1"
            },
            TestName = "Negation")]

        public IEnumerable<string> TestDeleteDeadCodeWithDeadVars(string sourceCode) =>
            TestTACOptimization(sourceCode, DeleteDeadCodeWithDeadVars.DeleteDeadCode);
    }
}
