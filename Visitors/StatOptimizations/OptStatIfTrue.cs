using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptStatIfTrue : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // if (true) st1; else st2
            if (n is IfElseNode ifNode && ifNode.Expr is BoolValNode boolNode && boolNode.Val)
            {
                ReplaceStat(ifNode, ifNode.TrueStat);
            }
        }
    }
}
