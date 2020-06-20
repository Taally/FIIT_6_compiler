using System.Collections.Generic;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class FillParentsVisitor : AutoVisitor
    {
        private readonly Stack<Node> st = new Stack<Node>();

        public override void PreVisit(Node n)
        {
            n.Parent = st.Count != 0 ? st.Peek() : null;
            st.Push(n);
        }

        public override void PostVisit(Node n) => st.Pop();
    }
}
