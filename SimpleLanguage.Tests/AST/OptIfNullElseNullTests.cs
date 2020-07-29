using NUnit.Framework;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptIfNullElseNullTests : ASTTestsBase
    {
        [Test]
        public void RemoveSimple()
        {
            // if (a) EmptyNode;
            // if (a) EmptyNode; else EmptyNode;
            var if1 = new IfElseNode(new IdNode("a"), new EmptyNode());
            var if2 = new IfElseNode(new IdNode("a"), new EmptyNode(), new EmptyNode());

            var root = new StListNode(if1);
            root.Add(if2);
            if1.Parent = if2.Parent = root;

            var opt = new IfNullElseNull();
            root.Visit(opt);

            Assert.IsNull(root.Parent);
            Assert.AreEqual(root.ExprChildren.Count, 0);
            Assert.AreEqual(root.StatChildren.Count, 2);

            Assert.IsTrue(root.StatChildren[0] is EmptyNode);
            Assert.IsTrue(root.StatChildren[1] is EmptyNode);
        }

        [Test]
        public void RemoveInnerIf1()
        {
            // if (a)
            //   if (b) EmptyNode; else EmptyNode;
            var ifInner = new IfElseNode(new IdNode("b"), new EmptyNode(), new EmptyNode());
            var ifOuter = new IfElseNode(new IdNode("a"), ifInner);
            ifInner.Parent = ifOuter;

            var root = new StListNode(ifOuter);
            ifOuter.Parent = root;

            var opt = new IfNullElseNull();
            root.Visit(opt);

            Assert.IsNull(root.Parent);
            Assert.AreEqual(root.ExprChildren.Count, 0);
            Assert.AreEqual(root.StatChildren.Count, 1);
            Assert.IsTrue(root.StatChildren[0] is EmptyNode);
        }

        [Test]
        public void RemoveInnerIf2()
        {
            // if (a)
            //   if (b) EmptyNode;
            var ifInner = new IfElseNode(new IdNode("b"), new EmptyNode());
            var ifOuter = new IfElseNode(new IdNode("a"), ifInner);
            ifInner.Parent = ifOuter;

            var root = new StListNode(ifOuter);
            ifOuter.Parent = root;

            var opt = new IfNullElseNull();
            root.Visit(opt);

            Assert.IsNull(root.Parent);
            Assert.AreEqual(root.ExprChildren.Count, 0);
            Assert.AreEqual(root.StatChildren.Count, 1);
            Assert.IsTrue(root.StatChildren[0] is EmptyNode);
        }

        [Test]
        public void RemoveInBlock()
        {
            // { if (a) EmptyNode; }
            // { if (a) EmptyNode; else EmptyNode; }
            var if1 = new IfElseNode(new IdNode("a"), new EmptyNode());
            var if2 = new IfElseNode(new IdNode("a"), new EmptyNode(), new EmptyNode());

            var block1 = new BlockNode(new StListNode(if1));
            var block2 = new BlockNode(new StListNode(if2));
            if1.Parent = block1;
            if2.Parent = block2;

            var root = new StListNode(block1);
            root.Add(block2);
            block1.Parent = block2.Parent = root;

            var opt = new IfNullElseNull();
            root.Visit(opt);

            Assert.IsNull(root.Parent);
            Assert.AreEqual(root.ExprChildren.Count, 0);
            Assert.AreEqual(root.StatChildren.Count, 2);

            foreach (var node in root.StatChildren)
            {
                Assert.IsTrue(node is BlockNode);
                Assert.AreEqual(node.ExprChildren.Count, 0);
                Assert.AreEqual(node.StatChildren.Count, 1);
                Assert.IsTrue(node.StatChildren[0] is EmptyNode);
            }
        }

        [Test]
        public void WithoutRemoveSimple2()
        {
            // if (a) a = 0;
            // if (a) a = 0; else a = 1;
            var if1 = new IfElseNode(new IdNode("a"), new AssignNode(new IdNode("a"), new IntNumNode(0)));
            var if2 = new IfElseNode(new IdNode("a"), new AssignNode(new IdNode("a"), new IntNumNode(0)), new AssignNode(new IdNode("a"), new IntNumNode(1)));

            var root = new StListNode(if1);
            root.Add(if2);
            if1.Parent = if2.Parent = root;

            var opt = new IfNullElseNull();
            root.Visit(opt);

            Assert.IsNull(root.Parent);
            Assert.AreEqual(root.ExprChildren.Count, 0);
            Assert.AreEqual(root.StatChildren.Count, 2);

            Assert.IsTrue(root.StatChildren[0] is IfElseNode);
            Assert.IsTrue(root.StatChildren[1] is IfElseNode);
        }
    }
}
