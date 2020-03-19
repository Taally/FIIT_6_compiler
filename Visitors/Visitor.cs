using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public abstract class Visitor
    {
        public virtual void VisitIdNode(IdNode node) { }
        public virtual void VisitIntNumNode(IntNumNode node) { }
        public virtual void VisitBoolValNode(BoolValNode node) { }
        public virtual void VisitBinOpNode(BinOpNode node) { }
        public virtual void VisitAssignNode(AssignNode node) { }
        public virtual void VisitWhileNode(WhileNode node) { }
        public virtual void VisitForNode(ForNode node) { }
        public virtual void VisitBlockNode(BlockNode node) { }
        public virtual void VisitPrintNode(PrintNode node) { }
        public virtual void VisitInputNode(InputNode node) { }
        public virtual void VisitIfElseNode(IfElseNode node) { }
        public virtual void VisitVarListNode(VarListNode node) { }
        public virtual void VisitExprListNode(ExprListNode node) { }

        public virtual void VisitEmptyNode(EmptyNode node) { }
    }
}
