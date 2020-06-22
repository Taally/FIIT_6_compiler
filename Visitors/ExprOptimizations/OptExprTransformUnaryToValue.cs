using System;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprTransformUnaryToValue : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is UnOpNode unOpNode)
            {
                if (unOpNode.Expr is IntNumNode num)
                {
                    var vForNum = unOpNode.Op == OpType.UNMINUS ? -1 * num.Num
                        : throw new ArgumentException("IntNumNode linked with UNMINUS");
                    ReplaceExpr(unOpNode, new IntNumNode(vForNum));
                }
                else
                if (unOpNode.Expr is BoolValNode b)
                {
                    var vForBool = unOpNode.Op == OpType.NOT ? !b.Val
                        : throw new ArgumentException("BoolValNode linked with NOT");
                    ReplaceExpr(unOpNode, new BoolValNode(vForBool));
                }
                else
                if (unOpNode.Expr is IdNode
                     && unOpNode.Parent is UnOpNode && (unOpNode.Parent as UnOpNode).Op == unOpNode.Op)
                {
                    if (unOpNode.Parent is UnOpNode parent && parent.Op == unOpNode.Op)
                    {
                        ReplaceExpr(parent, unOpNode.Expr);
                    }
                }
            }
        }
    }
}
