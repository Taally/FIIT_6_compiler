using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprLeftLessRight : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binop)
            {
                if (binop.Left is IntNumNode && binop.Right is IntNumNode && (binop.Left as IntNumNode).Num < (binop.Right as IntNumNode).Num && binop.Op == OpType.LESS)
                {
                    ReplaceExpr(binop, new BoolValNode(true));
                }
                else
                if (binop.Left is IntNumNode && binop.Right is IntNumNode && (binop.Left as IntNumNode).Num > (binop.Right as IntNumNode).Num && binop.Op == OpType.LESS)
                {
                    ReplaceExpr(binop, new BoolValNode(false));
                }
                else
                if (binop.Left is BoolValNode lbo && binop.Right is BoolValNode rbo && binop.Op == OpType.LESS)
                {
                    if (lbo.Val == false && rbo.Val == true)
                        ReplaceExpr(binop, new BoolValNode(true));
                    else
                        ReplaceExpr(binop, new BoolValNode(false));
                }
                else
                {
                    base.VisitBinOpNode(binop);
                }
            }
        }
    }
}
