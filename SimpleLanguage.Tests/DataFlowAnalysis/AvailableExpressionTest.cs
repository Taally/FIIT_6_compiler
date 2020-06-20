using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    public class AvailableExpressionTest : TACTestsBase
    {
        private ControlFlowGraph cfg;
        private List<OneExpression> In;
        private List<OneExpression> Out;
        private InOutData<List<OneExpression>> resAvailableExpressions;
        private AvailableExpressions availableExpressions;

        private List<(List<OneExpression>, List<OneExpression>)> GetActualInOutData(List<Instruction> TAC)
        {
            cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            availableExpressions = new AvailableExpressions();
            resAvailableExpressions = availableExpressions.Execute(cfg);
            In = new List<OneExpression>();
            Out = new List<OneExpression>();
            var actual = new List<(List<OneExpression>, List<OneExpression>)>();
            foreach (var block in resAvailableExpressions)
            {
                foreach (var expr in block.Value.In)
                {
                    In.Add(expr);
                }
                foreach (var expr in block.Value.Out)
                {
                    Out.Add(expr);
                }
                actual.Add((new List<OneExpression>(In), new List<OneExpression>(Out)));
                In.Clear();
                Out.Clear();
            }
            return actual;
        }
        private void AssertSet(
            List<(List<OneExpression>, List<OneExpression>)> expected,
            List<(List<OneExpression>, List<OneExpression>)> actual)
        {
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.True(SetEquals(expected[i].Item1, actual[i].Item1));
                Assert.True(SetEquals(expected[i].Item2, actual[i].Item2));
            }
        }
        private bool SetEquals(List<OneExpression> listOfExpr1, List<OneExpression> listOfExpr2)
        {
            for (var i = 0; i < listOfExpr1.Count; i++)
            {
                if (listOfExpr1[i].ToString() != listOfExpr2[i].ToString())
                {
                    return false;
                }
            }
            return true;
        }
        [Test]
        public void EmptyProgram()
        {
            var TAC = GenTAC(@"var a;");
            var actual = GetActualInOutData(TAC);

            var expected = new List<(List<OneExpression>, List<OneExpression>)>()
            {
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>())
            };
            Assert.AreEqual(2, actual.Count);
            AssertSet(expected, actual);
        }
        [Test]
        public void SimpleProgramWithUnreachableCode()
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e,g, y,zz,i; 
2: a = x + y;
g = c + d;
3: zz = 1;
goto 1;
1: if(a < b) 
    c = 1; 
