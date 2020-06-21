using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.CFG
{
    [TestFixture]
    internal class CFGcreate : TACTestsBase
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

            var vertexCount = cfg.GetCurrentBasicBlocks().Count;

            Assert.AreEqual(vertexCount, blocks.Count + 2); // standart blocks, in and out
            Assert.AreEqual(cfg.GetChildrenBasicBlocks(0).Count, 1); // inblock have 1 child
            Assert.AreEqual(cfg.GetParentsBasicBlocks(0).Count, 0);  // inblock not have parents
            Assert.AreEqual(cfg.GetChildrenBasicBlocks(vertexCount - 1).Count, 0); // outblock not have childs
            Assert.AreEqual(cfg.GetParentsBasicBlocks(vertexCount - 1).Count, 1); // outblock have 1 parent


            var graphBlocks = cfg.GetCurrentBasicBlocks();

            var vertex1 = cfg.VertexOf(graphBlocks[1]); // goto 200;
            Assert.AreEqual(vertex1, 1);
            Assert.AreEqual(cfg.GetChildrenBasicBlocks(vertex1).Count, 1);

            var vertex2 = cfg.VertexOf(graphBlocks[2]); // 200: a = 10 + 5;
            Assert.AreEqual(vertex2, 2);
            Assert.AreEqual(cfg.GetChildrenBasicBlocks(vertex2).Count, 1);
            //
            var vertex3 = cfg.VertexOf(graphBlocks[3]); // for i=2,7
            Assert.AreEqual(vertex3, 3);
            var children3 = cfg.GetChildrenBasicBlocks(vertex3);
            Assert.AreEqual(children3.Count, 2); // for and next block

            Assert.AreEqual(children3[0].vertex, 5); // for body
            var forBody = children3[0].block.GetInstructions();
            Assert.AreEqual(forBody[0].ToString(), "L2: noop");
            Assert.AreEqual(cfg.GetChildrenBasicBlocks(children3[0].vertex).Count, 2);

            Assert.AreEqual(children3[1].vertex, 4); // next
            ///
            var vertex6 = cfg.VertexOf(graphBlocks[6]); // if
            Assert.AreEqual(vertex6, 6);
            var children6 = cfg.GetChildrenBasicBlocks(vertex6);
            Assert.AreEqual(children6.Count, 1);
        }
    }
}
