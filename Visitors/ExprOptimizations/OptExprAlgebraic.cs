using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprAlgebraic : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // Algebraic expressions of the form: 2 * 3 => 6
            if (n is BinOpNode binop && binop.Left is IntNumNode && binop.Right is IntNumNode)
            {
                if (binop.Left is IntNumNode && binop.Right is IntNumNode)
                {
                    var result = new IntNumNode(0);
                    switch (binop.Op)
                    {
                        case OpType.PLUS:
                            result.Num = (binop.Left as IntNumNode).Num + (binop.Right as IntNumNode).Num;
                            break;
                        case OpType.MINUS:
                            result.Num = (binop.Left as IntNumNode).Num - (binop.Right as IntNumNode).Num;
                            break;
                        case OpType.DIV:
                            result.Num = (binop.Left as IntNumNode).Num / (binop.Right as IntNumNode).Num;
                            break;
                        case OpType.MULT:
                            result.Num = (binop.Left as IntNumNode).Num * (binop.Right as IntNumNode).Num;
                            break;
                        default:
                            return;
                    }
                    ReplaceExpr(binop, result);
                }
                else
                {
                    base.VisitBinOpNode(binop);
                }
            }
        }
    }
}
