using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.CFG
{
    [TestFixture]
    internal class CFGdfs : TACTestsBase
    {
        [Test]
        public void Test1()
        {
            var TAC = GenTAC(@"
var a, b, c, d, x, u, e,g, y,zz,i;
goto 200;
200: a = 10 + 5;
for i=2,7 
	x = 1;
if c > a
{
	a = 1;
}
else 
{
    b = 1;
}
");

            var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
            var cfg = new ControlFlowGraph(blocks);


            foreach (var block in cfg.GetCurrentBasicBlocks())
            {
                Console.WriteLine($"{cfg.VertexOf(block)}  {block.GetInstructions()[0]}");
                var children = cfg.GetChildrenBasicBlocks(cfg.VertexOf(block));
                var childrenStr = string.Join(" | ", children.Select(v => v.vertex.ToString() + ": " + v.block.GetInstructions()[0].ToString()));
                Console.WriteLine($" children: {childrenStr}");
            }

            //            0
            //            ↓
            //            1
            //            ↓
            //            2
            //            ↓
            //        → → 3
            //        ↑  / \
            //        ← 4   \
            //              ↓
            //              5
            //             / \
            //            7   6
            //            ↓   ↓
            //            8 ← ←
            //            ↓
            //            9

            var nlr = new List<int>() { 0, 1, 2, 3, 5, 7, 8, 9, 6, 4 };
            CollectionAssert.AreEqual(nlr, cfg.PreOrderNumeration);

            Console.WriteLine("NLR");
            foreach (var v in cfg.PreOrderNumeration)
            {
                Console.Write("{0} ", v);
            }
            Console.WriteLine();

            var lrn = new List<int>() { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            CollectionAssert.AreEqual(lrn, cfg.PostOrderNumeration);

            Console.WriteLine("LRN");
            foreach (var v in cfg.PostOrderNumeration)
            {
                Console.Write("{0} ", v);
            }
            Console.WriteLine();

            lrn.Reverse();

            for (var i = 0; i < lrn.Count; ++i)
            {
                Assert.AreEqual(cfg.DepthFirstNumeration[lrn[i]], i);
            }

            Console.WriteLine("DFN");
            for (var i = 0; i < cfg.DepthFirstNumeration.Count; ++i)
            {
                Console.Write("{0}->{1} ", i, cfg.DepthFirstNumeration[i]);
            }
            Console.WriteLine();

            var check = new bool[cfg.GetCurrentBasicBlocks().Count];
            foreach ((var u, var v) in cfg.DepthFirstSpanningTree)
            {
                check[u] = true;
                check[v] = true;
            }

            foreach (var c in check)
            {
                Assert.IsTrue(c);
            }

            Assert.AreEqual(check.Length - 1, cfg.DepthFirstSpanningTree.Count);

            var ce = new List<(int from, int to, ControlFlowGraph.EdgeType type)>{
                (0, 1, ControlFlowGraph.EdgeType.Advancing),
                (1, 2, ControlFlowGraph.EdgeType.Advancing),
                (2, 3, ControlFlowGraph.EdgeType.Advancing),
                (3, 5, ControlFlowGraph.EdgeType.Advancing),
                (5, 7, ControlFlowGraph.EdgeType.Advancing),
                (7, 8, ControlFlowGraph.EdgeType.Advancing),
                (8, 9, ControlFlowGraph.EdgeType.Advancing),
                (5, 6, ControlFlowGraph.EdgeType.Advancing),
                (6, 8, ControlFlowGraph.EdgeType.Cross),
                (3, 4, ControlFlowGraph.EdgeType.Advancing),
                (4, 3, ControlFlowGraph.EdgeType.Retreating)
                };
            
            Assert.AreEqual(ce.Count, cfg.ClassifiedEdges.Count);
            Assert.AreEqual(
                ce.FindAll(c => c.type == ControlFlowGraph.EdgeType.Cross).Count, 
                1
            );
            Assert.AreEqual(
                ce.FindAll(c => c.type == ControlFlowGraph.EdgeType.Retreating).Count, 
                1
            );
            Assert.AreEqual(
                ce.FindAll(c => c.type == ControlFlowGraph.EdgeType.Advancing).Count, 
                9
            );
            Assert.IsTrue(ce.Find(x => x.from == 6 && x.to == 8).type == ControlFlowGraph.EdgeType.Cross);
            Assert.IsTrue(ce.Find(x => x.from == 4 && x.to == 3).type == ControlFlowGraph.EdgeType.Retreating);
            foreach (var c in cfg.ClassifiedEdges)
            {
                Assert.Contains(c, ce);
            }
            CollectionAssert.AreEqual(ce, cfg.ClassifiedEdges);
            Console.WriteLine("ClassifiedEdges");
            foreach (var c in cfg.ClassifiedEdges)
            {
                Console.WriteLine(c);
            }
        }
    }
}
