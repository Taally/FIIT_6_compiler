using System;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class ChangeVisitor : AutoVisitor
    {
        public void ReplaceExpr(ExprNode from, ExprNode to)
        {
            var p = from.Parent;
            to.Parent = p;
            if (p is AssignNode assn)
            {
                assn.Expr = to;
            }
            else if (p is BinOpNode binopn)
            {
                if (binopn.Left == from)
                    binopn.Left = to;
                else if (binopn.Right == from)
                    binopn.Right = to;
            }
            else if (p is IfElseNode ifNode)
            {
                ifNode.Expr = to;
            }
            else if (p is BlockNode)
            {
                throw new Exception("Parent node doesn't contain expressions.");
            }
        }

        public void ReplaceStat(StatementNode from, StatementNode to)
        {
            var p = from.Parent;
            if (p is AssignNode || p is ExprNode)
            {
                throw new Exception("Parent node doesn't contain statements.");
            }
            to.Parent = p;
            if (p is BlockNode bln)
            {
                for (var i = 0; i < bln.List.StList.Count; i++)
                    if (bln.List.StList[i] == from)
                        bln.List.StList[i] = to;
            }
            else if (p is StListNode stList)
            {
                for (int i = 0; i < stList.StList.Count; ++i)
                    if (stList.StList[i] == from)
                        stList.StList[i] = to;
            }
            else if (p is IfElseNode ifNode)
            {
                if (ifNode.TrueStat == from)
                    ifNode.TrueStat = to;
                else if (ifNode.FalseStat == from)
                    ifNode.FalseStat = to;
            }
        }
    }
}
