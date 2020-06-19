using ProgramTree;

namespace SimpleLang.Visitors
{
    internal class OptExprMultZero : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binOpNode)
        {
            base.VisitBinOpNode(binOpNode);
            var operationIsMult = binOpNode.Op == OpType.MULT;
            var leftIsZero = binOpNode.Left is IntNumNode && (binOpNode.Left as IntNumNode).Num == 0;
            var rightIsZero = binOpNode.Right is IntNumNode && (binOpNode.Right as IntNumNode).Num == 0;
            if (operationIsMult && (leftIsZero || rightIsZero))
            {
                ReplaceExpr(binOpNode, new IntNumNode(0));
            }
        }
    }
}
