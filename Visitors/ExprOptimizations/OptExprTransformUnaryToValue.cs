using System;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprTransformUnaryToValue : ChangeVisitor
    {
        public override void VisitUnOpNode(UnOpNode unop)
        {
            if (unop.Expr is IntNumNode num)
            {
                var vForNum = unop.Op == OpType.UNMINUS ? -1 * num.Num
                    : throw new ArgumentException("IntNumNode linked with UNMINUS");
                ReplaceExpr(unop, new IntNumNode(vForNum));
            }
            else if (unop.Expr is BoolValNode b)
            {
                var vForBool = unop.Op == OpType.NOT ? !b.Val
                    : throw new ArgumentException("BoolValNode linked with NOT");
                ReplaceExpr(unop, new BoolValNode(vForBool));
            }
            else if (unop.Expr is IdNode 
                 && unop.Parent is UnOpNode && (unop.Parent as UnOpNode).Op == unop.Op)
            {
                ReplaceExpr(unop.Parent as UnOpNode, unop.Expr);
            }
            else
            {
                base.VisitUnOpNode(unop);
            }
        }
    }
}
