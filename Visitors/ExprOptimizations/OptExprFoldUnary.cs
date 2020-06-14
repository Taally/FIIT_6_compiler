using ProgramTree;

namespace SimpleLang.Visitors
{
    internal class OptExprFoldUnary : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            var left = binop.Left as UnOpNode;
            var right = binop.Right as UnOpNode;

            if (left != null && right != null && left.Op == right.Op && left.Expr is IdNode idl)
            {
                if (right.Expr is IdNode idr && idl.Name == idr.Name)
                {
                    if (binop.Op == OpType.EQUAL)
                    {
                        ReplaceExpr(binop, new BoolValNode(true));
                    }
                    if (binop.Op == OpType.NOTEQUAL)
                    {
                        ReplaceExpr(binop, new BoolValNode(false));
                    }
                }
            }
            else
            if (left != null && left.Op == OpType.NOT && left.Expr is IdNode
                && binop.Right is IdNode && (left.Expr as IdNode).Name == (binop.Right as IdNode).Name)
            {
                if (binop.Op == OpType.EQUAL)
                {
                    ReplaceExpr(binop, new BoolValNode(false));
                }
                if (binop.Op == OpType.NOTEQUAL)
                {
                    ReplaceExpr(binop, new BoolValNode(true));
                }
            }
            else
                if (right != null && right.Op == OpType.NOT && right.Expr is IdNode
                    && binop.Left is IdNode && (right.Expr as IdNode).Name == (binop.Left as IdNode).Name)
            {
                if (binop.Op == OpType.EQUAL)
                {
                    ReplaceExpr(binop, new BoolValNode(false));
                }
                if (binop.Op == OpType.NOTEQUAL)
                {
                    ReplaceExpr(binop, new BoolValNode(true));
                }
            }
            else
            {
                base.VisitBinOpNode(binop);
            }
        }
    }
}
