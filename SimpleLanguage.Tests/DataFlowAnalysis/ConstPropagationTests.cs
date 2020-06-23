using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class ConstPropagationTests : OptimizationsTestBase
    {
        [Test]
        public void TestNoBlocks()
        {
            var program = @"
var a,b,c;
";
            var blocks = GenBlocks(program);
            Assert.AreEqual(0, blocks.Count);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);

            Assert.AreEqual(2, InOut.IN.Count);
            Assert.AreEqual(2, InOut.OUT.Count);
        }

        [Test]
        public void OneAssign()
        {
            var program = @"
var a,b,c;
a = 5;
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(InOut.OUT[blocks[0]]["a"].Type, LatticeTypeData.CONST);
            Assert.AreEqual("5", InOut.OUT[blocks[0]]["a"].ConstValue);
        }

        [Test]
        public void VariableAndConst()
        {
            var program = @"
var u,p,v;
u = 3;
p = u + 2;
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(InOut.OUT[blocks[0]]["u"].Type, LatticeTypeData.CONST);
            Assert.AreEqual(InOut.OUT[blocks[0]]["p"].Type, LatticeTypeData.CONST);

            Assert.AreEqual("3", InOut.OUT[blocks[0]]["u"].ConstValue);
            Assert.AreEqual("5", InOut.OUT[blocks[0]]["p"].ConstValue);
        }

        [Test]
        public void VariableAndConst2()
        {
            var program = @"
var a,b,c;
b = 3;
goto 11;
12: c = b + 2;
11: a = 7;
goto 12;
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[1]]["b"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[1]]["a"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[1]]["c"].Type);

            Assert.AreEqual("3", InOut.OUT[blocks[1]]["b"].ConstValue);
            Assert.AreEqual("7", InOut.OUT[blocks[1]]["a"].ConstValue);
            Assert.AreEqual("5", InOut.OUT[blocks[1]]["c"].ConstValue);
        }

        [Test]
        public void VariableAndConst3()
        {
            var program = @"
var a,b,c;
b = 3;
goto 11;
c = b + 2;
11: a = 7;
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[2]]["b"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[2]]["a"].Type);
            Assert.AreEqual(false, InOut.OUT[blocks[2]].ContainsKey("c"));

            Assert.AreEqual("3", InOut.OUT[blocks[2]]["b"].ConstValue);
            Assert.AreEqual("7", InOut.OUT[blocks[2]]["a"].ConstValue);
        }

        [Test]
        public void ConstAndVariable()
        {
            var program = @"
var a,b,c;
b = 3;
a = 2 * b;
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(InOut.OUT[blocks[0]]["b"].Type, LatticeTypeData.CONST);
            Assert.AreEqual(InOut.OUT[blocks[0]]["a"].Type, LatticeTypeData.CONST);

            Assert.AreEqual("3", InOut.OUT[blocks[0]]["b"].ConstValue);
            Assert.AreEqual("6", InOut.OUT[blocks[0]]["a"].ConstValue);
        }

        [Test]
        public void ComplicatedEquation()
        {
            var program = @"
var a,b,c;
a = 2;
b = 3;
c = a * b - 2;
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(InOut.OUT[blocks[0]]["a"].Type, LatticeTypeData.CONST);
            Assert.AreEqual(InOut.OUT[blocks[0]]["b"].Type, LatticeTypeData.CONST);
            Assert.AreEqual(InOut.OUT[blocks[0]]["c"].Type, LatticeTypeData.CONST);

            Assert.AreEqual("2", InOut.OUT[blocks[0]]["a"].ConstValue);
            Assert.AreEqual("3", InOut.OUT[blocks[0]]["b"].ConstValue);
            Assert.AreEqual("4", InOut.OUT[blocks[0]]["c"].ConstValue);
        }

        [Test]
        public void TransfNotDistr()
        {
            var program = @"
var a,b,c;
if c > 5
{
    a = 2;
    b = 3;
}
else
{
    a = 3;
    b = 2;
}
c = a + b;
";
            var blocks = GenBlocks(program);
            Assert.AreEqual(4, blocks.Count);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(InOut.OUT[blocks[3]]["a"].Type, LatticeTypeData.NAC);
            Assert.AreEqual(InOut.OUT[blocks[3]]["b"].Type, LatticeTypeData.NAC);
            Assert.AreEqual(InOut.OUT[blocks[3]]["c"].Type, LatticeTypeData.NAC);
        }

        [Test]
        public void InputAssignsNAC()
        {
            var program = @"
var a, x, c;
input(c);
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(LatticeTypeData.NAC, InOut.OUT[blocks[0]]["c"].Type);
        }

        [Test]
        public void PropagateOneVariant()
        {
            var program = @"
var a, x, c;
if c > 5
    x = 10;
else
    input(c);
if c > 5
    a = x;
";
            var blocks = GenBlocks(program);
            Assert.AreEqual(7, blocks.Count);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[5]]["a"].Type);
            Assert.AreEqual("10", InOut.OUT[blocks[5]]["a"].ConstValue);
            Assert.AreEqual(LatticeTypeData.NAC, InOut.OUT[blocks[5]]["c"].Type);
        }

        [Test]
        public void TwoConstValues()
        {
            var program = @"
var a, x, c;
input(c);
if c > 5
    x = 10;
else
    input(c);
if c > 5
    x = 20;
a = x;
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(LatticeTypeData.NAC, InOut.OUT[blocks[6]]["a"].Type);
            Assert.AreEqual(LatticeTypeData.NAC, InOut.OUT[blocks[6]]["x"].Type);
        }

        [Test]
        public void PropagateTwoVariants()
        {
            var program = @"
var a, x, c;
if c > 10
    x = 10;
else
    a = 20;
c = a + x;
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(InOut.OUT[blocks[3]]["c"].Type, LatticeTypeData.CONST);
            Assert.AreEqual(InOut.OUT[blocks[3]]["c"].ConstValue, "30");
        }

        [Test]
        public void PropagateTwoVariants2()
        {
            var program = @"
var a, x, c;
x = 10;
a = 20;
goto 666;
666: c = a + x;
";

            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(InOut.OUT[blocks[1]]["c"].Type, LatticeTypeData.CONST);
            Assert.AreEqual(InOut.OUT[blocks[1]]["c"].ConstValue, "30");
        }

        [Test]
        public void WhileProp()
        {
            var program = @"
var a, b, x, c;
while x > 1
{
	a = 2;
	b = 5;
}
c = a + b;
";

            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(InOut.OUT[blocks[3]]["c"].Type, LatticeTypeData.CONST);
            Assert.AreEqual(InOut.OUT[blocks[3]]["c"].ConstValue, "7");
        }

        [Test]
        public void ForProp()
        {
            var program = @"
var a, b, x, c;
for x=1,10
{
	a = 2;
	b = 2;
}
c = a + b;
";

            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[3]]["c"].Type);
            Assert.AreEqual("4", InOut.OUT[blocks[3]]["c"].ConstValue);
        }

        [Test]
        public void ForReverse()
        {
            var program = @"
var a, b, x, c, d;
for x=1,2
{
    a = b;
    b = c;
    c = d;
    d = 5;
}
";
            var blocks = GenBlocks(program);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[3]]["a"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[3]]["b"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[3]]["c"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, InOut.OUT[blocks[3]]["d"].Type);

            Assert.AreEqual("5", InOut.OUT[blocks[3]]["a"].ConstValue);
            Assert.AreEqual("5", InOut.OUT[blocks[3]]["b"].ConstValue);
            Assert.AreEqual("5", InOut.OUT[blocks[3]]["c"].ConstValue);
            Assert.AreEqual("5", InOut.OUT[blocks[3]]["d"].ConstValue);
        }

        [Test]
        public void ConstPropagationIterativeTest()
        {
            var program = @"
var a, x, c;
if c > 5
    x = 10;
else
    input(c);
if c > 5
    a = x;
";
            var cfg = GenCFG(program);
            var constProp = new ConstPropagation();
            var result = constProp.Execute(cfg);

            var blocks = cfg.GetCurrentBasicBlocks();

            Assert.AreEqual(LatticeTypeData.CONST, result[blocks[6]].Out["x"].Type);
            Assert.AreEqual("10", result[blocks[6]].Out["x"].ConstValue);
            Assert.AreEqual(LatticeTypeData.CONST, result[blocks[6]].Out["a"].Type);
            Assert.AreEqual("10", result[blocks[6]].Out["a"].ConstValue);
            Assert.AreEqual(LatticeTypeData.NAC, result[blocks[6]].Out["c"].Type);
        }
    }
}
