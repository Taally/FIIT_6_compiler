using ProgramTree;

namespace SimpleLang.Visitors
{
    internal class OptStatIfFalse : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // if (false) st1; else st2; => st2;
            if (n is IfElseNode ifNode)
            {
                if (ifNode.Expr is BoolValNode boolNode && boolNode.Val == false)
                {
                    if (ifNode.FalseStat != null)
                    {
                        ifNode.FalseStat.Visit(this);
                        ReplaceStat(ifNode, ifNode.FalseStat);
                    }
                    else
                    {
                        ReplaceStat(ifNode, new EmptyNode());
                    }
                }
            }
        }
    }
}
