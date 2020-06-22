using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprAlgebraic : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // Algebraic expressions of the form: 2 * 3 => 6
            if (n is BinOpNode binop && binop.Left is IntNumNode left && binop.Right is IntNumNode right)
            {
                if (binop.Left is IntNumNode && binop.Right is IntNumNode)
                {
                    var result = new IntNumNode(0);
                    switch (binop.Op)
                    {
                        case OpType.PLUS:
                            result.Num = left.Num + right.Num;
                            break;
                        case OpType.MINUS:
                            result.Num = left.Num - right.Num;
                            break;
                        case OpType.DIV:
                            result.Num = left.Num / right.Num;
                            break;
                        case OpType.MULT:
                            result.Num = left.Num * right.Num;
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
