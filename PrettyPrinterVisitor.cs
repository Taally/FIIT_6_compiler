using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    class PrettyPrinterVisitor : Visitor
    {
        public string Text = "";
        private int ident = 0;
        private string IdentStr() => new string(' ', ident);
        private void IdentPlus() => ident += 2;
        private void IdentMinus() => ident -= 2;
        public override void VisitIdNode(IdNode id) => Text += id.Name;
        public override void VisitIntNumNode(IntNumNode num) => Text += num.Num.ToString();
        public override void VisitBoolNode(BoolNode b) => Text += b.Bool.ToString().ToUpper();
        public override void VisitBinOpNode(BinOpNode binop)
        {
            Text += "(";
            binop.Left.Visit(this);
            string operation = "";
            switch (binop.Op)
            {
                case OpType.AND: operation = " and "; break;
                case OpType.DIV: operation = " / "; break;
                case OpType.EQGREATER: operation = " >= "; break;
                case OpType.EQLESS: operation = " <= "; break;
                case OpType.EQUAL: operation = " == "; break;
                case OpType.GREATER: operation = " > "; break;
                case OpType.LESS: operation = " < "; break;
                case OpType.MINUS: operation = " - "; break;
                case OpType.MULT: operation = " * "; break;
                case OpType.NOTEQUAL: operation = " != "; break;
                case OpType.OR: operation = " or "; break;
                case OpType.PLUS: operation = " + "; break;
            }
            if (operation == "") throw new InvalidOperationException("Операция не определена" + this.ToString());
            Text += operation;
            binop.Right.Visit(this);
            Text += ")";
        }
        public override void VisitAssignNode(AssignNode aNode)
        {
            Text += IdentStr();
            aNode.Id.Visit(this);
            Text += " = ";
            aNode.Expr.Visit(this);
            Text += ";";
        }
        public override void VisitForNode(ForNode forNode)
        {
            Text += IdentStr() + "for ";
            forNode.Id.Visit(this);
            Text += " = ";
            forNode.From.Visit(this);
            Text += ", ";
            forNode.To.Visit(this);
            Text += Environment.NewLine;
            if (forNode.Stat is BlockNode)
                forNode.Stat.Visit(this);
            else
            {
                IdentPlus();                
                forNode.Stat.Visit(this);
                IdentMinus();
            }
        }
        public override void VisitWhileNode(WhileNode whileNode)
        {
            Text += IdentStr() + "while ";
            whileNode.Expr.Visit(this);

            Text += Environment.NewLine;
            if (whileNode.Stat is BlockNode)
                whileNode.Stat.Visit(this);
            else
            {
                IdentPlus();                
                whileNode.Stat.Visit(this);
                IdentMinus();
            }
        }
        public override void VisitIfNode(IfNode ifNode)
        {
            Text += IdentStr() + "if ";
            ifNode.Expr.Visit(this);
            
            Text += Environment.NewLine;
            if (ifNode.StatTrue is BlockNode)
                ifNode.StatTrue.Visit(this);
            else
            {
                IdentPlus();
                ifNode.StatTrue.Visit(this);
                IdentMinus();                               
            }
            if (ifNode.StatFalse == null) return;
            Text += Environment.NewLine + IdentStr() + "else";
            Text += Environment.NewLine;
            if (ifNode.StatFalse is BlockNode)
            {                
                ifNode.StatFalse.Visit(this);
            }
            else
            {
                IdentPlus();                
                ifNode.StatFalse.Visit(this);
                IdentMinus();
            }
        }
        public override void VisitGotoNode(GotoNode gotoNode)
        {
            Text += IdentStr() + "goto ";
            gotoNode.Label.Visit(this);
            Text += ";";
        }
        public override void VisitPrintNode(PrintNode printNode)
        {
            Text += IdentStr() + "print (";
            printNode.ExprList.Visit(this);
            Text += ");";
        }
        public override void VisitInputNode(InputNode inputNode)
        {
            Text += IdentStr() + "input (";
            inputNode.Id.Visit(this);
            Text += ");";
        }
        public override void VisitExprListNode(ExprListNode exprListNode)
        {
            exprListNode.ExprList[0].Visit(this);
            for (var i = 1; i < exprListNode.ExprList.Count; ++i)
            {
                Text += ", ";
                exprListNode.ExprList[i].Visit(this);
            }
        }
        public override void VisitVarListNode(VarListNode varListNode)
        {
            Text += IdentStr() + "var " + varListNode.VarList[0].Name;
            for (int i = 1; i < varListNode.VarList.Count; i++)
                Text += ", " + varListNode.VarList[i].Name;
            Text += ";";
        }
        public override void VisitBlockNode(BlockNode block)
        {
            Text += IdentStr() + "{" + Environment.NewLine;
            IdentPlus();
            var count = block.StList.Count;
            if (count > 0) block.StList[0].Visit(this);
            for (var i = 1; i < count; ++i)
            {
                Text += Environment.NewLine;
                block.StList[i].Visit(this);
            }
            IdentMinus();
            Text += Environment.NewLine + IdentStr() + "}";
        }
        public override void VisitLabelStatementNode(LabelStatementNode labelStatementNode)
        {
            Text += IdentStr();
            labelStatementNode.Label.Visit(this);
            Text += ": ";
            labelStatementNode.Stat.Visit(this);
        }
    }
}
