using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    public class AutoVisitor : Visitor
    {        
        public override void VisitBinOpNode(BinOpNode binOpNode)
        {          
            binOpNode.Left.Visit(this);
            binOpNode.Right.Visit(this);
        }
        public override void VisitBlockNode(BlockNode block)
        {
            int count = block.StList.Count();
            for (int i = 0; i < count; i++)
                block.StList[i].Visit(this);
        }
        public override void VisitAssignNode(AssignNode aNode)
        {
            aNode.Id.Visit(this);
            aNode.Expr.Visit(this);
        }
        public override void VisitForNode(ForNode forNode) 
        {
            forNode.Id.Visit(this);
            forNode.From.Visit(this);
            forNode.To.Visit(this);
            forNode.Stat.Visit(this);
        }
        public override void VisitWhileNode(WhileNode whileNode) 
        {
            whileNode.Expr.Visit(this);
            whileNode.Stat.Visit(this);
        }

        public override void VisitIfNode(IfNode ifNode) 
        {
            ifNode.Expr.Visit(this);
            ifNode.StatTrue.Visit(this);
            if (ifNode.StatFalse != null)
                ifNode.StatFalse.Visit(this);
        }

        public override void VisitInputNode(InputNode inputNode) 
        {
            inputNode.Id.Visit(this);
        }

        public override void VisitExprListNode(ExprListNode exprListNode)
        {
            int count = exprListNode.ExprList.Count();
            for (int i = 0; i < count; i++)
                exprListNode.ExprList[i].Visit(this);
        }

        public override void VisitPrintNode(PrintNode printNode)
        {
            printNode.ExprList.Visit(this);
        }
        
        public override void VisitVarListNode(VarListNode varListNode) 
        {
            int count = varListNode.VarList.Count();
            for (int i = 0; i < count; i++)
                varListNode.VarList[i].Visit(this);
        }
               
        public override void VisitLabelStatementNode(LabelStatementNode labelStatementNode) 
        {
            labelStatementNode.Label.Visit(this);
            labelStatementNode.Stat.Visit(this);
        }
    }
}
