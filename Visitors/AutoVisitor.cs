using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    // базовая логика обхода без действий
    // Если нужны действия или другая логика обхода, то соответствующие методы надо переопределять
    // При переопределении методов для задания действий необходимо не забывать обходить подузлы
    class AutoVisitor: Visitor
    {
        public override void VisitBinOpNode(BinOpNode n) 
        {
            n.Left.Visit(this);
            n.Right.Visit(this);
        }
        public override void VisitAssignNode(AssignNode n) 
        {
            // для каких-то визиторов порядок может быть обратный - вначале обойти выражение, потом - идентификатор
            n.Id.Visit(this);
            n.Expr.Visit(this);
        }
        public override void VisitForNode(ForNode n) 
        {
            n.Id.Visit(this);
            n.Stat.Visit(this);
        }
        public override void VisitWhileNode(WhileNode n)
        {
            n.Expr.Visit(this);
            n.Stat.Visit(this);
        }
        public override void VisitBlockNode(BlockNode n) 
        {
            foreach (var st in n.StList)
                st.Visit(this);
        }
        public override void VisitPrintNode(PrintNode n) 
        {
            foreach (var e in n.exprList.exprList)
                e.Visit(this);
        }
        public override void VisitInputNode(InputNode n)
        {
            n.Ident.Visit(this);
        }
        public override void VisitVarListNode(VarListNode n) 
        {
            foreach (var v in n.vars)
                v.Visit(this);
        }
        public override void VisitIfElseNode(IfElseNode n)
        {
            n.Expr.Visit(this);
            n.TrueStat.Visit(this);
            if (n.FalseStat != null)
                n.FalseStat.Visit(this);
        }
        public override void VisitExprListNode(ExprListNode n)
        {
            foreach (var e in n.exprList)
                e.Visit(this);
        }
        public override void VisitGotoNode(GotoNode n)
        {
            n.Label.Visit(this);
        }
        public override void VisitLabelStatementNode(LabelStatementNode n)
        {
            n.Label.Visit(this);
            n.Stat.Visit(this);
        }

    }
}
