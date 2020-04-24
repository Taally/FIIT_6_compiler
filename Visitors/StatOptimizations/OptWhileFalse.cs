using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors {
    class OptWhileFalseVisitor : ChangeVisitor{
        public override void PostVisit(Node nd)
        {
            if (!(nd is WhileNode n)) return;
            
            if (n.Expr is BoolValNode bn && !bn.Val) {
                ReplaceStat(n, new EmptyNode());
            } else {
                n.Expr.Visit(this);
            }
        }
    }
}