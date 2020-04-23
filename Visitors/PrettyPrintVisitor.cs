using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class PrettyPrintVisitor : Visitor
    {
        public string Text = "";
        private int Indent = 0;

        private string IndentStr()
        {
            return new string(' ', Indent);
        }
        private void IndentPlus()
        {
            Indent += 2;
        }
        private void IndentMinus()
        {
            Indent -= 2;
        }
        public override void VisitIdNode(IdNode id)
        {
            Text += id.Name;
        }
        public override void VisitIntNumNode(IntNumNode num)
        {
            Text += num.Num.ToString();
        }

        private string GetOp(OpType t)
        {
            switch (t)
            {
                case OpType.OR:
                    return "or";
                case OpType.AND:
                    return "and";
                case OpType.EQUAL:
                    return "==";
                case OpType.NOTEQUAL:
                    return "!=";
                case OpType.GREATER:
                    return ">";
                case OpType.LESS:
                    return "<";
                case OpType.EQGREATER:
                    return ">=";
                case OpType.EQLESS:
                    return "<=";
                case OpType.PLUS:
                    return "+";
                case OpType.MINUS:
                    return "-";
                case OpType.MULT:
                    return "*";
                case OpType.DIV:
                    return "/";
            }
            throw new ArgumentException();
        }

        public override void VisitBinOpNode(BinOpNode binop)
        {
            Text += "(";
            binop.Left.Visit(this);
            Text += " " + GetOp(binop.Op) + " ";
            binop.Right.Visit(this);
            Text += ")";
        }
        public override void VisitAssignNode(AssignNode a)
        {
            Text += IndentStr();
            a.Id.Visit(this);
            Text += " = ";
            a.Expr.Visit(this);
            Text += ";";
        }

        public override void VisitBlockNode(BlockNode bl)
        {
            //Text += IndentStr() + "{" + Environment.NewLine;
            Text += "{" + Environment.NewLine;
            IndentPlus();
            bl.List.Visit(this);
            IndentMinus();
            Text += Environment.NewLine + IndentStr() + "}";
        }

        public override void VisitStListNode(StListNode bl)
        {
            var Count = bl.StatChildren.Count;
            if (Count > 0)
                bl.StatChildren[0].Visit(this);
            for (var i = 1; i < Count; i++)
            {
                Text += Environment.NewLine;
                bl.StatChildren[i].Visit(this);
            }
        }

        public override void VisitVarListNode(VarListNode w)
        {
            Text += IndentStr() + "var " + w.vars[0].Name;
            for (int i = 1; i < w.vars.Count; i++)
                Text += ", " + w.vars[i].Name;
            Text += ";";
        }

        public override void VisitForNode(ForNode f)
        {
            Text += IndentStr() + "for ";
            f.Id.Visit(this);
            Text += " = ";
            f.From.Visit(this);
            Text += ", ";
            f.To.Visit(this);
            if (f.Stat is BlockNode)
            {
                Text += " ";
                f.Stat.Visit(this);
            }
            else
            {
                IndentPlus();
                Text += Environment.NewLine;
                f.Stat.Visit(this);
                IndentMinus();
            }
        }

        public override void VisitWhileNode(WhileNode w)
        {
            Text += IndentStr() + "while ";
            w.Expr.Visit(this);

            if (w.Stat is BlockNode)
            {
                Text += " ";
                w.Stat.Visit(this);
            }
            else
            {
                IndentPlus();
                Text += Environment.NewLine;
                w.Stat.Visit(this);
                IndentMinus();
            }
        }

        public override void VisitLabelstatementNode(LabelStatementNode l)
        {
            Text += IndentStr();
            l.Label.Visit(this);
            Text += ": ";
            l.Stat.Visit(this);
        }

        public override void VisitIfElseNode(IfElseNode i)
        {
            Text += IndentStr() + "if ";
            i.Expr.Visit(this);

            if (i.TrueStat is BlockNode)
            {
                Text += " ";
                i.TrueStat.Visit(this);
            }
            else
            {
                IndentPlus();
                Text += Environment.NewLine;
                i.TrueStat.Visit(this);
                IndentMinus();
            }
            if (i.FalseStat == null) return;
            Text += Environment.NewLine + IndentStr() + "else";
            if (i.FalseStat is BlockNode)
            {
                Text += " ";
                i.FalseStat.Visit(this);
            }
            else
            {
                IndentPlus();
                Text += Environment.NewLine;
                i.FalseStat.Visit(this);
                IndentMinus();
            }
        }

        public override void VisitGotoNode(GotoNode g)
        {
            Text += IndentStr() + "goto ";
            g.Label.Visit(this);
            Text += ";";
        }

        public override void VisitPrintNode(PrintNode p)
        {
            Text += IndentStr() + "print(";
            p.ExprList.Visit(this);
            Text += ");";
        }

        public override void VisitExprListNode(ExprListNode e)
        {
            e.ExprChildren[0].Visit(this);
            for (var i = 1; i < e.ExprChildren.Count; ++i)
            {
                Text += ", ";
                e.ExprChildren[i].Visit(this);
            }

        }

        public override void VisitInputNode(InputNode i)
        {
            Text += IndentStr() + "input(";
            i.Ident.Visit(this);
            Text += ");";
        }

        public override void VisitBoolValNode(BoolValNode b)
        {
            Text += b.Val.ToString().ToLower();
        }
    }
}