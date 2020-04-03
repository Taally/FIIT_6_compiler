using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors{
    class OptStat1Visitor : ChangeVisitor{
        public override void VisitIfElseNode(IfElseNode n){
            if (n.Expr is BoolValNode bn && bn.Val == true){
                n.TrueStat.Visit(this);
                ReplaceStat(n, n.TrueStat);
            }
            else
                base.VisitIfElseNode(n);
        }
    }
}
