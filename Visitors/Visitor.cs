using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors{
    public abstract class Visitor{
        public virtual void VisitIdNode(IdNode id) { }
        public virtual void VisitIntNumNode(IntNumNode num) { }
        public virtual void VisitBinOpNode(BinOpNode binop) { }
        public virtual void VisitAssignNode(AssignNode a) { }
        public virtual void VisitStListNode(StListNode bl) { }
        public virtual void VisitBoolValNode(BoolValNode b) { }
        public virtual void VisitWhileNode(WhileNode w) { }
        public virtual void VisitForNode(ForNode f) { }
        public virtual void VisitIfElseNode(IfElseNode i) { }
        public virtual void VisitPrintNode(PrintNode p) { }
        public virtual void VisitInputNode(InputNode i) { }
        public virtual void VisitExprListNode(ExprListNode e) { }
        public virtual void VisitVarListNode(VarListNode v) { }
        public virtual void VisitGotoNode(GotoNode g) { }
        public virtual void VisitLabelstatementNode(LabelStatementNode l) { }
        public virtual void VisitBlockNode(BlockNode b) { }

        public virtual void VisitEmptyNode(EmptyNode w) { }
    }
}
