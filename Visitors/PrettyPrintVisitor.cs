using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class PrettyPrintVisitor: Visitor
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
        public override void VisitBoolValNode(BoolValNode n)
        {
            Text += n.Val.ToString();
        }

        private string getSymbol(OpType t)
        {
            switch (t)
            {
                case OpType.PLUS: return "+";
                case OpType.MINUS: return "-";
                case OpType.MULT: return "*";
                case OpType.DIV: return "/";
                case OpType.AND: return "and";
                case OpType.OR: return "or";
                case OpType.EQUAL: return "==";
                case OpType.NOTEQUAL: return "!=";
                case OpType.LESS: return "<";
                case OpType.EQLESS: return "<=";
                case OpType.GREATER: return ">";
                case OpType.EQGREATER: return ">=";
                default: return "unknown";
            }
        }
        public override void VisitBinOpNode(BinOpNode binop) 
        {
            Text += "(";
            binop.Left.Visit(this);
            Text += " " + getSymbol(binop.Op) + " ";
            binop.Right.Visit(this);
            Text += ")";
        }
        public override void VisitAssignNode(AssignNode a) 
        {
            Text += IndentStr();
            a.Id.Visit(this);
            Text += " := ";
            a.Expr.Visit(this);
        }
        public override void VisitWhileNode(WhileNode n) 
        {
            Text += IndentStr() + "while ";
            n.Expr.Visit(this);
            Text += Environment.NewLine;
            n.Stat.Visit(this);
        }
        public override void VisitForNode(ForNode n)
        {
            Text += IndentStr() + "for ";
            n.Id.Visit(this);
            Text += "=";
            n.From.Visit(this);
            Text += ", ";
            n.To.Visit(this);
            Text += Environment.NewLine;
            IndentPlus();
            n.Stat.Visit(this);
            IndentMinus();
        }
        public override void VisitBlockNode(BlockNode bl) 
        {
            Text += IndentStr() + "{" + Environment.NewLine;
            IndentPlus();

            var Count = bl.StList.Count;

            if (Count>0)
                if (bl.StList[0] != null)
                    bl.StList[0].Visit(this);
            for (var i = 1; i < Count; i++)
            {
                Text += ';';
                if (!(bl.StList[i] is EmptyNode))
                    Text += Environment.NewLine;
                bl.StList[i].Visit(this);
            }
            IndentMinus();
            Text += Environment.NewLine + IndentStr() + "}";
        }
        public override void VisitPrintNode(PrintNode n) 
        {
            Text += IndentStr() + "print(";
            n.exprList.Visit(this);
            Text += ")";
        }
        public override void VisitInputNode(InputNode n)
        {
            Text += IndentStr() + "input(";
            n.Ident.Visit(this);
            Text += ")";
        }
        public override void VisitVarListNode(VarListNode w) 
        {
            Text += IndentStr() + "var " + w.vars[0].Name;
            for (int i = 1; i < w.vars.Count; i++)
                Text += ',' + w.vars[i].Name;
        }
        public override void VisitExprListNode(ExprListNode n)
        {
            n.exprList[0].Visit(this);
            for (int i = 1; i < n.exprList.Count; i++)
            {
                Text += ',';
                n.exprList[i].Visit(this);
            }
        }
        public override void VisitIfElseNode(IfElseNode n)
        {
            Text += IndentStr() + "if ";
            n.Expr.Visit(this);
            Text += Environment.NewLine;
            IndentPlus();
            n.TrueStat.Visit(this);
            IndentMinus();
            Text += Environment.NewLine;
            if (n.FalseStat != null)
            {
                Text += IndentStr() + "else";
                Text += Environment.NewLine;
                IndentPlus();
                n.FalseStat.Visit(this);
                IndentMinus();
                Text += Environment.NewLine;
            }
        }
        public override void VisitEmptyNode(EmptyNode n)
        {
        }

        public override void VisitGotoNode(GotoNode n)
        {
            Text += IndentStr() + "goto ";
            n.Label.Visit(this);
        }
        public override void VisitLabelStatementNode(LabelStatementNode n)
        {
            Text += IndentStr();
            n.Label.Visit(this);
            Text += ":" + Environment.NewLine;
            n.Stat.Visit(this);
        }

    }
}
