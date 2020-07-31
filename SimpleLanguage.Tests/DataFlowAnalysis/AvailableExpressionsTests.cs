using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    public class AvailableExpressionsTests : OptimizationsTestBase
    {
        private List<(IEnumerable<OneExpression>, IEnumerable<OneExpression>)> GetActualInOutData(string program)
        {
            var cfg = GenCFG(program);
            var resActiveVariable = new AvailableExpressions().Execute(cfg);
            var actual = cfg.GetCurrentBasicBlocks()
                .Select(z => resActiveVariable[z])
                .Select(p => ((IEnumerable<OneExpression>)p.In, (IEnumerable<OneExpression>)p.Out))
                .ToList();
            return actual;
        }

        private void AssertSet(
            List<(IEnumerable<OneExpression> In, IEnumerable<OneExpression> Out)> expected,
            List<(IEnumerable<OneExpression> In, IEnumerable<OneExpression> Out)> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                CollectionAssert.AreEquivalent(expected[i].In, actual[i].In, $"In: i = {i}");
                CollectionAssert.AreEquivalent(expected[i].Out, actual[i].Out, $"Out: i = {i}");
            }
        }

        [Test]
        public void EmptyProgram()
        {
            var actual = GetActualInOutData(@"var a;");
            var expected = new List<(IEnumerable<OneExpression>, IEnumerable<OneExpression>)>()
            {
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>())
            };
            AssertSet(expected, actual);
        }

        [Test]
        public void SimpleProgramWithUnreachableCode()
        {
            var actual = GetActualInOutData(@"
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

        [Test]
        public void SimpleProgramWithUnreachableCode2()
        {
            var actual = GetActualInOutData(@"
var b, c, d, x, e, g, zz, i;
1: g = c + d;
b = c + x;
goto 1;
e = zz + i;
");
            var expected = new List<(IEnumerable<OneExpression> In, IEnumerable<OneExpression> Out)>()
            {
                // 0
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()),

                // 1
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "c", "d"),
                    new OneExpression("PLUS", "c", "x")
                }),

                // 2
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>())
            };
            Assert.AreEqual(expected.Count, actual.Count);
            AssertSet(expected, actual);
        }

        [Test]
        public void ProgramWithLoopFor()
        {
            var actual = GetActualInOutData(@"
var a, b, d, x, zz, i;
zz = i + x;
for i = 2, 7
{
    x = x + d;
    a = a + b;
}
");
            var expected = new List<(IEnumerable<OneExpression> In, IEnumerable<OneExpression> Out)>()
            {
                // 0
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()),

                // 1
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "i", "x")
                }),

                // 2
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                }),

                // 3
                (In: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "d"),
                    new OneExpression("PLUS", "a", "b"),
                    new OneExpression("PLUS", "i", "1")
                }),

                // 4
                (In: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                }),

                // 5
                (In: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                })
            };
            Assert.AreEqual(expected.Count, actual.Count);
            AssertSet(expected, actual);
        }

        [Test]
        public void ProgramWithLoopWhile()
        {
            var actual = GetActualInOutData(@"
var a, b, d, x, e, g, zz, i;
zz = i + x;
while (e < g)
{
    x = x + d;
    a = a + b;
}
");
            var expected = new List<(IEnumerable<OneExpression> In, IEnumerable<OneExpression> Out)>()
            {
                // 0
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()),

                // 1
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "i", "x")
                }),

                // 2
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("LESS", "e", "g")
                }),

                // 3
                (In: new List<OneExpression>()
                {
                    new OneExpression("LESS", "e", "g")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("LESS", "e", "g")
                }),

                // 4
                (In: new List<OneExpression>()
                {
                    new OneExpression("LESS", "e", "g")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "d"),
                    new OneExpression("PLUS", "a", "b"),
                    new OneExpression("LESS", "e", "g")
                }),

                // 5
                (In: new List<OneExpression>()
                {
                    new OneExpression("LESS", "e", "g")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("LESS", "e", "g")
                }),

                // 6
                (In: new List<OneExpression>()
                {
                    new OneExpression("LESS", "e", "g")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("LESS", "e", "g")
                })
            };
            Assert.AreEqual(actual.Count, expected.Count);
            AssertSet(expected, actual);
        }

        [Test]
        public void TrashProgramWithGoto()
        {
            var actual = GetActualInOutData(@"
var a, b, c, d, x, e, g, zz, i;
zz = i + x;
for i = 2 ,7
{
    b = x + d;
    1: x = e + g;
}
a = c + d;
goto 1;
");
            var expected = new List<(IEnumerable<OneExpression> In, IEnumerable<OneExpression> Out)>()
            {
                // 0
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()),

                // 1
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "i", "x")
                }),

                // 2
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                }),

                // 3
                (In: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "d"),
                    new OneExpression("EQGREATER", "i", "7")
                }),

                // 4
                (In: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "e", "g"),
                    new OneExpression("PLUS", "i", "1")
                }),

                // 5
                (In: new List<OneExpression>()
                {
                    new OneExpression("EQGREATER", "i", "7")
                },
                new List<OneExpression>()
                {
                    new OneExpression("PLUS", "c", "d"),
                    new OneExpression("EQGREATER", "i", "7")
                }),

                // 6
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>())
            };
            Assert.AreEqual(expected.Count, actual.Count);
            AssertSet(expected, actual);
        }

        [Test]
        public void ProgramWithCrossGoTo()
        {
            var actual = GetActualInOutData(@"
var a, b, c, d, x, u, e, g, zz, i;
a = b + c;
1: d = x + u;
goto 2;
x = x + x;
2: e = g + zz;
goto 1;
i = a + b;
");
            var expected = new List<(IEnumerable<OneExpression> In, IEnumerable<OneExpression> Out)>()
            {
                // 0
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()),

                // 1
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "b", "c")
                }),

                // 2
                (In: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "b", "c")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "u"),
                    new OneExpression("PLUS", "b", "c")
                }),

                // 3
                (In: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "x", "u"),
                    new OneExpression("PLUS", "b", "c")
                },
                Out: new List<OneExpression>()
                {
                    new OneExpression("PLUS", "g", "zz"),
                    new OneExpression("PLUS", "x", "u"),
                    new OneExpression("PLUS", "b", "c")
                }),

                // 4
                (In: new List<OneExpression>(),
                Out: new List<OneExpression>())
            };
            Assert.AreEqual(expected.Count, actual.Count, "Размер");
            AssertSet(expected, actual);
        }
    }
}
