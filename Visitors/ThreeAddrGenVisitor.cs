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
            // Чтобы не затиралась временная метка у циклов
            if (l.Stat is WhileNode || l.Stat is ForNode)
                GenCommand("", "noop", "", "", "");
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

        public override void VisitWhileNode(WhileNode w)
        {
            string exprTmpName = Gen(w.Expr);

            string whileHeadLabel = ThreeAddressCodeTmp.GenTmpLabel();
            string whileBodyLabel = ThreeAddressCodeTmp.GenTmpLabel();
            string exitLabel = ThreeAddressCodeTmp.GenTmpLabel();

            Instructions[Instructions.Count - 1].Label = whileHeadLabel;

            GenCommand("", "ifgoto", exprTmpName, whileBodyLabel, "");
            GenCommand("", "goto", exitLabel, "", "");

            var instructionIndex = Instructions.Count;
            w.Stat.Visit(this);
            Instructions[instructionIndex].Label = whileBodyLabel;
            GenCommand("", "goto", whileHeadLabel, "", "");
            GenCommand(exitLabel, "noop", "", "", "");
        }

        public override void VisitForNode(ForNode f)
        {
            string Id = f.Id.Name;
            string forHeadLabel = ThreeAddressCodeTmp.GenTmpLabel();
            string forBodyLabel = ThreeAddressCodeTmp.GenTmpLabel();
            string exitLabel = ThreeAddressCodeTmp.GenTmpLabel();

            string fromTmpName = Gen(f.From);
            GenCommand("", "assign", fromTmpName, "", Id);

            string toTmpName = Gen(f.To);
            // Делаем допущение, что for шагает на +1 до границы, не включая ее
            string condTmpName = ThreeAddressCodeTmp.GenTmpName();
            GenCommand(forHeadLabel, "LESS", Id, toTmpName, condTmpName);

            GenCommand("", "ifgoto", condTmpName, forBodyLabel, "");
            GenCommand("", "goto", exitLabel, "", "");

            var instructionIndex = Instructions.Count;
            f.Stat.Visit(this);
            Instructions[instructionIndex].Label = forBodyLabel;

            GenCommand("", "PLUS", Id, "1", Id);
            GenCommand("", "goto", forHeadLabel, "", "");
            GenCommand(exitLabel, "noop", "", "", "");
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
            else if (ex.GetType() == typeof(UnOpNode)) {
                var unop = (UnOpNode)ex;
                string argument1 = Gen(unop.Expr);
                string result = ThreeAddressCodeTmp.GenTmpName();
                GenCommand("", unop.Op.ToString(), argument1, null, result);
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
