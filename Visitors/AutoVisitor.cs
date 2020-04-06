using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors{
    class AutoVisitor : Visitor{
        public virtual void PreVisit(Node n){ }
        public virtual void PostVisit(Node n){ }

        public override void VisitBinOpNode(BinOpNode n){
            PreVisit(n);
            n.Left.Visit(this);
            n.Right.Visit(this);
            PostVisit(n);
        }

        public override void VisitAssignNode(AssignNode n){
            PreVisit(n);
            n.Id.Visit(this);
            n.Expr.Visit(this);
            PostVisit(n);
        }

        public override void VisitStListNode(StListNode n){
            PreVisit(n);
            for (int i = 0; i < n.StList.Count; ++i)
                n.StList[i].Visit(this);
            PostVisit(n);
        }

        public override void VisitVarListNode(VarListNode n){
            PreVisit(n);
            foreach (var v in n.vars)
                v.Visit(this);
            PostVisit(n);
        }

        public override void VisitBlockNode(BlockNode n){
            PreVisit(n);
            n.List.Visit(this);
            PostVisit(n);
        }

        public override void VisitExprListNode(ExprListNode n){
            PreVisit(n);
            for (int i = 0; i < n.exprList.Count; ++i)
                n.exprList[i].Visit(this);
            PostVisit(n);
        }

        public override void VisitForNode(ForNode n){
            PreVisit(n);
            n.Id.Visit(this);
            n.From.Visit(this);
            n.To.Visit(this);
            n.Stat.Visit(this);
            PostVisit(n);
        }

        public override void VisitGotoNode(GotoNode n){
            PreVisit(n);
            n.Label.Visit(this);
            PostVisit(n);
        }

        public override void VisitIfElseNode(IfElseNode n){
            PreVisit(n);
            n.Expr.Visit(this);
            n.TrueStat.Visit(this);
            if (n.FalseStat != null)
                n.FalseStat.Visit(this);
            PostVisit(n);
        }

        public override void VisitInputNode(InputNode n){
            PreVisit(n);
            n.Ident.Visit(this);
            PostVisit(n);
        }

        public override void VisitLabelstatementNode(LabelStatementNode n){
            PreVisit(n);
            n.Label.Visit(this);
            n.Stat.Visit(this);
            PostVisit(n);
        }

        public override void VisitPrintNode(PrintNode n){
            PreVisit(n);
            n.exprList.Visit(this);
            PostVisit(n);
        }

        public override void VisitWhileNode(WhileNode n){
            PreVisit(n);
            n.Expr.Visit(this);
            n.Stat.Visit(this);
            PostVisit(n);
        }

        public override void VisitBoolValNode(BoolValNode n){
            PreVisit(n);
            PostVisit(n);
        }

        public override void VisitIdNode(IdNode n){
            PreVisit(n);
            PostVisit(n);
        }

        public override void VisitIntNumNode(IntNumNode n){
            PreVisit(n);
            PostVisit(n);
        }
    }
}
