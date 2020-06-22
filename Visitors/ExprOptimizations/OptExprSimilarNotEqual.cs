using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprSimilarNotEqual : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binOpNode &&
                (binOpNode.Op == OpType.GREATER || binOpNode.Op == OpType.LESS || binOpNode.Op == OpType.NOTEQUAL)
                &&
                // Для цифр и значений bool:
                (binOpNode.Left is IntNumNode inl && binOpNode.Right is IntNumNode inr && inl.Num == inr.Num
                || binOpNode.Left is BoolValNode bvl && binOpNode.Right is BoolValNode bvr && bvl.Val == bvr.Val
                // Для переменных:
                || binOpNode.Left is IdNode idl && binOpNode.Right is IdNode idr && idl.Name == idr.Name))
            {
                ReplaceExpr(binOpNode, new BoolValNode(false));
            }
        }
    }
}
