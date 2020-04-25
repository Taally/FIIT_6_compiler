using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    class AssignCountVisitor : AutoVisitor
    {
        public int Count { get; set; }
        public override void VisitAssignNode(AssignNode aNode)
        {
            Count = Count + 1;
            aNode.Id.Visit(this);
            aNode.Expr.Visit(this);
        }
    }
}
