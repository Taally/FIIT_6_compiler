using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class GenericIterativeAlgorithmTests : TACTestsBase
    {
        [Test]
        public void LiveVariableIterativeTest()
        {
            var program = @"
var a,b,c;

input (b);
a = b + 1;
if a < c
	c = b - a;
else
	c = b + a;
print (c);"
;

            var cfg = GenCFG(program);
            var resActiveVariable = new LiveVariableAnalysis().Execute(cfg);
            var actual = cfg.GetCurrentBasicBlocks()
                .Select(z => resActiveVariable[z])
                .Select(p => ((IEnumerable<string>)p.In, (IEnumerable<string>)p.Out))
                .ToList();

            var expected =
                new List<(IEnumerable<string>, IEnumerable<string>)>()
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

            var cfg = GenCFG(TAC);
            var resReachingDefinitions = new ReachingDefinitions().Execute(cfg);
            var actual = cfg.GetCurrentBasicBlocks()
                .Select(z => resReachingDefinitions[z])
                .Select(p => ((IEnumerable<Instruction>)p.In, (IEnumerable<Instruction>)p.Out))
                .ToList();

            var expected =
                new List<(IEnumerable<Instruction>, IEnumerable<Instruction>)>()
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

            var cfg = GenCFG(TAC);
            var resAvailableExpressions = new AvailableExpressions().Execute(cfg);
            var actual = cfg.GetCurrentBasicBlocks()
                .Select(z => resAvailableExpressions[z])
                .Select(p => ((IEnumerable<OneExpression>)p.In, (IEnumerable<OneExpression>)p.Out))
                .ToList();

            var expected = new List<(IEnumerable<OneExpression>, IEnumerable<OneExpression>)>()
            {
                (new List<OneExpression>(), 
                new List<OneExpression>()),

                (new List<OneExpression>(), 
                new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d") } ),

                (new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d") } ,
                new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")}),

                (new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d") },
                new List<OneExpression>() { new OneExpression("LESS", "a", "b" ), new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")}),

                (new List<OneExpression>() { new OneExpression("LESS", "a", "b" ), new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")},
                new List<OneExpression>() { new OneExpression("LESS", "a", "b" ), new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")}),

                (new List<OneExpression>() { new OneExpression("LESS", "a", "b" ), new OneExpression("PLUS", "x", "y"), new OneExpression("PLUS", "c", "d")},
                new List<OneExpression>() { new OneExpression("LESS", "a", "b" ) , new OneExpression("PLUS", "x", "y") }),

                (new List<OneExpression>() { new OneExpression("PLUS", "x", "y"), new OneExpression("LESS", "a", "b")},
                new List<OneExpression>() { new OneExpression("PLUS", "c", "d"), new OneExpression("PLUS", "x", "y")}),

            };

            AssertSet(expected, actual);
        }

        private void AssertSet<T>(
            List<(IEnumerable<T> IN, IEnumerable<T> OUT)> expected,
            List<(IEnumerable<T> IN, IEnumerable<T> OUT)> actual)
        {
            for (var i = 0; i < expected.Count; ++i)
            {
                CollectionAssert.AreEquivalent(expected[i].IN, actual[i].IN);
                CollectionAssert.AreEquivalent(expected[i].OUT, actual[i].OUT);
            }
        }

        /*private void AssertSet(
            List<(List<OneExpression>, List<OneExpression>)> expected,
            List<(List<OneExpression>, List<OneExpression>)> actual)
        {
            for (var i = 0; i < expected.Count; ++i)
            {
                for (var j = 0; j < expected[i].Item1.Count; j++)
                {
                    Assert.True(IsContains(expected[i].Item1[j], actual[i].Item1));
                }

                for (var j = 0; j < expected[i].Item2.Count; j++)
                {
                    Assert.True(IsContains(expected[i].Item2[j], actual[i].Item2));
                }
            }
        }

        private void AssertSet(
            List<(HashSet<string> IN, HashSet<string> OUT)> expected,
            List<(HashSet<string> IN, HashSet<string> OUT)> actual)
        {
            for (var i = 0; i < expected.Count; ++i)
            {
                CollectionAssert.AreEquivalent(expected[i].IN, actual[i].IN);
                CollectionAssert.AreEquivalent(expected[i].OUT, actual[i].OUT);
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
                    Assert.True(IsContains(expected[i].IN[j], actual[i].IN));
                }

                for (var j = 0; j < expected[i].OUT.Count; j++)
                {
                    Assert.True(IsContains(expected[i].OUT[j], actual[i].OUT));
                }
            }
        }

        private bool IsContains(Instruction findInstruction, List<Instruction> actualInstruction)
            => actualInstruction.Where(x => x.ToString() == findInstruction.ToString()).Any();

        private bool IsContains(OneExpression findInstruction, List<OneExpression> actualInstruction)
            => actualInstruction.Where(x => x.ToString() == findInstruction.ToString()).Any();
            */
    }
}
