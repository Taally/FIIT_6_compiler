using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class GenericIterativeAlgorithmTest : TACTestsBase
    {
        [Test]
        public void LiveVariableIterativeTest()
        {
            var TAC = GenTAC(@"
var a,b,c;

input (b);
a = b + 1;
if a < c
	c = b - a;
else
	c = b + a;
print (c);"
);

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var activeVariable = new LiveVariableAnalysis();
            var resActiveVariable = activeVariable.Execute(cfg);
            var In = new HashSet<string>();
            var Out = new HashSet<string>();
            var actual = new List<(HashSet<string> IN, HashSet<string> OUT)>();
            foreach (var x in cfg.GetCurrentBasicBlocks().Select(z => resActiveVariable[z]))
            {
                foreach (var y in x.In)
                {
                    In.Add(y);
                }

                foreach (var y in x.Out)
                {
                    Out.Add(y);
                }
                actual.Add((new HashSet<string>(In), new HashSet<string>(Out)));
                In.Clear(); Out.Clear();
            }

            var expected =
                new List<(HashSet<string> IN, HashSet<string> OUT)>()
                {
                    (new HashSet<string>(){"c"}, new HashSet<string>(){ "c" }),
                    (new HashSet<string>(){"c"}, new HashSet<string>(){"a", "b"}),
                    (new HashSet<string>(){"a", "b"}, new HashSet<string>(){ "c" }),
                    (new HashSet<string>(){"a", "b"}, new HashSet<string>(){"c"}),
                    (new HashSet<string>(){"c"}, new HashSet<string>(){ }),
                    (new HashSet<string>(){ }, new HashSet<string>(){ })
                };

            AssertSet(expected, actual);
        }

        [Test]
        public void ReachingDefinitionIterativeTest()
        {
            var TAC = GenTAC(@"
var a,b,c;

input (b);
a = b + 1;
if a < c
	c = b - a;
else
	c = b + a;
print (c);"
);

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var reachingDefinitions = new ReachingDefinitions();
            var resReachingDefinitions = reachingDefinitions.Execute(cfg);
            var In = new List<Instruction>();
            var Out = new List<Instruction>();
            var actual = new List<(List<Instruction> IN, List<Instruction> OUT)>();
            foreach (var x in resReachingDefinitions)
            {
                foreach (var y in x.Value.In)
                {
                    In.Add(y);
                }

                foreach (var y in x.Value.Out)
                {
                    Out.Add(y);
                }
                actual.Add((new List<Instruction>(In), new List<Instruction>(Out)));
                In.Clear(); Out.Clear();
            }

            var expected =
                new List<(List<Instruction> IN, List<Instruction> OUT)>()
                {
                    (new List<Instruction>(){}, new List<Instruction>(){}),
                    (new List<Instruction>(){}, new List<Instruction>(){TAC[2], TAC[0]}),
                    (new List<Instruction>(){TAC[2], TAC[0]}, new List<Instruction>(){ TAC[6], TAC[2], TAC[0] }),
                    (new List<Instruction>(){TAC[2], TAC[0]}, new List<Instruction>(){ TAC[9], TAC[2], TAC[0] }),
                    (new List<Instruction>(){TAC[6], TAC[2], TAC[0], TAC[9]}, new List<Instruction>(){TAC[6], TAC[2], TAC[0], TAC[9]}),
                    (new List<Instruction>(){TAC[6], TAC[2], TAC[0], TAC[9]}, new List<Instruction>(){ TAC[6], TAC[2], TAC[0], TAC[9]})
                };

            AssertSet(expected, actual);
        }
        [Test]
        public void AvailableExpressionsTest()
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

            };

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var availableExpressions = new AvailableExpressions();
            var resAvailableExpressions = availableExpressions.Execute(cfg);
            var In = new List<OneExpression>();
            var Out = new List<OneExpression>();
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
            AssertSet(expected, actual);
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
        private void AssertSet(
            List<(HashSet<string> IN, HashSet<string> OUT)> expected,
            List<(HashSet<string> IN, HashSet<string> OUT)> actual)
        {
            for (var i = 0; i < expected.Count; ++i)
            {
                Assert.True(expected[i].IN.SetEquals(actual[i].IN));
                Assert.True(expected[i].OUT.SetEquals(actual[i].OUT));
            }
        }

        private void AssertSet(
            List<(List<Instruction> IN, List<Instruction> OUT)> expected,
            List<(List<Instruction> IN, List<Instruction> OUT)> actual)
        {
            for (var i = 0; i < expected.Count; ++i)
            {
                for (var j = 0; j < expected[i].IN.Count; j++)
                {
                    Assert.True(expected[i].IN[j].ToString().Equals(actual[i].IN[j].ToString()));
                }

                for (var j = 0; j < expected[i].OUT.Count; j++)
                {
                    Assert.True(expected[i].OUT[j].ToString().Equals(actual[i].OUT[j].ToString()));
                }
            }
        }
    }
}
