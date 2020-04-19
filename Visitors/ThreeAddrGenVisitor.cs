using System.Collections.Generic;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class Instruction
    {
        public string Label { get; }
        public string Operation { get; }
        public string Argument1 { get; }
        public string Argument2 { get; }
        public string Result { get; }

        public Instruction(string label, string operation, string argument1, string argument2, string result)
        {
            Label = label;
            Operation = operation;
            Argument1 = argument1;
            Argument2 = argument2;
            Result = result;
        }

        public override string ToString()
        {
            var label = Label != "" ? Label + ": " : "";
            switch (Operation)
            {
                case "assign":
                    return label + Result + " = " + Argument1;
                case "OR":
                case "AND":
                case "EQUAL":
                case "NOTEQUAL":
                case "LESS":
                case "GREATER":
                case "EQGREATER":
                case "EQLESS":
                case "PLUS":
                case "MINUS":
                case "MULT":
                case "DIV":
                    return $"{label}{Result} = {Argument1} {ConvertToMathNotation(Operation)} {Argument2}";
                default:
                    return $"label: {Label}; op {Operation}; arg1: {Argument1}; arg2: {Argument2}; res: {Result}";
            }
        }

        private string ConvertToMathNotation(string operation)
        {
            switch (operation)
            {
                case "OR":
                    return "or";
                case "AND":
                    return "and";
                case "EQUAL":
                    return "==";
                case "NOTEQUAL":
                    return "!=";
                case "LESS":
                    return "<";
                case "GREATER":
                    return ">";
                case "EQGREATER":
                    return ">=";
                case "EQLESS":
                    return "<=";
                case "PLUS":
                    return "+";
                case "MINUS":
                    return "-";
                case "MULT":
                    return "*";
                case "DIV":
                    return "/";
                default:
                    return operation;
            }
        }
    }

    class ThreeAddrGenVisitor : AutoVisitor
    {
        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public override void VisitAssignNode(AssignNode a)
        {
            string argument1 = Gen(a.Expr);
            //GenCommand(a.Id.Name + " = " + tmp);
            GenCommand("", "assign", argument1, "", a.Id.Name);
        }

        //public override void VisitVarListNode(VarListNode w)
        //{
        //    string tmp = "var ";
        //    for (int i = 0; i < w.vars.Count - 1; ++i)
        //        tmp += Gen(w.vars[i]) + ", ";
        //    tmp += Gen(w.vars[w.vars.Count - 1]);
        //    GenCommand(tmp);
        //}

        //public override void VisitIfElseNode(IfElseNode n) // ??
        //{
        //    string tmp = Gen(n.Expr);
        //    string L1 = GenTmpLabel();
        //    string L2 = GenTmpLabel();
        //    GenCommand("if " + tmp + " goto " + L1);
        //    n.FalseStat.Visit(this);
        //    GenCommand("L1: nop");
        //    n.TrueStat.Visit(this);
        //    GenCommand("L2: nop");
        //}

        int tmpInd = 0;
        string GenTmpName()
        {
            ++tmpInd;
            return "#t" + tmpInd;
        }

        int tmpLabelInd = 0;
        string GenTmpLabel()
        {
            ++tmpLabelInd;
            return "L" + tmpLabelInd;
        }

        void GenCommand(string label, string operation, string argument1, string argument2, string result)
        {
            //Console.WriteLine(s);
            Instructions.Add(new Instruction(label, operation, argument1, argument2, result));
        }

        string Gen(ExprNode ex)
        {
            if (ex.GetType() == typeof(BinOpNode))
            {
                var bin = (BinOpNode)ex;
                string argument1 = Gen(bin.Left);
                string argument2 = Gen(bin.Right);
                string result = GenTmpName();
                //GenCommand(tmp + " = " + tmp1 + " " + bin.Op + " " + tmp2);
                GenCommand("", bin.Op.ToString(), argument1, argument2, result);
                return result;
            }
            else if (ex.GetType() == typeof(IdNode))
            {
                var id = (IdNode)ex;
                return id.Name;
            }
            else if (ex.GetType() == typeof(IntNumNode))
            {
                var id = (IntNumNode)ex;
                return id.Num.ToString();
            }
            else if (ex.GetType() == typeof(BoolValNode))
            {
                var bl = (BoolValNode)ex;
                return bl.Val.ToString();
            }

            return null;
        }
    }
}
