using ProgramTree;

namespace SimpleLang.Visitors
{
    class OptStatVisitor : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is IfElseNode ifNode)
                if (ifNode.Expr is BoolValNode boolNode && boolNode.Val)
                {
                    if (ifNode.TrueStat != null)
                        ifNode.TrueStat.Visit(this);
                    ReplaceStat(ifNode, ifNode.TrueStat);
                }
        }
    }
}
