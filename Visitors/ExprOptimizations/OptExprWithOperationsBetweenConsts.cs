using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprWithOperationsBetweenConsts : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binop)
            {
                if (binop.Left is IntNumNode && binop.Right is IntNumNode)
                {
                    if ((binop.Left as IntNumNode).Num < (binop.Right as IntNumNode).Num && binop.Op == OpType.LESS)
                    {
                        ReplaceExpr(binop, new BoolValNode(true));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num > (binop.Right as IntNumNode).Num && binop.Op == OpType.LESS)
                    {
                        ReplaceExpr(binop, new BoolValNode(false));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num > (binop.Right as IntNumNode).Num && binop.Op == OpType.GREATER)
                    {
                        ReplaceExpr(binop, new BoolValNode(true));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num < (binop.Right as IntNumNode).Num && binop.Op == OpType.GREATER)
                    {
                        ReplaceExpr(binop, new BoolValNode(false));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num >= (binop.Right as IntNumNode).Num && binop.Op == OpType.EQGREATER)
                    {
                        ReplaceExpr(binop, new BoolValNode(true));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num < (binop.Right as IntNumNode).Num && binop.Op == OpType.EQGREATER)
                    {
                        ReplaceExpr(binop, new BoolValNode(false));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num <= (binop.Right as IntNumNode).Num && binop.Op == OpType.EQLESS)
                    {
                        ReplaceExpr(binop, new BoolValNode(true));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num > (binop.Right as IntNumNode).Num && binop.Op == OpType.EQLESS)
                    {
                        ReplaceExpr(binop, new BoolValNode(false));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num != (binop.Right as IntNumNode).Num && binop.Op == OpType.NOTEQUAL)
                    {
                        ReplaceExpr(binop, new BoolValNode(true));
                    }
                    else
                    if ((binop.Left as IntNumNode).Num == (binop.Right as IntNumNode).Num && binop.Op == OpType.NOTEQUAL)
                    {
                        ReplaceExpr(binop, new BoolValNode(false));
                    }
                }
                else
                {
                    base.VisitBinOpNode(binop);
                }
            }
        }
    }
}
