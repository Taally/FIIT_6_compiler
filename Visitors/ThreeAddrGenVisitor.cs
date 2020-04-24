using System.Collections.Generic;
using ProgramTree;

namespace SimpleLang.Visitors
{

    class ThreeAddrGenVisitor : AutoVisitor
    {
        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public override void VisitLabelstatementNode(LabelStatementNode l)
        {
            var instructionIndex = Instructions.Count;
            l.Stat.Visit(this);
            Instructions[instructionIndex].Label = l.Label.Num.ToString();
        }

        public override void VisitAssignNode(AssignNode a)
        {
            string argument1 = Gen(a.Expr);
            GenCommand("", "assign", argument1, "", a.Id.Name);
        }

        public override void VisitIfElseNode(IfElseNode i)
        {
            // перевод в трёхадресный код условия
            string exprTmpName = Gen(i.Expr);

            string trueLabel = ThreeAddressCodeTmp.GenTmpLabel();
            string falseLabel = ThreeAddressCodeTmp.GenTmpLabel();
            GenCommand("", "ifgoto", exprTmpName, trueLabel, "");

            // перевод в трёхадресный код false ветки
            i.FalseStat?.Visit(this);
            GenCommand("", "goto", falseLabel, "", "");

            // перевод в трёхадресный код true ветки
            var instructionIndex = Instructions.Count;
            i.TrueStat.Visit(this);
            Instructions[instructionIndex].Label = trueLabel;

            GenCommand(falseLabel, "noop", "", "", "");
        }

        public override void VisitEmptyNode(EmptyNode w)
        {
           GenCommand("", "noop", "", "", "");
        }

        public override void VisitGotoNode(GotoNode g)
        {
            GenCommand("", "goto", g.Label.Num.ToString(), "", "");
        }

        void GenCommand(string label, string operation, string argument1, string argument2, string result)
        {
            Instructions.Add(new Instruction(label, operation, argument1, argument2, result));
        }

        string Gen(ExprNode ex)
        {
            if (ex.GetType() == typeof(BinOpNode))
            {
                var bin = (BinOpNode)ex;
                string argument1 = Gen(bin.Left);
                string argument2 = Gen(bin.Right);
                string result = ThreeAddressCodeTmp.GenTmpName();
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
