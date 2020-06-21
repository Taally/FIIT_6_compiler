using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprMultZero : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binOpNode)
        {
            base.VisitBinOpNode(binOpNode);
            var operationIsMult = binOpNode.Op == OpType.MULT;
            var leftIsZero = binOpNode.Left is IntNumNode intNumLeft && intNumLeft.Num == 0;
            var rightIsZero = binOpNode.Right is IntNumNode intNumRight && intNumRight.Num == 0;
            if (operationIsMult && (leftIsZero || rightIsZero))
            {
                ReplaceExpr(binOpNode, new IntNumNode(0));
            }
        }
    }
}
