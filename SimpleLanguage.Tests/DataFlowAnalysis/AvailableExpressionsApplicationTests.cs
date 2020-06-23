using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;
using System.Linq;
using SimpleLang.DataFlowAnalysis;

namespace SimpleLanguage.Tests.CFG
{
    [TestFixture]
    internal class AvailableExpressionsApplicationTests : TACTestsBase
    {
        private IReadOnlyList<BasicBlock> GeneratorBasicBlockAfterOptimization(List<Instruction> TAC)
        {
            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var inOutData = new AvailableExpressions().Execute(cfg);
            AvailableExpressionsApplication.Execute(cfg, inOutData);
            return cfg.GetCurrentBasicBlocks();
        }

        private List<string> GetStringFromBlocks(IReadOnlyList<BasicBlock> basicBlocks)
        {
            var listOfInstructions = new List<string>();
            foreach (var block in basicBlocks)
            {
                foreach (var instr in block.GetInstructions())
                {
                    listOfInstructions.Add(instr.ToString());
                }
            }
            return listOfInstructions;
        }
        [Test]
        public void EmptyProgram()
        {
            var TAC = GenTAC(@"var a;");
            var availableExpressionOpt = GeneratorBasicBlockAfterOptimization(TAC);
            var expected = new List<BasicBlock>() { new BasicBlock(), new BasicBlock() };
            Assert.AreEqual(availableExpressionOpt.Count, expected.Count);
        }
        [Test]
        public void NoOptimizationInNotReducibleGraph() // hardest
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e, g, y, zz, i;
if a > b
{
    c = d;
    1:  x = e;
    goto 2;
}
else
{
    e = g;
    2: a = d;
    goto 1;
}");
            var availableExprOpt = GeneratorBasicBlockAfterOptimization(TAC);
            var graph = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var expectedBlock = graph.GetCurrentBasicBlocks();

            Assert.AreEqual(availableExprOpt.Count, expectedBlock.Count);

            var actual = GetStringFromBlocks(availableExprOpt);
            var expected = GetStringFromBlocks(expectedBlock);

            CollectionAssert.AreEqual(actual, expected);
        }
        [Test]
        public void SimpleProgram()
        {
            var TAC = GenTAC(@"var a, b, x, y, z, p, q, s;
x = y + z;
if (a < b)
{
    p = y + z;
}
q = y + z;");
            var availableExpr = GeneratorBasicBlockAfterOptimization(TAC);
            var actual = GetStringFromBlocks(availableExpr);
            var expected = new List<string>()
            {
                "#in: noop",
                "#t6 = y + z",
                "#t5 = #t6",
                "#t1 = #t5",
                "x = #t1",
                "#t2 = a < b",
                "if #t2 goto L1",
                "goto L2",
                "L1: #t3 = #t5",
                "p = #t3",
                "L2: noop" ,
                "#t4 = #t6",
                "q = #t4",
                "#out: noop",
            };
            Assert.AreEqual(expected.Count, actual.Count);
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void SimpleProgramWithGoto()
        {
            var TAC = GenTAC(@"var a, b, x, y, z, p, q, s;
x = y + z;
goto 1;
1: p = y + z;
goto 2;
2: q = y + z;"
                );
            var availableExpr = GeneratorBasicBlockAfterOptimization(TAC);
            var actual = GetStringFromBlocks(availableExpr);
            var expected = new List<string>()
            {
                "#in: noop",
                "#t5 = y + z",
                "#t4 = #t5",
                "#t1 = #t4",
                "x = #t1",
                "goto 1",
                "1: #t2 = #t4",
                "p = #t2",
                "goto 2",
                "2: #t3 = #t5",
                "q = #t3",
                "#out: noop"
            };
            Assert.AreEqual(actual.Count, expected.Count);
            CollectionAssert.AreEqual(actual, expected);
        }
        [Test]
        public void ProgramWithLoopForNoOptimization1()
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e,g, y,zz,i; 
zz = i + x;
for i=2,7 
{
	x = x + d; 
	a = a + b;
}");
            var availableExpr = GeneratorBasicBlockAfterOptimization(TAC);
            var actual = GetStringFromBlocks(availableExpr);
            var expected = new List<string>()
            {
                "#in: noop",
                "#t1 = i + x",
                "zz = #t1",
                "i = 2",
                "L1: #t2 = i >= 7",
                "if #t2 goto L2",
                "#t3 = x + d",
                "x = #t3",
                "#t4 = a + b",
                "a = #t4",
                "i = i + 1",
                "goto L1",
                "L2: noop",
                "#out: noop",
            };
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ProgramWithLoopForNoOptimization2()
        {
            var TAC = GenTAC(@"var a, b, c, d, x, u, e,g, y,zz,i; 
e = x + d;
zz = i + x;
for i=2,7 
{
	x = x + d; 
	a = a + b;
}");
            var availableExpr = GeneratorBasicBlockAfterOptimization(TAC);
            var actual = GetStringFromBlocks(availableExpr);
            var expected = new List<string>()
            {
"#in: noop",
"#t1 = x + d",
"e = #t1",
"#t2 = i + x",
"zz = #t2",
"i = 2",
"L1: #t3 = i >= 7",
"if #t3 goto L2",
"#t4 = x + d",
"x = #t4",
"#t5 = a + b",
"a = #t5",
"i = i + 1",
"goto L1",
"L2: noop",
"#out: noop",
            };
            CollectionAssert.AreEqual(expected, actual);
        }
        [Test]
        public void NoOptimizationAfterAllOptimization()
        {
            var graph = BuildTACOptimizeCFG(@"var a, b, x, y, z, p, q, s;
x = y + z;
y = 1;
if (a < b)
{
    z = 2;
    p = y + z;
}
z = 1;
q = y + z;");
            var ae = new AvailableExpressions().Execute(graph);
            AvailableExpressionsApplication.Execute(graph, ae);
            var expected = new List<string>()
            {
"#in: noop",
"#t1 = y + z",
"x = #t1",
"y = 1",
"#t2 = a < b",
"if #t2 goto L1",
"goto L2",
"L1: z = 2",
"#t3 = y + 2",
"p = #t3",
"L2: z = 1",
"#t4 = y + 1",
"q = #t4",
"#out: noop"
            };
            var actual = GetStringFromBlocks(graph.GetCurrentBasicBlocks());
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
