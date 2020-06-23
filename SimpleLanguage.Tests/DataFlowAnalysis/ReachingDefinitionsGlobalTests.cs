using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    public class ReachingDefinitionsGlobalTests : TACTestsBase
    {
        [Test]
        public void SimpleTest()
        {
            var graph = GenCFG(@"
var a, b, c;
a = 1;
b = 2;
c = 3;
");
            new ReachingDefinitionsGlobal().DeleteDeadCode(graph);

            var actual = graph.GetCurrentBasicBlocks().SelectMany(b => b.GetInstructions().Select(i => i.ToString()));
            var expected = new[]
            {
                "#in: noop",
                "a = 1",
                "b = 2",
                "c = 3",
                "#out: noop"
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void OneBlockTest()
        {
            var graph = GenCFG(@"
var a, b, c;
a = 1;
b = 2;
c = 3;
a = 4;
c = 5;
c = 6;
");
            new ReachingDefinitionsGlobal().DeleteDeadCode(graph);

            // this optimization doesn't delete definitions which are rewritten in the same block
            var actual = graph.GetCurrentBasicBlocks().SelectMany(b => b.GetInstructions().Select(i => i.ToString()));
            var expected = new[]
            {
                "#in: noop",
                "a = 1",
                "b = 2",
                "c = 3",
                "a = 4",
                "c = 5",
                "c = 6",
                "#out: noop"
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SomeBlocksInARow()
        {
            var graph = GenCFG(@"
var a, b, c;
a = 1;
b = 2;
c = 3;
goto 1;
1: a = 4;
c = 5;
goto 2;
2: c = 6;
");
            new ReachingDefinitionsGlobal().DeleteDeadCode(graph);
            
            var actual = graph.GetCurrentBasicBlocks().SelectMany(b => b.GetInstructions().Select(i => i.ToString()));
            var expected = new[]
            {
                "#in: noop",
                "b = 2",
                "goto 1",
                "1: a = 4",
                "goto 2",
                "2: c = 6",
                "#out: noop"
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void DeleteDefinitionsWithLabels()
        {
            var graph = GenCFG(@"
var a, b, c;
1: a = 1;
2: b = 2;
3 : c = 3;
goto 4;
4: a = 4;
5: c = 5;
goto 6;
6: c = 6;
");
            
            new ReachingDefinitionsGlobal().DeleteDeadCode(graph);

            var actual = graph.GetCurrentBasicBlocks().SelectMany(b => b.GetInstructions().Select(i => i.ToString()));
            var expected = new[]
            {
                "#in: noop",
                "1: noop",
                "2: b = 2",
                "3: noop",
                "goto 4",
                "4: a = 4",
                "5: noop",
                "goto 6",
                "6: c = 6",
                "#out: noop"
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void IfTest()
        {
            var graph = GenCFG(@"
var a, b, c, d, e;
a = 2;
b = 5;
if e > 0
    a = 10;
else
    b = 1;

c = 0;
d = -1;
if e == 0
{
    c = 5;
    d = -5;
}
else
    d = 0;
");

            new ReachingDefinitionsGlobal().DeleteDeadCode(graph);

            var actual = graph.GetCurrentBasicBlocks().SelectMany(b => b.GetInstructions().Select(i => i.ToString()));
            var expected = new[]
            {
                "#in: noop",
                "a = 2",
                "b = 5",
                "#t1 = e > 0",
                "if #t1 goto L1",
                "b = 1",
                "goto L2",
                "L1: a = 10",
                "L2: noop",
                "c = 0",
                "#t2 = -1",
                "#t3 = e == 0",
                "if #t3 goto L3",
                "d = 0",
                "goto L4",
                "L3: c = 5",
                "#t4 = -5",
                "d = #t4",
                "L4: noop",
                "#out: noop"
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void LoopTest()
        {
            var graph = BuildTACOptimizeCFG(@"
var a, b, c, d, i;
a = 1;
b = 2;
for i = 1, 10
    a = 10;
for i = 2, 3
    b = 3;
c = 5;
d = 6;
for i = 10, 100
{
    c = 128;
    d = 256;
}
");

            new ReachingDefinitionsGlobal().DeleteDeadCode(graph);

            var actual = graph.GetCurrentBasicBlocks().SelectMany(b => b.GetInstructions().Select(i => i.ToString()));
            var expected = new[]
            {
                "#in: noop",
                "a = 1",
                "b = 2",
                "i = 1",
                "L1: #t1 = i >= 10",
                "if #t1 goto L2",
                "a = 10",
                "i = i + 1",
                "goto L1",
                "L2: i = 2",
                "L3: #t2 = i >= 3",
                "if #t2 goto L4",
                "b = 3",
                "i = i + 1",
                "goto L3",
                "L4: c = 5",
                "d = 6",
                "i = 10",
                "L5: #t3 = i >= 100",
                "if #t3 goto L6",
                "c = 128",
                "d = 256",
                "i = i + 1",
                "goto L5",
                "L6: noop",
                "#out: noop"
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ComplexTest()
        {
            var graph = BuildTACOptimizeCFG(@"
var a, b, c, d, i, j;
a = 1;
b = 2;
i = 100;
while i > 0
{
    i = i / 2;
    a = a + 1;
    b = b - 1;
    if i > 20
    {
        a = 10;
        b = 20;
    }
    else
        b = 10;
    d = a * b;
    if (d > 100)
        d = a * a;
    else
        d = b * b;
    for j = 1, 5
        d = d * 2;
}
");

            new ReachingDefinitionsGlobal().DeleteDeadCode(graph);

            var actual = graph.GetCurrentBasicBlocks().SelectMany(b => b.GetInstructions().Select(i => i.ToString()));
            var expected = new[]
            {
                "#in: noop",
                "a = 1",
                "b = 2",
                "i = 100",
                "L1: #t1 = i > 0",
                "if #t1 goto L2",
                "goto L3",
                "L2: #t2 = i / 2",
                "i = #t2",
                "#t3 = a + 1",
                "a = #t3",
                "#t4 = b - 1",
                "#t5 = #t2 > 20",
                "if #t5 goto L4",
                "b = 10",
                "goto L5",
                "L4: a = 10",
                "b = 20",
                "L5: #t6 = a * b",
                "#t7 = #t6 > 100",
                "if #t7 goto L6",
                "#t8 = b * b",
                "d = #t8",
                "goto L7",
                "L6: #t9 = a * a",
                "d = #t9",
                "L7: j = 1",
                "L8: #t10 = j >= 5",
                "if #t10 goto L1",
                "#t11 = d * 2",
                "d = #t11",
                "j = j + 1",
                "goto L8",
                "L3: noop",
                "#out: noop"
            };

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
