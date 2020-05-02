using ProgramTree;

namespace SimpleLang.Visitors
{
    class IfNullElseNull : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is IfElseNode ifn)
            {
                if ((ifn.FalseStat is EmptyNode || ifn.FalseStat == null)
                    && (ifn.TrueStat is EmptyNode || ifn.TrueStat == null))
                    ReplaceStat(ifn, new EmptyNode());
            }
        }
    }
}
