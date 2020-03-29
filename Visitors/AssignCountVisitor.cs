using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class AssignCountVisitor : AutoVisitor
    {
        public int Count = 0;
        public override void VisitAssignNode(AssignNode a)
        {
            Count += 1;
        }
        public override void VisitPrintNode(PrintNode w) 
        {
        }
        public override void VisitInputNode(InputNode w)
        {
        }
        public override void VisitVarListNode(VarListNode w)
        { 
        }
    }
}
