using ProgramTree;

namespace SimpleLang.Visitors
{
    class OptStatIfTrue : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // if (true) st1; else st2
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
