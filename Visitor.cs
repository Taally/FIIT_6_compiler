using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    public abstract class Visitor
    {
        public virtual void VisitExprNode(ExprNode exprNode) { }
        public virtual void VisitBinOpNode(BinOpNode binop) { }
        public virtual void VisitIdNode(IdNode id) { }
        public virtual void VisitIntNumNode(IntNumNode num) { }
        public virtual void VisitBoolNode(BoolNode b) { }        
        public virtual void VisitStatementNode(StatementNode stat) { }
        public virtual void VisitBlockNode(BlockNode block) { }
        public virtual void VisitAssignNode(AssignNode aNode) { }
        public virtual void VisitForNode(ForNode forNode) { }
        public virtual void VisitWhileNode(WhileNode whileNode) { }
        public virtual void VisitIfNode(IfNode ifNode) { }
        public virtual void VisitInputNode(InputNode inputNode) { }
        public virtual void VisitExprListNode(ExprListNode exprListNode) { }
        public virtual void VisitPrintNode(PrintNode printNode) { }
        public virtual void VisitVarNode(VarNode varNode) { }
        public virtual void VisitVarListNode(VarListNode varListNode) { }
        public virtual void VisitGotoNode(GotoNode gotoNode) { }
        public virtual void VisitLabelStatementNode(LabelStatementNode labelStatementNode) { }        
    }
}