using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprEqualBoolNum : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // 5 == 5 -> true
            // 5 == 6 -> false
            // false == false -> true
            // true == false -> false
            if (n is BinOpNode binop && binop.Op == OpType.EQUAL)
            {
                if (binop.Left is IntNumNode intValLeft && binop.Right is IntNumNode intValRight)
                {
                    ReplaceExpr(binop, new BoolValNode(intValLeft.Num == intValRight.Num));
                }
                else if (binop.Left is BoolValNode boolValLeft && binop.Right is BoolValNode boolValRight)
                {
                    ReplaceExpr(binop, new BoolValNode(boolValLeft.Val == boolValRight.Val));
                }
            }
        }
    }
}
