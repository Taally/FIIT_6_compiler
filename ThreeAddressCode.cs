using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    public class ThreeAddressCode : AutoVisitor
    {
        public List<Command> Code { get; set; }
        public static int Number { get; set; }

        public ThreeAddressCode()
        {
            Code = new List<Command>();
            Number = 0;
        }

        public string genTmpName()
        {
            Number += 1;
            return "#t" + Number;
        }

        public override void VisitAssignNode(AssignNode a)
        {
            string tmp = Gen(a.Expr);
            Code.Add(Command.GenCommand("assign", tmp, null, a.Id.Name, ""));
        }
       
        
        public string Gen(ExprNode ex)
        {
            if (ex.GetType() == typeof(BinOpNode))
            {
                var bin = (BinOpNode)ex;
                string tmp1 = Gen(bin.Left);
                string tmp2 = Gen(bin.Right);
                Command command;
                string tmp = genTmpName();
                command = Command.GenCommand(bin.Op.GetName(), tmp1, tmp2, tmp, "");                
                Code.Add(command);
                return tmp;
            }
            if (ex.GetType() == typeof(IdNode))
            {
                var id = (IdNode)ex;
                return id.Name;
            }
            if (ex.GetType() == typeof(IntNumNode))
            {
                var num = (IntNumNode)ex;
                return num.Num.ToString();
            }
            if (ex.GetType() == typeof(BoolNode))
            {
                var b = (BoolNode)ex;
                return b.Bool.ToString();
            }
            return null;
        }
        
        public void Print()
        {
            foreach (var command in Code)
            {
                Console.WriteLine(command.ToString());
            }                
        }

        public void ConsntantConvolution()
        {
            int count = Code.Count;
            for (int i = 0; i < count; i++)
            {
                double arg1, arg2;
                if (double.TryParse(Code[i].Arg1, out arg1) && double.TryParse(Code[i].Arg2, out arg2))
                {
                    switch (Code[i].Op)
                    {
                        case "==":
                            Code[i] = Command.GenCommand("assign", (arg1 == arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "!=":
                            Code[i] = Command.GenCommand("assign", (arg1 != arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case ">":
                            Code[i] = Command.GenCommand("assign", (arg1 > arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "<":
                            Code[i] = Command.GenCommand("assign", (arg1 < arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case ">=":
                            Code[i] = Command.GenCommand("assign", (arg1 >= arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "<=":
                            Code[i] = Command.GenCommand("assign", (arg1 <= arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "+":
                            Code[i] = Command.GenCommand("assign", (arg1 + arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "-":
                            Code[i] = Command.GenCommand("assign", (arg1 - arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "*":
                            Code[i] = Command.GenCommand("assign", (arg1 * arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "/":
                            Code[i] = Command.GenCommand("assign", (arg1 / arg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                    }
                }
                bool boolArg1, boolArg2;
                if (bool.TryParse(Code[i].Arg1, out boolArg1) && bool.TryParse(Code[i].Arg2, out boolArg2))
                    switch (Code[i].Op)
                    {
                        case "&":
                            Code[i] = Command.GenCommand("assign", (boolArg1 && boolArg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "|":
                            Code[i] = Command.GenCommand("assign", (boolArg1 || boolArg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "==":
                            Code[i] = Command.GenCommand("assign", (boolArg1 == boolArg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                        case "!=":
                            Code[i] = Command.GenCommand("assign", (boolArg1 != boolArg2).ToString(), "", Code[i].Result, Code[i].Label);
                            continue;
                    }
            }            
        }

        public void CheckAlgebraicIdentities()
        {
            int count = Code.Count;
            for ( int i = 0; i < count; i++)
            {
                //a - a == 0
                bool b;
                bool variablesAreNotBool = !bool.TryParse(Code[i].Arg1, out b) && !bool.TryParse(Code[i].Arg2, out b);
                if (variablesAreNotBool && Code[i].Arg1 == Code[i].Arg2 && Code[i].Op == "-")
                    Code[i] = Command.GenCommand("assign", "0", "", Code[i].Result, Code[i].Label);
                
                //Умножение на 1
                double arg1, arg2;
                bool arg1IsNumber = double.TryParse(Code[i].Arg1, out arg1);
                if (Code[i].Op == "*" && variablesAreNotBool && arg1IsNumber && arg1 == 1)
                    Code[i] = Command.GenCommand("assign", Code[i].Arg2, "", Code[i].Result, Code[i].Label);
                bool arg2IsNumber = double.TryParse(Code[i].Arg2, out arg2);
                if (Code[i].Op == "*" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
                    Code[i] = Command.GenCommand("assign", Code[i].Arg1, "", Code[i].Result, Code[i].Label);

                //Суммирование с 0                
                if (Code[i].Op == "+" && variablesAreNotBool && arg1IsNumber && arg1 == 0)
                    Code[i] = Command.GenCommand("assign", Code[i].Arg2, "", Code[i].Result, Code[i].Label);                
                if (Code[i].Op == "+" && variablesAreNotBool && arg2IsNumber && arg2 == 0)
                    Code[i] = Command.GenCommand("assign", Code[i].Arg1, "", Code[i].Result, Code[i].Label);

                //Умножение на 0
                if (Code[i].Op == "*" && variablesAreNotBool && (arg1IsNumber && arg1 == 0 || arg2IsNumber && arg2 == 0))
                    Code[i] = Command.GenCommand("assign", "0", "", Code[i].Result, Code[i].Label);

                //0 Делить на !0
                if (Code[i].Op == "/" && variablesAreNotBool && arg1IsNumber && arg1 == 0 && arg2IsNumber && arg2 != 0)
                    Code[i] = Command.GenCommand("assign", "0", "", Code[i].Result, Code[i].Label);
            }                        
        }
    }
}
