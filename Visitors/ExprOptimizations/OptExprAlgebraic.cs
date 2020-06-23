using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprAlgebraic : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // Algebraic expressions of the form: 2 * 3 => 6
            if (n is BinOpNode binop && binop.Left is IntNumNode intNumLeft && binop.Right is IntNumNode intNumRight)
            {
                var result = new IntNumNode(0);
                switch (binop.Op)
                {
                    case OpType.PLUS:
                        result.Num = intNumLeft.Num + intNumRight.Num;
                        break;
                    case OpType.MINUS:
                        result.Num = intNumLeft.Num - intNumRight.Num;
                        break;
                    case OpType.DIV:
                        result.Num = intNumLeft.Num / intNumRight.Num;
                        break;
                    case OpType.MULT:
                        result.Num = intNumLeft.Num * intNumRight.Num;
                        break;
                    default:
                        return;
                }
                ReplaceExpr(binop, result);
            }
        }
    }
}
