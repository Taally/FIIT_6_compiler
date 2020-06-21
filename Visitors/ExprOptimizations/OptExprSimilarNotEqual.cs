using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprSimilarNotEqual : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (
                binop.Left is IntNumNode iLeft && binop.Right is IntNumNode iRight && iLeft.Num == iRight.Num
                && (binop.Op == OpType.GREATER || binop.Op == OpType.LESS || binop.Op == OpType.NOTEQUAL)
                || binop.Left is IdNode idLeft && binop.Right is IdNode idRight && idLeft.Name == idRight.Name
                && (binop.Op == OpType.GREATER || binop.Op == OpType.LESS || binop.Op == OpType.NOTEQUAL)
                )
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
