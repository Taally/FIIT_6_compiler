using NUnit.Framework;
using SimpleLang;
using System.Collections.Generic;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    class GenericIterativeAlgorithmTest : TACTestsBase
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
            var resActiveVariable = activeVariable.ExecuteThroughItAlg(cfg);
            HashSet<string> In = new HashSet<string>();
            HashSet<string> Out = new HashSet<string>();
            List<(HashSet<string> IN, HashSet<string> OUT)> actual = new List<(HashSet<string> IN, HashSet<string> OUT)>();
            foreach (var x in resActiveVariable)
            {
                foreach (var y in x.Value.In)
                {
                    In.Add(y);
                }

                foreach (var y in x.Value.Out)
                {
                    Out.Add(y);
                }
                actual.Add((new HashSet<string>(In), new HashSet<string>(Out)));
                In.Clear(); Out.Clear();
            }

            List<(HashSet<string> IN, HashSet<string> OUT)> expected =
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
            List<Instruction> In = new List<Instruction>();
            List<Instruction> Out = new List<Instruction>();
            List<(List<Instruction> IN, List<Instruction> OUT)> actual = new List<(List<Instruction> IN, List<Instruction> OUT)>();
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

            List<(List<Instruction> IN, List<Instruction> OUT)> expected =
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

        void AssertSet(
            List<(HashSet<string> IN, HashSet<string> OUT)> expected,
            List<(HashSet<string> IN, HashSet<string> OUT)> actual)
        {
            for (int i = 0; i < expected.Count; ++i)
            {
                Assert.True(expected[i].IN.SetEquals(actual[i].IN));
                Assert.True(expected[i].OUT.SetEquals(actual[i].OUT));
            }
        }

        void AssertSet(
            List<(List<Instruction> IN, List<Instruction> OUT)> expected,
            List<(List<Instruction> IN, List<Instruction> OUT)> actual)
        {
            for (int i = 0; i < expected.Count; ++i)
            {
                for(int j =  0; j < expected[i].IN.Count; j++)
                {
                    Assert.True(expected[i].IN[j].ToString().Equals(actual[i].IN[j].ToString()));
                }

                for (int j = 0; j < expected[i].OUT.Count; j++)
                {
                    Assert.True(expected[i].OUT[j].ToString().Equals(actual[i].OUT[j].ToString()));
                }
            }
        }
    }
}
