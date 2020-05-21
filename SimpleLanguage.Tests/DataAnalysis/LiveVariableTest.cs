using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataAnalysis
{
    [TestFixture]
    class LiveVariableTest: TACTestsBase
    {
        [Test]
        public void SimpleTest() {
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

            var actual = Execute(TAC);
            AssertSet(expected, actual);
        }

        [Test]
        public void WithCycle() {
            var TAC = GenTAC(@"
var a,b,c;

input (b);

while a > 5{
	a = b + 1;
	c = 5;
}

print (c);"
);
            List<(HashSet<string> IN, HashSet<string> OUT)> expected =
                new List<(HashSet<string> IN, HashSet<string> OUT)>()
                {
                    (new HashSet<string>(){"a","c"}, new HashSet<string>(){"a","c"}),
                    (new HashSet<string>(){"a","c"}, new HashSet<string>(){"a","b","c"}),
                    (new HashSet<string>(){"a","b","c"}, new HashSet<string>(){"b", "c"}),
                    (new HashSet<string>(){ "c" }, new HashSet<string>(){ "c" }),
                    (new HashSet<string>(){"b"}, new HashSet<string>(){ "a", "b", "c"}),
                    (new HashSet<string>(){"c"}, new HashSet<string>(){ }),
                    (new HashSet<string>(){ }, new HashSet<string>(){ })
                };
            var actual = Execute(TAC);
           AssertSet(expected, actual);
        }

        [Test]
        public void ComplexWithCycleTest() {
            var TAC = GenTAC(@"
var a,b,c,i;

for i = 1,b {
	input (a);
	c = c + a;
	print(c);
	if c < b
		c = c + 1;
	else {
		b = b - 1;
		print(b);
		print(c);
	}
}

print (c+a+b);"
);
            List<(HashSet<string> IN, HashSet<string> OUT)> expected =
                new List<(HashSet<string> IN, HashSet<string> OUT)>()
                {
                    (new HashSet<string>(){"b","c","a"}, new HashSet<string>(){"c","b","a"}),
                    (new HashSet<string>(){"b","c","a"}, new HashSet<string>(){"c","b","i","a"}),
                    (new HashSet<string>(){"c","b","i","a"}, new HashSet<string>(){"c","b","i","a"}),
                    (new HashSet<string>(){"c","a","b"}, new HashSet<string>(){"c","a","b"}),
                    (new HashSet<string>(){"c","b","i"}, new HashSet<string>(){"c","b","i","a"}),
                    (new HashSet<string>(){"c","b","i","a"}, new HashSet<string>(){"c","b","i","a"}),
                    (new HashSet<string>(){"c","b","i","a"}, new HashSet<string>(){"c","b","i","a"}),
                    (new HashSet<string>(){"c","b","i","a"}, new HashSet<string>(){"c","b","i","a"}),
                    (new HashSet<string>(){"c","a","b"}, new HashSet<string>(){ }),
                    (new HashSet<string>(){ }, new HashSet<string>(){ })
                };
            var actual = Execute(TAC);
            AssertSet(expected, actual);
        }

        List<(HashSet<string> IN, HashSet<string> OUT)> Execute(List<Instruction> TAC)
        {
            var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
            var cfg = new ControlFlowGraph(blocks);

            var liveAct = new LiveVariableAnalysis();
            liveAct.Execute(cfg);

            var listAct = liveAct.dictInOut
                .Select(x => x.Value)
                .Select(y => (y.In, y.Out));
            //.Skip(1);
            //return listAct.Take(listAct.Count() - 1).ToList();
            return listAct.ToList();
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
    }
}
