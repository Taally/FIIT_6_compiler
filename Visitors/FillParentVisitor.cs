using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors{
    class FillParentVisitor : AutoVisitor{
        Stack<Node> st = new Stack<Node>();

        public override void VisitAssignNode(AssignNode a)
        {
            a.Parent = st.Peek();
            st.Push(a);
            base.VisitAssignNode(a);
            st.Pop();
        }

        public override void VisitBinOpNode(BinOpNode binop)
        {
            binop.Parent = st.Peek();
            st.Push(binop);
            base.VisitBinOpNode(binop);
            st.Pop();
        }

        public override void VisitBlockNode(BlockNode b)
        {
            b.Parent = st.Peek();
            st.Push(b);
            base.VisitBlockNode(b);
            st.Pop();
        }

        public override void VisitBoolValNode(BoolValNode b)
        {
            b.Parent = st.Peek();
        }

        public override void VisitExprListNode(ExprListNode e)
        {
            e.Parent = st.Peek();
            st.Push(e);
            base.VisitExprListNode(e);
            st.Pop();
        }

        public override void VisitForNode(ForNode f)
        {
            f.Parent = st.Peek();
            st.Push(f);
            base.VisitForNode(f);
            st.Pop();
        }

        public override void VisitGotoNode(GotoNode g)
        {
            g.Parent = st.Peek();
            st.Push(g);
            base.VisitGotoNode(g);
            st.Pop();
        }

        public override void VisitIdNode(IdNode id)
        {
            id.Parent = st.Peek();
            //base.VisitIdNode(id);
        }

        public override void VisitIfElseNode(IfElseNode i)
        {
            i.Parent = st.Peek();
            st.Push(i);
            base.VisitIfElseNode(i);
            st.Pop();
        }

        public override void VisitInputNode(InputNode i)
        {
            i.Parent = st.Peek();
            st.Push(i);
            base.VisitInputNode(i);
            st.Pop();
        }

        public override void VisitIntNumNode(IntNumNode num)
        {
            //base.VisitIntNumNode(num);
            num.Parent = st.Peek();
        }

        public override void VisitLabelstatementNode(LabelStatementNode l)
        {
            l.Parent = st.Peek();
            st.Push(l);
            base.VisitLabelstatementNode(l);
            st.Pop();
        }

        public override void VisitPrintNode(PrintNode p)
        {
            p.Parent = st.Peek();
            st.Push(p);
            base.VisitPrintNode(p);
            st.Pop();
        }

        public override void VisitStListNode(StListNode bl)
        {
            if(st.Count > 0)
                bl.Parent = st.Peek();
            st.Push(bl);
            base.VisitStListNode(bl);
            st.Pop();
        }

        public override void VisitVarListNode(VarListNode w)
        {
            w.Parent = st.Peek();
            st.Push(w);
            base.VisitVarListNode(w);
            st.Pop();
        }

        public override void VisitWhileNode(WhileNode w)
        {
            w.Parent = st.Peek();
            st.Push(w);
            base.VisitWhileNode(w);
            st.Pop();
        }
    }
}
