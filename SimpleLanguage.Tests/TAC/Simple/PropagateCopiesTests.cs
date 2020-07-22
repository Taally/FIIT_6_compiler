using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class PropagateCopiesTests : OptimizationsTestBase
    {
        [TestCase(@"
var a, b, c, d, e, x, y, k;
a = b;
c = b - a;
d = c + 1;
e = d * a;
a = x - y;
k = c + a;
",
            ExpectedResult = new string[]
            {
                "a = b",
                "#t1 = b - b",
                "c = #t1",
                "#t2 = #t1 + 1",
                "d = #t2",
                "#t3 = #t2 * b",
                "e = #t3",
                "#t4 = x - y",
                "a = #t4",
                "#t5 = #t1 + #t4",
                "k = #t5"
            },
            TestName = "Test1")]

        [TestCase(@"
var a, b, c, d, e, x, y, k;
b = x;
x = 5;
c = b + 5;
d = c;
e = d;
",
            ExpectedResult = new string[]
            {
                "b = x",
                "x = 5",
                "#t1 = b + 5",
                "c = #t1",
                "d = #t1",
                "e = #t1"
            },
            TestName = "Test2")]

        [TestCase(@"
var a, b, c, d, e, x, y, k;
a = b;
b = d + a;
e = a;
",
            ExpectedResult = new string[]
            {
                "a = b",
                "#t1 = d + b",
                "b = #t1",
                "e = a",
            },
            TestName = "Test3")]

        public IEnumerable<string> TestPropagateConstants(string sourceCode) =>
            TestTACOptimization(sourceCode, ThreeAddressCodeCopyPropagation.PropagateCopies);
    }
}
