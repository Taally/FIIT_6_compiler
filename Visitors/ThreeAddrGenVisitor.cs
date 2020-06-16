using System.Collections.Generic;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class ThreeAddrGenVisitor : AutoVisitor
    {
        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public override void VisitLabelstatementNode(LabelStatementNode l)
        {
            var instructionIndex = Instructions.Count;
            // Чтобы не затиралась временная метка у while
            if (l.Stat is WhileNode)
            {
                GenCommand("", "noop", "", "", "");
            }
            l.Stat.Visit(this);
            Instructions[instructionIndex].Label = l.Label.Num.ToString();
        }

        public override void VisitAssignNode(AssignNode a)
        {
            var argument1 = Gen(a.Expr);
            GenCommand("", "assign", argument1, "", a.Id.Name);
        }

        public override void VisitIfElseNode(IfElseNode i)
        {
            // перевод в трёхадресный код условия
            var exprTmpName = Gen(i.Expr);

            var trueLabel = ThreeAddressCodeTmp.GenTmpLabel();
            if (i.TrueStat is LabelStatementNode label)
            {
                trueLabel = label.Label.Num.ToString();
            }
            else
            if (i.TrueStat is BlockNode block
                && block.List.StatChildren[0] is LabelStatementNode labelB)
            {
                trueLabel = labelB.Label.Num.ToString();
            }

            var falseLabel = ThreeAddressCodeTmp.GenTmpLabel();
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

        public override void VisitEmptyNode(EmptyNode w) => GenCommand("", "noop", "", "", "");

        public override void VisitGotoNode(GotoNode g) => GenCommand("", "goto", g.Label.Num.ToString(), "", "");

        public override void VisitWhileNode(WhileNode w)
        {
            var exprTmpName = Gen(w.Expr);

            var whileHeadLabel = ThreeAddressCodeTmp.GenTmpLabel();
            var whileBodyLabel = ThreeAddressCodeTmp.GenTmpLabel();
            var exitLabel = ThreeAddressCodeTmp.GenTmpLabel();

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
            var Id = f.Id.Name;
            var forHeadLabel = ThreeAddressCodeTmp.GenTmpLabel();
            var exitLabel = ThreeAddressCodeTmp.GenTmpLabel();

            var fromTmpName = Gen(f.From);
            GenCommand("", "assign", fromTmpName, "", Id);

            var toTmpName = Gen(f.To);
            // Делаем допущение, что for шагает на +1 до границы, не включая ее
            var condTmpName = ThreeAddressCodeTmp.GenTmpName();
            GenCommand(forHeadLabel, "EQGREATER", Id, toTmpName, condTmpName);
            GenCommand("", "ifgoto", condTmpName, exitLabel, "");

            f.Stat.Visit(this);

            GenCommand("", "PLUS", Id, "1", Id);
            GenCommand("", "goto", forHeadLabel, "", "");
            GenCommand(exitLabel, "noop", "", "", "");
        }

        public override void VisitInputNode(InputNode i) => GenCommand("", "input", "", "", i.Ident.Name);

        public override void VisitPrintNode(PrintNode p)
        {
            foreach (var x in p.ExprList.ExprChildren)
            {
                var exprTmpName = Gen(x);
                GenCommand("", "print", exprTmpName, "", "");
            }
        }

        private void GenCommand(string label, string operation, string argument1, string argument2, string result) => Instructions.Add(new Instruction(label, operation, argument1, argument2, result));

        private string Gen(ExprNode ex)
        {
            if (ex.GetType() == typeof(BinOpNode))
            {
                var bin = (BinOpNode)ex;
                var argument1 = Gen(bin.Left);
                var argument2 = Gen(bin.Right);
                var result = ThreeAddressCodeTmp.GenTmpName();
                GenCommand("", bin.Op.ToString(), argument1, argument2, result);
                return result;
            }
            else if (ex.GetType() == typeof(UnOpNode))
            {
                var unop = (UnOpNode)ex;
                var argument1 = Gen(unop.Expr);
                var result = ThreeAddressCodeTmp.GenTmpName();
                GenCommand("", unop.Op.ToString(), argument1, "", result);
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
