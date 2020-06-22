using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprFoldUnary : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (binop.Op == OpType.EQUAL || binop.Op == OpType.NOTEQUAL)
            {
                if (binop.Left is UnOpNode left && binop.Right is UnOpNode right
                    && left.Op == right.Op && left.Op == OpType.NOT
                    && left.Expr is IdNode idl && right.Expr is IdNode idr && idl.Name == idr.Name)
                {
                    ReplaceExpr(binop, new BoolValNode(binop.Op == OpType.EQUAL));
                }
                else if (binop.Left is UnOpNode lleft && lleft.Expr is IdNode luexpr
                    && binop.Right is IdNode rexpr && luexpr.Name == rexpr.Name)
                {
                    ReplaceExpr(binop, new BoolValNode(binop.Op == OpType.NOTEQUAL));
                }
                else if (binop.Right is UnOpNode rright && rright.Expr is IdNode ruexpr
                    && binop.Left is IdNode lexpr && ruexpr.Name == lexpr.Name)
                {
                    ReplaceExpr(binop, new BoolValNode(binop.Op == OpType.NOTEQUAL));
                }
            }
            else
            {
                base.VisitBinOpNode(binop);
            }
        }
    }
}
