using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptAssignEquality : ChangeVisitor
    {
        public override void VisitAssignNode(AssignNode n)
        {
            if (n.Expr is IdNode idn && n.Id.Name == idn.Name)
            {
                ReplaceStat(n, new EmptyNode());
            }
        }
    }
}
