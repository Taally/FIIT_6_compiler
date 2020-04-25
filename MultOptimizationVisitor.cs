using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{

    public class MultOptimizationVisitor : ChangeVisitor
    {
        public override void VisitBinOpNode(BinOpNode binOpNode)
        {
            base.VisitBinOpNode(binOpNode);
            bool operationIsMult = binOpNode.Op == OpType.MULT;
            bool leftIsZero = binOpNode.Left is IntNumNode && (binOpNode.Left as IntNumNode).Num == 0;
            bool rightIsZero = binOpNode.Right is IntNumNode && (binOpNode.Right as IntNumNode).Num == 0;
            if (operationIsMult && (leftIsZero || rightIsZero))
            {
                ReplaceExpr(binOpNode, new IntNumNode(0));                
            }                             
        }
    }    
}
