using ProgramTree;

namespace SimpleLang.Visitors
{
    internal class OptExprSimilarNotEqual : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (
                // Для цифр и значений bool :
                binop.Left is IntNumNode && binop.Right is IntNumNode && (binop.Left as IntNumNode).Num == (binop.Right as IntNumNode).Num && (binop.Op == OpType.GREATER || binop.Op == OpType.LESS)
                || binop.Left is BoolValNode && binop.Right is BoolValNode && (binop.Left as BoolValNode).Val == (binop.Right as BoolValNode).Val && (binop.Op == OpType.GREATER || binop.Op == OpType.LESS)
                || binop.Left is IntNumNode && binop.Right is IntNumNode && (binop.Left as IntNumNode).Num == (binop.Right as IntNumNode).Num && binop.Op == OpType.NOTEQUAL
                || binop.Left is BoolValNode && binop.Right is BoolValNode && (binop.Left as BoolValNode).Val == (binop.Right as BoolValNode).Val && binop.Op == OpType.NOTEQUAL
                // Для переменных :
                || binop.Left is IdNode && binop.Right is IdNode && (binop.Left as IdNode).Name == (binop.Right as IdNode).Name && (binop.Op == OpType.GREATER || binop.Op == OpType.LESS)
                || binop.Left is IdNode && binop.Right is IdNode && (binop.Left as IdNode).Name == (binop.Right as IdNode).Name && binop.Op == OpType.NOTEQUAL
                )
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
