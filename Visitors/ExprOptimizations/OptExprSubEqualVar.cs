using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprSubEqualVar : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // a - a => 0
            if (n is BinOpNode binop && binop.Op == OpType.MINUS
                && binop.Left is IdNode id1 && binop.Right is IdNode id2 && id1.Name == id2.Name)
            {
                if (id1.Name == id2.Name)
                {
                    ReplaceExpr(binop, new IntNumNode(0));
                }
                else
                {
                    base.VisitBinOpNode(binop);
                }
            }
        }
    }
}
