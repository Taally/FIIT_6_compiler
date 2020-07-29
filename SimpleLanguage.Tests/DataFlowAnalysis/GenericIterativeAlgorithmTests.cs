using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class GenericIterativeAlgorithmTests : OptimizationsTestBase
    {
        [Test]
        public void LiveVariables()
        {
            var program = @"
var a, b, c;

input(b);
a = b + 1;
if a < c
    c = b - a;
else
    c = b + a;
print(c);
";

            var cfg = GenCFG(program);
            var resActiveVariable = new LiveVariables().Execute(cfg);
            var actual = cfg.GetCurrentBasicBlocks()
                .Select(z => resActiveVariable[z])
                .Select(p => ((IEnumerable<string>)p.In, (IEnumerable<string>)p.Out))
                .ToList();

            var expected =
                new List<(IEnumerable<string>, IEnumerable<string>)>()
                {
                    (new HashSet<string>(){ "c" }, new HashSet<string>(){ "c" }),
                    (new HashSet<string>(){ "c" }, new HashSet<string>(){ "a", "b" }),
                    (new HashSet<string>(){ "a", "b" }, new HashSet<string>(){ "c" }),
                    (new HashSet<string>(){ "a", "b" }, new HashSet<string>(){ "c" }),
                    (new HashSet<string>(){ "c" }, new HashSet<string>(){ }),
                    (new HashSet<string>(){ }, new HashSet<string>(){ })
                };

            AssertSet(expected, actual);
        }

        [Test]
        public void ReachingDefinitions()
        {
            var TAC = GenTAC(@"
var a, b, c;

input(b);
a = b + 1;
if a < c
    c = b - a;
else
    c = b + a;
print(c);
");

            var cfg = GenCFG(TAC);
            var resReachingDefinitions = new ReachingDefinitions().Execute(cfg);
            var actual = cfg.GetCurrentBasicBlocks()
                .Select(z => resReachingDefinitions[z])
                .Select(p => ((IEnumerable<Instruction>)p.In, (IEnumerable<Instruction>)p.Out))
                .ToList();

            var expected =
                new List<(IEnumerable<Instruction>, IEnumerable<Instruction>)>()
                {
                    (new List<Instruction>(){ }, new List<Instruction>(){ }),
                    (new List<Instruction>(){ }, new List<Instruction>(){ TAC[2], TAC[0] }),
                    (new List<Instruction>(){ TAC[2], TAC[0] }, new List<Instruction>(){ TAC[6], TAC[2], TAC[0] }),
                    (new List<Instruction>(){ TAC[2], TAC[0] }, new List<Instruction>(){ TAC[9], TAC[2], TAC[0] }),
                    (new List<Instruction>(){ TAC[6], TAC[2], TAC[0], TAC[9] }, new List<Instruction>(){ TAC[6], TAC[2], TAC[0], TAC[9] }),
                    (new List<Instruction>(){ TAC[6], TAC[2], TAC[0], TAC[9] }, new List<Instruction>(){ TAC[6], TAC[2], TAC[0], TAC[9] })
                };

            AssertSet(expected, actual);
        }

        [Test]
        public void AvailableExpressions()
        {
            var TAC = GenTAC(@"
var a, b, c, d, x, e, g, y, zz, i;
2: a = x + y;
g = c + d;
3: zz = 1;
goto 1;
1: if (a < b)
    c = 1;
b = c + d;
goto 3;
e = zz + i;
");

            var cfg = GenCFG(TAC);
            var resAvailableExpressions = new AvailableExpressions().Execute(cfg);
            var actual = cfg.GetCurrentBasicBlocks()
                .Select(z => resAvailableExpressions[z])
                .Select(p => ((IEnumerable<OneExpression>)p.In, (IEnumerable<OneExpression>)p.Out))
                .ToList();

            var expected = new List<(IEnumerable<OneExpression> In, IEnumerable<OneExpression> Out)>()
            {
                // 0
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()),

                // 1
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "y"),
                    new OneExpression("PLUS", "c", "d")
                }),

                // 2
                (In: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "y"),
                    new OneExpression("PLUS", "c", "d")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "y"),
                    new OneExpression("PLUS", "c", "d")
                }),

                // 3
                (In: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "y"),
                    new OneExpression("PLUS", "c", "d")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "a", "b" ),
                    new OneExpression("PLUS", "x", "y"),
                    new OneExpression("PLUS", "c", "d")
                }),

                // 4
                (In: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "a", "b" ),
                    new OneExpression("PLUS", "x", "y"),
                    new OneExpression("PLUS", "c", "d")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "a", "b" ),
                    new OneExpression("PLUS", "x", "y"),
                }),

                // 5
                (In: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "a", "b" ),
                    new OneExpression("PLUS", "x", "y"),
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "c", "d"),
                    new OneExpression("PLUS", "x", "y")
                }),

                // 6
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>())
            };

            AssertSet(expected, actual);
        }

        private void AssertSet<T>(
            List<(IEnumerable<T> IN, IEnumerable<T> OUT)> expected,
            List<(IEnumerable<T> IN, IEnumerable<T> OUT)> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; ++i)
            {
                CollectionAssert.AreEquivalent(expected[i].IN, actual[i].IN);
                CollectionAssert.AreEquivalent(expected[i].OUT, actual[i].OUT);
            }
        }
    }
}
