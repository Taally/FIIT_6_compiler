using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    public class ChangeVisitor : AutoVisitor
    {
        public void ReplaceExpr(ExprNode from, ExprNode to)
        {
            var p = from.Parent;
            to.Parent = p;
            if (p is AssignNode assn)
            {
                assn.Expr = to;
                return;
            }
            if (p is BinOpNode binopn)
            {
                if (binopn.Left == from) // Поиск подузла в Parent                    
                    binopn.Left = to;
                else
                    if (binopn.Right == from)
                    binopn.Right = to;
                return;
            }
            if (p is WhileNode cycleWhile)
            {                
                cycleWhile.Expr = to;
                return;
            }
            if (p is IfNode ifNode)
            {
                ifNode.Expr = to;
                return;
            }
            if (p is ForNode forNode)
            {
                if (from == forNode.From)
                    forNode.From = to;
                if (from == forNode.To)
                    forNode.To = to;
                return;
            }
            if (p is ExprListNode exprListNode)
            {
                int count = exprListNode.ExprList.Count;
                for (int i = 0; i < count; i++)
                    if (from == exprListNode.ExprList[i])
                    {
                        exprListNode.ExprList[i] = to;
                        return;
                    }
            }
            if (p is BlockNode)                      
                throw new Exception("Родительский узел не содержит выражений");                    
        }
    }
}
