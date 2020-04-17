using System;
using System.Collections.Generic;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLang{
    public class Command{
        public string Result { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public string Op { get; set; }
        public string Label { get; set; }

        public Command(string result, string arg1, string arg2, string op, string label){
            Result = result;
            Arg1 = arg1;
            Arg2 = arg2;
            Op = op;
            Label = label;
        }

        public override string ToString() =>
            (Label != "" ? Label + ": " : "" ) + Result + " = " + Arg1 + " " + Op + " " + Arg2;
    }

    class ThreeAddrGen: AutoVisitor{
        private static int tmpNum = 0;
        public List<Command> table = new List<Command>();
        private static int tmpLabelNum = 0;

        string genTmpName(){
            string tmpName = "#tmp" + tmpNum.ToString();
            tmpNum += 1;
            return tmpName;
        }

        string getStringOp(OpType op){
            switch (op){
                case OpType.PLUS: return "+";
                case OpType.MINUS: return "-";
                case OpType.MULT: return "*";
                case OpType.DIV: return "/";
                case OpType.AND: return "and";
                case OpType.OR: return "or";
                case OpType.LESS: return "<";
                case OpType.EQLESS: return "<=";
                case OpType.GREATER: return ">";
                case OpType.EQGREATER: return ">=";
                case OpType.EQUAL:return "==";
                case OpType.NOTEQUAL: return "!=";

                default: return "";
            }
        }

        string genTmpLabel(){
            string tmpLabel = "L" + tmpLabelNum;
            tmpLabelNum += 1;
            return tmpLabel;
        }

        void genCommand(string result, string arg1, string arg2, string op, string label){
            table.Add(new Command(result, arg1, arg2, op, label));
        }

        string gen(ExprNode n){
            if (n is BinOpNode bin){
                string tmp1 = (bin.Left is IdNode idl) ? idl.Name
                    : ((bin.Left is IntNumNode inl) ? inl.Val.ToString()
                    : ((bin.Left is BoolValNode bvl) ? bvl.Val.ToString().ToLower()
                    : gen(bin.Left)));

                string tmp2 = (bin.Right is IdNode idr) ? idr.Name
                    : ((bin.Right is IntNumNode inr) ? inr.Val.ToString()
                    : ((bin.Right is BoolValNode bvr) ? bvr.Val.ToString().ToLower()
                    : gen(bin.Right)));

                string tmp = genTmpName();
                genCommand(tmp, tmp1, tmp2, getStringOp(bin.Op), "");
                return tmp;
            }
            #region
            /*else if (n is IntNumNode num)
            {
                string tmp = genTmpName();
                genCommand(tmp, num.Val.ToString(), "", "", "");
                return tmp;
            } else if (n is BoolValNode bn)
            {
                string tmp = genTmpName();
                genCommand(tmp, bn.Val.ToString().ToLower(), "", "", "");
                return tmp;
            }
            else if (n is IdNode id)
            {
                string tmp = genTmpName();
                genCommand(tmp, id.Name, "", "", "");
                return tmp;
            }*/
            //else if (n is ExprListNode exprlist)
            //{
            //    string tmp = genTmpName();
            //}
            #endregion
            throw new ArgumentException("Wrong expr type");
        }

        public override void VisitAssignNode(AssignNode n){
            if (n.Expr is IntNumNode node)
                genCommand(n.Id.Name, node.Val.ToString(), "", "", "");

             else if (n.Expr is BoolValNode bnode)
                genCommand(n.Id.Name, bnode.Val.ToString().ToLower(), "", "", "");

             else if (n.Expr is IdNode idnode)
                genCommand(n.Id.Name, idnode.Name, "", "", "");
            else{
                string tmp = gen(n.Expr);
                genCommand(n.Id.Name, tmp, "", "", "");
            }
        }
    }
}
