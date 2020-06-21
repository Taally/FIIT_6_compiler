using ProgramTree;

namespace SimpleLang.Visitors
{
    internal class OptExprSimilarNotEqual : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (
                (binop.Op == OpType.GREATER || binop.Op == OpType.LESS || binop.Op == OpType.NOTEQUAL)
                &&
                // Для цифр и значений bool :
                (binop.Left is IntNumNode inl && binop.Right is IntNumNode inr && inl.Num == inr.Num
                || binop.Left is BoolValNode bvl && binop.Right is BoolValNode bvr && bvl.Val == bvr.Val
                // Для переменных :
                || binop.Left is IdNode idl && binop.Right is IdNode idr && idl.Name == idr.Name))
            {
                binop.Left.Visit(this);
                binop.Right.Visit(this); // Вначале сделать то же в правом поддереве
                ReplaceExpr(binop, new BoolValNode(false)); // Заменить себя на своё правое поддерево
            }
            else
            {
                base.VisitBinOpNode(binop);
            }

        }
    }
}
