using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public abstract class Visitor
    {
        public virtual void VisitIdNode(IdNode n) { }
        public virtual void VisitIntNumNode(IntNumNode n) { }
        public virtual void VisitBinOpNode(BinOpNode n) { }
        public virtual void VisitAssignNode(AssignNode n) { }
        public virtual void VisitBlockNode(BlockNode n) { }
        public virtual void VisitVarListNode(VarListNode n) { }
        public virtual void VisitEmptyNode(EmptyNode n) { }
        public virtual void VisitBoolValNode(BoolValNode n) { }
        public virtual void VisitWhileNode(WhileNode n) { }
        public virtual void VisitForNode(ForNode n) { }
        public virtual void VisitIfElseNode(IfElseNode n) { }
        public virtual void VisitPrintNode(PrintNode n) { }
        public virtual void VisitInputNode(InputNode n) { }
        public virtual void VisitExprListNode(ExprListNode n) { }
        public virtual void VisitGotoNode(GotoNode n) { }
        public virtual void VisitLabelStatementNode(LabelStatementNode n) { }

    }
}