b = c + d;
goto 3;
e = zz + i;"
);
            var actual = GetActualInOutData(TAC);
            var expected = new List<(List<OneExpression>, List<OneExpression>)>()
            {
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>()
                { new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d") } ),

                (new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d") } ,
                new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")}),

                (new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d") },
                new List<OneExpression>() { new OneExpression("LESS", "a", "b" ),
                    new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")}),

                (new List<OneExpression>() { new OneExpression("LESS", "a", "b" ), new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")},
                new List<OneExpression>() { new OneExpression("LESS", "a", "b" )
                , new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")}
                ),

                (new List<OneExpression>() { new OneExpression("LESS", "a", "b" ), new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")},
                new List<OneExpression>() { new OneExpression("LESS", "a", "b" ) , new OneExpression("PLUS", "x", "y") }
                ),

                ( new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("LESS", "a", "b")},
                  new List<OneExpression>() { new OneExpression("PLUS", "c", "d"), new OneExpression("PLUS", "x", "y")}
                ),

                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "zz", "i") }),

                (new List<OneExpression>() { new OneExpression("PLUS", "zz", "i") } ,
                new List<OneExpression>() { new OneExpression("PLUS", "zz", "i")})

            };
            Assert.AreEqual(expected.Count, actual.Count);
            AssertSet(expected, actual);
        }
        [Test]
        public void SimpleTestWithUnreachableCode2()
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e, g, y, zz, i;
1: g = c + d;
b = c + x;
goto 1;
e = zz + i;");
            var actual = GetActualInOutData(TAC);
            var expected = new List<(List<OneExpression>, List<OneExpression>)>()
            {
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "c", "d"), new OneExpression("PLUS", "c", "x")} ),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "zz", "i")}),
                (new List<OneExpression>() {new OneExpression("PLUS", "zz", "i") }, new List<OneExpression>() {new OneExpression("PLUS", "zz", "i")})
            };
            Assert.AreEqual(expected.Count, actual.Count);
            AssertSet(expected, actual);
        }
        public void ProgramWithLoopFor()
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e,g, y,zz,i; 
zz = i + x;
for i=2,7 
{
	x = x + d; 
	a = a + b;
}");
            var actual = GetActualInOutData(TAC);
            var expected = new List<(List<OneExpression>, List<OneExpression>)>()
            {
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "i", "x")}),
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>() 
                { new OneExpression("PLUS", "x", "d"), new OneExpression("PLUS", "a", "b"), new OneExpression("PLUS", "i", "1") }),
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>())
            };
            Assert.AreEqual(expected, actual);
            AssertSet(expected, actual);
        }
        [Test]
        public void ProgramWithLoopWhile()
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e,g, y,zz,i; 
zz = i + x;
while (e < g) 
{
	x = x + d; 
	a = a + b;
}");
            var actual = GetActualInOutData(TAC);
            var expected = new List<(List<OneExpression>, List<OneExpression>)>()
            {
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "i", "x") }),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("LESS", "e", "g") }),
                (new List<OneExpression>() { new OneExpression("LESS", "e", "g")}, new List<OneExpression>() { new OneExpression("LESS", "e", "g") }),
                (new List<OneExpression>() { new OneExpression("LESS", "e", "g") }, 
                new List<OneExpression>() { new OneExpression("PLUS", "x", "d"), new OneExpression("PLUS", "a", "b"), new OneExpression("LESS", "e", "g") }),
                (new List<OneExpression>() { new OneExpression("LESS", "e", "g")}, new List<OneExpression>() { new OneExpression("LESS", "e", "g") }),
                (new List<OneExpression>() { new OneExpression("LESS", "e", "g")}, new List<OneExpression>() { new OneExpression("LESS", "e", "g") })
            };
            Assert.AreEqual(actual.Count, expected.Count);
            AssertSet(expected, actual);
        }
        [Test]
        public void TrashProgramWithGoto()
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e,g, y,zz,i; 
zz = i + x;
for i=2,7 
{
	b = x + d; 
	1: x = e + g;
}
a = c + d;
goto 1;");
            var actual = GetActualInOutData(TAC);
            var expected = new List<(List<OneExpression>, List<OneExpression>)>()
            {
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "i", "x") }),
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "x", "d") }),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "e", "g"), new OneExpression("PLUS","i","1")}),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "c", "d") }),
                (new List<OneExpression>(), new List<OneExpression>())
            };
            Assert.AreEqual(expected.Count, actual.Count);
            AssertSet(expected, actual);
        }
        [Test]
        public void ProgramWithCrossGoTo()
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e,g, y,zz,i; 
a = b + c;
1: d = x + u;
goto 2;
x = x + x;
2: e = g + zz;
goto 1;
i = a + b;");
            var actual = GetActualInOutData(TAC);
            var expected = new List<(List<OneExpression>, List<OneExpression>)>()
            {
                (new List<OneExpression>(), new List<OneExpression>()),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "b", "c") }),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "x", "u") }),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "x", "x") }),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "g", "zz")}),
                (new List<OneExpression>(), new List<OneExpression>() { new OneExpression("PLUS", "a", "b") }),
                (new List<OneExpression>() { new OneExpression("PLUS", "a", "b")}, new List<OneExpression>() { new OneExpression("PLUS", "a", "b") })
            };
            Assert.AreEqual(expected.Count, actual.Count, "Размер");
            AssertSet(expected, actual);
        }
    }
}
