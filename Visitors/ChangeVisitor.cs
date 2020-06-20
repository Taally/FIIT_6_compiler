﻿using System;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class ChangeVisitor : AutoVisitor
    {
        public bool Changed { get; set; }

        public override void VisitStListNode(StListNode bl)
        {
            if (bl.Parent == null)
            {
                Changed = false;
            }
            base.VisitStListNode(bl);
        }

        public void ReplaceExpr(ExprNode from, ExprNode to)
        {
            var p = from.Parent;
            to.Parent = p;
            if (p.ExprChildren.Count > 0)
            {
                for (var i = 0; i < p.ExprChildren.Count; ++i)
                {
                    if (p.ExprChildren[i] == from)
                    {
                        p.ExprChildren[i] = to;
                        Changed = true;
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("Parent node doesn't contain expressions.");
            }
        }

        public void ReplaceStat(StatementNode from, StatementNode to)
        {
            var p = from.Parent;
            to.Parent = p;
            if (p.StatChildren.Count > 0)
            {
                for (var i = 0; i < p.StatChildren.Count; ++i)
                {
                    if (p.StatChildren[i] == from)
                    {
                        p.StatChildren[i] = to is BlockNode block
                            ? block.List.StatChildren.Count == 1
                                ? block.List.StatChildren[0] : block.List
                            : to;
                        Changed = true;
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("Parent node doesn't contain statements.");
            }
        }
    }
}
