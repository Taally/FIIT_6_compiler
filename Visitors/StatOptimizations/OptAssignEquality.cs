using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptAssignEquality : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is AssignNode assignNode && assignNode.Expr is IdNode idn && assignNode.Id.Name == idn.Name)
            {
                ReplaceStat(assignNode, new EmptyNode());
            }
        }
    }
}
