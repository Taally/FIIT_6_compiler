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
        public override void VisitIntNumNode(IntNumNode node) 
        {
            Text += node.Num.ToString();
        }
        public override void VisitBoolValNode(BoolValNode node)
        {
            Text += node.Val.ToString();
        }
        public override void VisitBinOpNode(BinOpNode node) 
        {
            Text += "(";
            node.Left.Visit(this);
            Text += " " + node.Op + " ";
            node.Right.Visit(this);
            Text += ")";
        }
        public override void VisitAssignNode(AssignNode node) 
        {
            Text += IndentStr();
            node.Id.Visit(this);
            Text += " = ";
            node.Expr.Visit(this);
        }
        public override void VisitWhileNode(WhileNode node) 
        {
            Text += IndentStr() + "while ";
            node.Expr.Visit(this);
            Text += Environment.NewLine;
            node.Stat.Visit(this);
        }
        public override void VisitForNode(ForNode node)
        {
            Text += IndentStr() + "for ";
            node.Ident.Visit(this);
            Text += " = ";
            node.Start.Visit(this);
            Text += ", ";
            node.Finish.Visit(this);
            Text += Environment.NewLine;
            IndentPlus();
            node.Stat.Visit(this);
            IndentMinus();
        }
        public override void VisitBlockNode(BlockNode bl) 
        {
            Text += IndentStr() + "{" + Environment.NewLine;
            IndentPlus();

            var Count = bl.StList.Count;

            if (Count>0)
                bl.StList[0].Visit(this);
            for (var i = 1; i < Count; i++)
            {
                Text += ";";
                if (!(bl.StList[i] is EmptyNode))
                    Text += Environment.NewLine;
                bl.StList[i].Visit(this);
            }
            IndentMinus();
            Text += Environment.NewLine + IndentStr() + "}";
        }
        public override void VisitPrintNode(PrintNode node) 
        {
            Text += IndentStr() + "print(";
            node.exprList.Visit(this);
            Text += ")";
        }
        public override void VisitExprListNode(ExprListNode node)
        {
            node.exprList[0].Visit(this);
            for (int i = 1; i < node.exprList.Count; ++i)
            {
                Text += ", ";
                node.exprList[i].Visit(this);
            }
        }
        public override void VisitInputNode(InputNode node)
        {
            Text += IndentStr() + "input(";
            node.Ident.Visit(this);
            Text += ")";
        }
        public override void VisitVarListNode(VarListNode node) 
        {
            Text += IndentStr() + "var " + node.vars[0].Name;
            for (int i = 1; i < node.vars.Count; i++)
                Text += ", " + node.vars[i].Name;
        }
        public override void VisitIfElseNode(IfElseNode node)
        {
            Text += IndentStr() + "if ";
            node.Expr.Visit(this);
            Text += " then";
            Text += Environment.NewLine;
            IndentPlus();
            node.TrueStat.Visit(this);
            IndentMinus();
            if (node.FalseStat != null)
            {
                Text += Environment.NewLine;
                Text += IndentStr() + "else";
                Text += Environment.NewLine;
                IndentPlus();
                node.FalseStat.Visit(this);
                IndentMinus();
            }
        }
    }
}
