using System.Collections.Generic;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class FillParentsVisitor : AutoVisitor
    {
        readonly Stack<Node> st = new Stack<Node>();

        public override void PreVisit(Node n)
        {
            n.Parent = st.Peek();
            st.Push(n);
        }

        public override void PostVisit(Node n)
        {
            st.Pop();
        }

        public override void VisitStListNode(StListNode bl)
        {
            if (st.Count > 0)
                bl.Parent = st.Peek();
            st.Push(bl);
            base.VisitStListNode(bl);
            st.Pop();
        }
    }
}
