using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprFoldUnary : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            var left = binop.Left as UnOpNode;
            var right = binop.Right as UnOpNode;

            if (left != null && right != null && left.Op == right.Op
                && left.Op == OpType.NOT && left.Expr is IdNode idl)
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
            if (left != null && left.Op == OpType.NOT && left.Expr is IdNode idl2
                && binop.Right is IdNode idr2 && idl2.Name == idr2.Name)
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
                if (right != null && right.Op == OpType.NOT && right.Expr is IdNode idr3
                    && binop.Left is IdNode idl3 && idr3.Name == idl3.Name)
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
