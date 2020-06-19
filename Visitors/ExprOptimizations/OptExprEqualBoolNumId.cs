using ProgramTree;

namespace SimpleLang.Visitors
{
    internal class OptExprEqualBoolNumId : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // a == a -> true
            // 5 == 5 -> true
            // 5 == 6 -> false
            // false == false -> true
            // true == false -> false
            if (n is BinOpNode binop)
            {
                switch (binop.Op)
                {
                    case OpType.EQUAL:
                        if (binop.Left is IntNumNode leftNode2 && binop.Right is IntNumNode rightNode2)
                        {
                            ReplaceExpr(binop, new BoolValNode(leftNode2.Num == rightNode2.Num));
                            break;
                        }

                        if (binop.Left is BoolValNode leftNode3 && binop.Right is BoolValNode rightNode3)
                        {
                            ReplaceExpr(binop, new BoolValNode(leftNode3.Val == rightNode3.Val));
                        }

                        break;
                }
            }
        }
    }
}
