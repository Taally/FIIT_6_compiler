using ProgramTree;

namespace SimpleLang.Visitors
{
    internal class OptExprSumZero : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binOpNode)
        {
            base.VisitBinOpNode(binOpNode);
            var operationIsPlus = binOpNode.Op == OpType.PLUS;
            var leftIsZero = binOpNode.Left is IntNumNode && (binOpNode.Left as IntNumNode).Num == 0;
            var rightIsZero = binOpNode.Right is IntNumNode && (binOpNode.Right as IntNumNode).Num == 0;
            if (operationIsPlus && leftIsZero)
            {
                ReplaceExpr(binOpNode, binOpNode.Right);
            }
            if (operationIsPlus && rightIsZero)
            {
                ReplaceExpr(binOpNode, binOpNode.Left);
            }
        }
    }

}
