using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprWithOperationsBetweenConsts : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binop)
            {
                if(binop.Left is IntNumNode lbn && binop.Right is IntNumNode rbn)
                {
                    switch (binop.Op)
                    {
                        case OpType.LESS:
                            ReplaceExpr(binop, new BoolValNode(lbn.Num < rbn.Num));
                            break;

                        case OpType.GREATER:
                            ReplaceExpr(binop, new BoolValNode(lbn.Num > rbn.Num));
                            break;
                        
                        case OpType.EQGREATER:
                            ReplaceExpr(binop, new BoolValNode(lbn.Num >= rbn.Num));
                            break;

                        case OpType.EQLESS:
                            ReplaceExpr(binop, new BoolValNode(lbn.Num <= rbn.Num));
                            break;
                        case OpType.NOTEQUAL:
                            ReplaceExpr(binop, new BoolValNode(lbn.Num != rbn.Num));
                            break;
                    }
                }
                if (binop.Left is BoolValNode left && binop.Right is BoolValNode right
                    && binop.Op == OpType.NOTEQUAL)
                {
                    ReplaceExpr(binop, new BoolValNode(left.Val != right.Val));
                    return;

                }
            }
        }
    }
}
