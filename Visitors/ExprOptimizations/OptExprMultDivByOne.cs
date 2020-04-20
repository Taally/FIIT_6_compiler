using ProgramTree;

namespace SimpleLang.Visitors
{
    class OptExprMultDivByOne : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binop)
        {
            switch (binop.Op)
            {
                case OpType.MULT:
                    if (binop.Left is IntNumNode && (binop.Left as IntNumNode).Num == 1)
                    {
                        binop.Right.Visit(this);
                        ReplaceExpr(binop, binop.Right);
                    }
                    else if (binop.Right is IntNumNode && (binop.Right as IntNumNode).Num == 1)
                    {
                        binop.Left.Visit(this);
                        ReplaceExpr(binop, binop.Left);

                    }
                    else base.VisitBinOpNode(binop);
                    break;

                case OpType.DIV:
                    if (binop.Right is IntNumNode && (binop.Right as IntNumNode).Num == 1)
                    {
                        binop.Left.Visit(this);
                        ReplaceExpr(binop, binop.Left);
                    }
                    break;

                default:
                    base.VisitBinOpNode(binop);
                    break;
            }
        }
    }
}
