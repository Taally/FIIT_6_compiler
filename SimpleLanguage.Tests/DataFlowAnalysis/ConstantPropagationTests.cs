using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    internal class ConstantPropagationTests : OptimizationsTestBase
    {
        [Test]
        public void NoBlocks()
        {
            var program = @"
var a,b,c;
";
            var blocks = GenBlocks(program);
            Assert.AreEqual(0, blocks.Count);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);

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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);
            Assert.AreEqual("5", actual["a"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["u"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["p"].Type);

            Assert.AreEqual("3", actual["u"].ConstValue);
            Assert.AreEqual("5", actual["p"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["b"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["c"].Type);

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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["b"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);

            Assert.AreEqual("3", actual["b"].ConstValue);
            Assert.AreEqual("7", actual["a"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["b"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);

            Assert.AreEqual("3", actual["b"].ConstValue);
            Assert.AreEqual("6", actual["a"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["b"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["c"].Type);

            Assert.AreEqual("2", actual["a"].ConstValue);
            Assert.AreEqual("3", actual["b"].ConstValue);
            Assert.AreEqual("4", actual["c"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.NAC, actual["a"].Type);
            Assert.AreEqual(LatticeTypeData.NAC, actual["b"].Type);
            Assert.AreEqual(LatticeTypeData.NAC, actual["c"].Type);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            Assert.AreEqual(LatticeTypeData.NAC, InOut.OUT[blocks.Last()]["c"].Type);
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
            Assert.AreEqual(6, blocks.Count);
            var cfg = new ControlFlowGraph(blocks);
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.NAC, actual["c"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["x"].Type);
            Assert.AreEqual("10", actual["a"].ConstValue);
            Assert.AreEqual("10", actual["x"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.NAC, actual["a"].Type);
            Assert.AreEqual(LatticeTypeData.NAC, actual["x"].Type);
            Assert.AreEqual(LatticeTypeData.NAC, actual["c"].Type);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["x"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["c"].Type);

            Assert.AreEqual("10", actual["x"].ConstValue);
            Assert.AreEqual("20", actual["a"].ConstValue);
            Assert.AreEqual("30", actual["c"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];
            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["x"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["c"].Type);

            Assert.AreEqual("20", actual["a"].ConstValue);
            Assert.AreEqual("30", actual["c"].ConstValue);
            Assert.AreEqual("10", actual["x"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["c"].Type);
            Assert.AreEqual("7", actual["c"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["c"].Type);
            Assert.AreEqual("4", actual["c"].ConstValue);
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
            var InOut = new ConstantPropagation().ExecuteNonGeneric(cfg);
            var actual = InOut.OUT[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.CONST, actual["a"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["b"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["c"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, actual["d"].Type);

            Assert.AreEqual("5", actual["a"].ConstValue);
            Assert.AreEqual("5", actual["b"].ConstValue);
            Assert.AreEqual("5", actual["c"].ConstValue);
            Assert.AreEqual("5", actual["d"].ConstValue);
        }

        [Test]
        public void ConstPropagationIterative()
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
            var constProp = new ConstantPropagation();
            var result = constProp.Execute(cfg);
            var blocks = cfg.GetCurrentBasicBlocks();
            var (_, Out) = result[blocks.Last()];

            Assert.AreEqual(LatticeTypeData.NAC, Out["c"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, Out["x"].Type);
            Assert.AreEqual(LatticeTypeData.CONST, Out["a"].Type);

            Assert.AreEqual("10", Out["x"].ConstValue);
            Assert.AreEqual("10", Out["a"].ConstValue);
        }
    }
}
