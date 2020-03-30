using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors{
    class AutoVisitor : Visitor{
        public override void VisitBinOpNode(BinOpNode binop){
            binop.Left.Visit(this);
            binop.Right.Visit(this);
        }
        public override void VisitAssignNode(AssignNode a){
            a.Id.Visit(this);
            a.Expr.Visit(this);
        }

        public override void VisitStListNode(StListNode bl){
            for (int i = 0; i < bl.StList.Count; ++i)
            {
                bl.StList[i].Visit(this);
            }
            //foreach (var st in bl.StList)
                //st.Visit(this);
        }

        public override void VisitVarListNode(VarListNode w){
            foreach (var v in w.vars)
                v.Visit(this);
        }

        public override void VisitBlockNode(BlockNode b){
            b.List.Visit(this);
        }

        public override void VisitExprListNode(ExprListNode e){
            foreach (var x in e.exprList)
                x.Visit(this);
        }

        public override void VisitForNode(ForNode f){
            f.Id.Visit(this);
            f.From.Visit(this);
            f.To.Visit(this);
            f.Stat.Visit(this);
        }

        public override void VisitGotoNode(GotoNode g){
            g.Label.Visit(this);
        }

        public override void VisitIfElseNode(IfElseNode i){
            i.Expr.Visit(this);
            i.TrueStat.Visit(this);
            if (i.FalseStat != null)
                i.FalseStat.Visit(this);
        }

        public override void VisitInputNode(InputNode i){
            i.Ident.Visit(this);
        }

        public override void VisitLabelstatementNode(LabelStatementNode l){
            l.Label.Visit(this);
            l.Stat.Visit(this);
        }

        public override void VisitPrintNode(PrintNode p){
            p.exprList.Visit(this);
        }

        public override void VisitWhileNode(WhileNode w){
            w.Expr.Visit(this);
            w.Stat.Visit(this);
        }

        public override void VisitBoolValNode(BoolValNode b)
        {
            
        }

        public override void VisitIdNode(IdNode id)
        {
            
        }

        public override void VisitIntNumNode(IntNumNode num)
        {
            
        }
    }
}
