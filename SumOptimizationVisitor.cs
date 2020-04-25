using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{

    public class SumOptimizationVisitor : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binOpNode)
        {
            base.VisitBinOpNode(binOpNode);
            bool operationIsPlus = binOpNode.Op == OpType.PLUS;
            bool leftIsZero = binOpNode.Left is IntNumNode && (binOpNode.Left as IntNumNode).Num == 0;
            bool rightIsZero = binOpNode.Right is IntNumNode && (binOpNode.Right as IntNumNode).Num == 0;
            if (operationIsPlus && leftIsZero)
            {
                ReplaceExpr(binOpNode, binOpNode.Right);
            }
            if (operationIsPlus && rightIsZero)
            {
                ReplaceExpr(binOpNode, binOpNode.Left);
            }
        }
    }
}