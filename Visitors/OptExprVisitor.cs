using ProgramTree;

namespace SimpleLang.Visitors
{
    class OptExprVisitor : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binop)
                if (binop.Left is IdNode Left && binop.Right is IdNode Right && Left.Name == Right.Name &&
                (binop.Op == OpType.EQUAL || binop.Op == OpType.EQLESS || binop.Op == OpType.EQGREATER))
                {
                    ReplaceExpr(binop, new BoolValNode(true));
                }
        }
    }
}
