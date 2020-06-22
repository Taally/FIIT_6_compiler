using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprMultDivByOne : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binOpNode && (binOpNode.Op == OpType.MULT || binOpNode.Op == OpType.DIV))
            {
                if (binOpNode.Left is IntNumNode intNumNodeLeft && intNumNodeLeft.Num == 1 &&
                    binOpNode.Op != OpType.DIV) // Do not replace "1 / a"
                {
                    ReplaceExpr(binOpNode, binOpNode.Right);
                }
                else
                if (binOpNode.Right is IntNumNode intNumNodeRight && intNumNodeRight.Num == 1)
                {
                    ReplaceExpr(binOpNode, binOpNode.Left);
                }
            }
        }
    }
}
