using NUnit.Framework;
using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests.CFG
{
    [TestFixture]
    class CFGdfs : TACTestsBase
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
                var childrenStr = String.Join(" | ", children.Select(v => v.Item1.ToString()+": "+v.Item2.GetInstructions()[0].ToString()));
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
            //        ← 5   4
            //              ↓
            //              6
            //             / \
            //            8   7
            //            ↓   ↓
            //            9 ← ←
            //            ↓
            //            10

            var nlr = new List<int>() { 0, 1, 2, 3, 5, 4, 6, 8, 9, 10, 7 };
            CollectionAssert.AreEqual(nlr, cfg.NLR);

            Console.WriteLine("NLR");
            foreach (var v in cfg.NLR)
                Console.Write("{0} ", v);
            Console.WriteLine();

            var lrn = new List<int>() { 5, 10, 9, 8, 7, 6, 4, 3, 2, 1, 0 };
            CollectionAssert.AreEqual(lrn, cfg.LRN);

            Console.WriteLine("LRN");
            foreach (var v in cfg.LRN)
                Console.Write("{0} ", v);
            Console.WriteLine();

            lrn.Reverse();

            for (int i = 0; i< lrn.Count; ++i)
                Assert.AreEqual(cfg.DFN[lrn[i]], i);

            Console.WriteLine("DFN");
            for (int i = 0; i < cfg.DFN.Count; ++i)
                Console.Write("{0}->{1} ", i, cfg.DFN[i]);
            Console.WriteLine();


            var check = new bool[cfg.GetCurrentBasicBlocks().Count];
            foreach ((var u, var v) in cfg.DFST)
            {
                check[u] = true;
                check[v] = true;
            }

            foreach (var c in check)
                Assert.IsTrue(c);

            Assert.AreEqual(check.Length - 1, cfg.DFST.Count);
        }
    }
}
