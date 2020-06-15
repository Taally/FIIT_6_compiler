using System;
using ProgramTree;

namespace SimpleLang.Visitors
{
    internal class OptExprTransformUnaryToValue : ChangeVisitor
    {
        public override void VisitUnOpNode(UnOpNode unop)
        {
            if (unop.Expr is IntNumNode num)
            {
                if (unop.Op == OpType.UNMINUS)
                {
                    ReplaceExpr(unop, new IntNumNode(-1 * num.Num));
                }
                else
                {
                    throw new ArgumentException("IntNumNode linked with UNMINUS");
                }
            }
            else if (unop.Expr is BoolValNode b)
            {
                if (unop.Op == OpType.NOT)
                {
                    ReplaceExpr(unop, new BoolValNode(!b.Val));
                }
                else
                {
                    throw new ArgumentException("BoolValNode linked with NOT");
                }
            }
            else if (unop.Expr is IdNode)
            {
                if (unop.Parent is UnOpNode && (unop.Parent as UnOpNode).Op == unop.Op)
                {
                    ReplaceExpr(unop.Parent as UnOpNode, unop.Expr);
                }
            }
            else
            {
                base.VisitUnOpNode(unop);
            }
        }
    }
}
