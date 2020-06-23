using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprFoldUnary : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binOpNode)
            {
                var left = binOpNode.Left as UnOpNode;
                var right = binOpNode.Right as UnOpNode;

                if (left != null && right != null && left.Op == right.Op
                    && left.Op == OpType.NOT && left.Expr is IdNode idl)
                {
                    if (right.Expr is IdNode idr && idl.Name == idr.Name)
                    {
                        if (binOpNode.Op == OpType.EQUAL)
                        {
                            ReplaceExpr(binOpNode, new BoolValNode(true));
                        }
                        else if (binOpNode.Op == OpType.NOTEQUAL)
                        {
                            ReplaceExpr(binOpNode, new BoolValNode(false));
                        }
                    }
                }
                else
                if (left != null && left.Op == OpType.NOT && left.Expr is IdNode idl2
                    && binOpNode.Right is IdNode idr2 && idl2.Name == idr2.Name)
                {
                    if (binOpNode.Op == OpType.EQUAL)
                    {
                        ReplaceExpr(binOpNode, new BoolValNode(false));
                    }
                    else if (binOpNode.Op == OpType.NOTEQUAL)
                    {
                        ReplaceExpr(binOpNode, new BoolValNode(true));
                    }
                }
                else
                if (right != null && right.Op == OpType.NOT && right.Expr is IdNode idr3
                        && binOpNode.Left is IdNode idl3 && idr3.Name == idl3.Name)
                {
                    if (binOpNode.Op == OpType.EQUAL)
                    {
                        ReplaceExpr(binOpNode, new BoolValNode(false));
                    }
                    else if (binOpNode.Op == OpType.NOTEQUAL)
                    {
                        ReplaceExpr(binOpNode, new BoolValNode(true));
                    }
                }
            }
        }
    }
}
