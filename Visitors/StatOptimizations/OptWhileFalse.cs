using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptWhileFalseVisitor : ChangeVisitor
    {
        public override void PostVisit(Node nd)
        {
            if (!(nd is WhileNode n))
            {
                return;
            }

            if (n.Expr is BoolValNode bn && !bn.Val)
            {
                ReplaceStat(n, new EmptyNode());
            }
            else
            {
                n.Expr.Visit(this);
            }
        }
    }
}
