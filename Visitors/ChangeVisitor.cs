using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors{
    class ChangeVisitor: AutoVisitor{
        public void ReplaceExpr(ExprNode from, ExprNode to){
            var p = from.Parent;
            to.Parent = p;
            if (p is AssignNode assn)
                assn.Expr = to;
             else if (p is BinOpNode binopn){
                if (binopn.Left == from)
                    binopn.Left = to;
                else if (binopn.Right == from)
                    binopn.Right = to;
             } else if (p is BlockNode)
                throw new Exception("Parent node does not contain expressions");
        }

        public void ReplaceStat(StatementNode from, StatementNode to){
            var p = from.Parent;
            if (p is AssignNode || p is ExprNode)
                throw new Exception("Parent node does not contain statements");

            to.Parent = p;
            if (p is BlockNode bln){
                for (var i = 0; i < bln.List.StList.Count - 1; ++i)
                    if (bln.List.StList[i] == from){
                        bln.List.StList[i] = to;
                        break;
                    }
            } else if (p is IfElseNode ifn){
                if (ifn.TrueStat == from)
                    ifn.TrueStat = to;
                else if (ifn.FalseStat == from)
                    ifn.FalseStat = to;
            } else if (p is StListNode sln){
                for (var i = 0; i < sln.StList.Count; ++i)
                    if (sln.StList[i] == from){
                        sln.StList[i] = to;
                        break;
                    }
            }
        }
    }
}