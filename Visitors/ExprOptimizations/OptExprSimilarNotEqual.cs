using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprSimilarNotEqual : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (
                (binop.Op == OpType.GREATER || binop.Op == OpType.LESS || binop.Op == OpType.NOTEQUAL)
                &&
                // Для цифр и значений bool:
                (binop.Left is IntNumNode inl && binop.Right is IntNumNode inr && inl.Num == inr.Num
                || binop.Left is BoolValNode bvl && binop.Right is BoolValNode bvr && bvl.Val == bvr.Val
                // Для переменных:
                || binop.Left is IdNode idl && binop.Right is IdNode idr && idl.Name == idr.Name))
            {
                ReplaceExpr(binop, new BoolValNode(false));
            }
            else
            {
                base.VisitBinOpNode(binop);
            }

        }
    }
}
