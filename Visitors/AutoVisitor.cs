using ProgramTree;

namespace SimpleLang.Visitors
{
    class AutoVisitor : Visitor
    {
        public virtual void PreVisit(Node n) { }
        public virtual void PostVisit(Node n) { }

        public override void VisitBinOpNode(BinOpNode binop)
        {
            PreVisit(binop);
            binop.Left.Visit(this);
            binop.Right.Visit(this);
            PostVisit(binop);
        }

        public override void VisitAssignNode(AssignNode a)
        {
            PreVisit(a);
            a.Id.Visit(this);
            a.Expr.Visit(this);
            PostVisit(a);
        }

        public override void VisitStListNode(StListNode bl)
        {
            PreVisit(bl);
            for (int i = 0; i < bl.StatChildren.Count; ++i)
                bl.StatChildren[i].Visit(this);
            PostVisit(bl);
        }

        public override void VisitVarListNode(VarListNode w)
        {
            PreVisit(w);
            foreach (var v in w.vars)
                v.Visit(this);
            PostVisit(w);
        }

        public override void VisitBlockNode(BlockNode b)
        {
            PreVisit(b);
            b.List.Visit(this);
            PostVisit(b);
        }

        public override void VisitExprListNode(ExprListNode e)
        {
            PreVisit(e);
            foreach (var x in e.ExprChildren)
                x.Visit(this);
            PostVisit(e);
        }

        public override void VisitForNode(ForNode f)
        {
            PreVisit(f);
            f.Id.Visit(this);
            f.From.Visit(this);
            f.To.Visit(this);
            f.Stat.Visit(this);
            PostVisit(f);
        }

        public override void VisitGotoNode(GotoNode g)
        {
            PreVisit(g);
            g.Label.Visit(this);
            PostVisit(g);
        }

        public override void VisitIfElseNode(IfElseNode i)
        {
            PreVisit(i);
            i.Expr.Visit(this);
            i.TrueStat.Visit(this);
            if (i.FalseStat != null)
                i.FalseStat.Visit(this);
            PostVisit(i);
        }

        public override void VisitInputNode(InputNode i)
        {
            PreVisit(i);
            i.Ident.Visit(this);
            PostVisit(i);
        }

        public override void VisitLabelstatementNode(LabelStatementNode l)
        {
            PreVisit(l);
            l.Label.Visit(this);
            l.Stat.Visit(this);
            PostVisit(l);
        }

        public override void VisitPrintNode(PrintNode p)
        {
            PreVisit(p);
            p.ExprList.Visit(this);
            PostVisit(p);
        }

        public override void VisitWhileNode(WhileNode w)
        {
            PreVisit(w);
            w.Expr.Visit(this);
            w.Stat.Visit(this);
            PostVisit(w);
        }
    }
}
