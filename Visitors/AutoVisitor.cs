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
        public override void VisitBinOpNode(BinOpNode node) 
        {
            node.Left.Visit(this);
            node.Right.Visit(this);
        }
        public override void VisitAssignNode(AssignNode node) 
        {
            // для каких-то визиторов порядок может быть обратный - вначале обойти выражение, потом - идентификатор
            node.Id.Visit(this);
            node.Expr.Visit(this);
        }
        public override void VisitWhileNode(WhileNode node) 
        {
            node.Expr.Visit(this);
            node.Stat.Visit(this);
        }
        public override void VisitForNode(ForNode node)
        {
            node.Ident.Visit(this);
            node.Stat.Visit(this);
        }
        public override void VisitBlockNode(BlockNode node) 
        {
            foreach (var st in node.StList)
                st.Visit(this);
        }
        public override void VisitPrintNode(PrintNode node) 
        {
            foreach (var expr in node.exprList.exprList)
                expr.Visit(this);
        }
        public override void VisitInputNode(InputNode node)
        {
            node.Ident.Visit(this);
        }

        public override void VisitVarListNode(VarListNode node) 
        {
            foreach (var v in node.vars)
                v.Visit(this);
        }

        public override void VisitIfElseNode(IfElseNode node)
        {
            node.Expr.Visit(this);
            node.TrueStat.Visit(this);
            node.FalseStat.Visit(this);
        }
    }
}
