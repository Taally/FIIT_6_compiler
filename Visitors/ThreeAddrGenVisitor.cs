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

        private string GenEndLabel(IfElseNode i)
        {
            Node parent, curNode;
            if (i.Parent is LabelStatementNode)
            {
                parent = i.Parent.Parent;
                curNode = i.Parent;
            }
            else
            {
                parent = i.Parent;
                curNode = i;
            }
            var ifIndex = parent.StatChildren.IndexOf(curNode as StatementNode);
            return parent.StatChildren.Count > ifIndex + 1 ? // next statement exists
                (parent.StatChildren[ifIndex + 1] is LabelStatementNode nextLabelNode) ? // next statement has label
                nextLabelNode.Label.Num.ToString() :
                ThreeAddressCodeTmp.GenTmpLabel() :
                ThreeAddressCodeTmp.GenTmpLabel();
        }

        private bool InvertIfExpression(IfElseNode i)
        {
            if (i.Expr is BoolValNode boolValNode)
            {
                boolValNode.Val = !boolValNode.Val;
                return true;
            }
            if (i.Expr is UnOpNode unOpNode && unOpNode.Op == OpType.NOT) // if it's negation, just remove it
            {
                i.Expr = (i.Expr as UnOpNode).Expr;
                return true;
            }
            if (i.Expr is BinOpNode binOpNode &&
                (binOpNode.Op == OpType.AND ||
                binOpNode.Op == OpType.EQGREATER ||
                binOpNode.Op == OpType.EQLESS ||
                binOpNode.Op == OpType.EQUAL ||
                binOpNode.Op == OpType.GREATER ||
                binOpNode.Op == OpType.LESS ||
                binOpNode.Op == OpType.NOTEQUAL ||
                binOpNode.Op == OpType.OR))
            {
                switch (binOpNode.Op)
                {
                    case OpType.OR:
                        binOpNode.Op = OpType.AND;
                        binOpNode.Left = new UnOpNode(binOpNode.Left, OpType.NOT);
                        binOpNode.Right = new UnOpNode(binOpNode.Right, OpType.NOT);
                        break;
                    case OpType.AND:
                        binOpNode.Op = OpType.OR;
                        binOpNode.Left = new UnOpNode(binOpNode.Left, OpType.NOT);
                        binOpNode.Right = new UnOpNode(binOpNode.Right, OpType.NOT);
                        break;
                    case OpType.EQUAL:
                        binOpNode.Op = OpType.NOTEQUAL;
                        break;
                    case OpType.NOTEQUAL:
                        binOpNode.Op = OpType.EQUAL;
                        break;
                    case OpType.GREATER:
                        binOpNode.Op = OpType.EQLESS;
                        break;
                    case OpType.LESS:
                        binOpNode.Op = OpType.EQGREATER;
                        break;
                    case OpType.EQGREATER:
                        binOpNode.Op = OpType.LESS;
                        break;
                    case OpType.EQLESS:
                        binOpNode.Op = OpType.GREATER;
                        break;
                }
                return true;
            }
            return false;
        }

        public override void VisitIfElseNode(IfElseNode i)
        {
            string endLabel, exprTmpName;

            if (i.FalseStat == null)
            {
                var wasInverted = InvertIfExpression(i);
                exprTmpName = Gen(i.Expr);
                endLabel = GenEndLabel(i);
                if (wasInverted)
                {
                    GenCommand("", "ifgoto", exprTmpName, endLabel, "");
                }
                else
                {
                    var tmpVar = ThreeAddressCodeTmp.GenTmpName();
                    GenCommand("", "assign", $"!{exprTmpName}", "", tmpVar);
                    GenCommand("", "ifgoto", tmpVar, endLabel, "");
                }

                i.TrueStat.Visit(this);
            }
            else
            {
                exprTmpName = Gen(i.Expr);
                var trueLabel = i.TrueStat is LabelStatementNode label
                   ? label.Label.Num.ToString()
                   : i.TrueStat is BlockNode block
                       && block.List.StatChildren[0] is LabelStatementNode labelB
                       ? labelB.Label.Num.ToString()
                       : ThreeAddressCodeTmp.GenTmpLabel();

                GenCommand("", "ifgoto", exprTmpName, trueLabel, "");

                i.FalseStat.Visit(this);

                endLabel = GenEndLabel(i);
                GenCommand("", "goto", endLabel, "", "");

                var instructionIndex = Instructions.Count;
                i.TrueStat.Visit(this);
                Instructions[instructionIndex].Label = trueLabel;
            }

            if (endLabel.StartsWith("L"))
            {
                GenCommand(endLabel, "noop", "", "", "");
            }
        }

        public override void VisitEmptyNode(EmptyNode w) => GenCommand("", "noop", "", "", "");

        public override void VisitGotoNode(GotoNode g) => GenCommand("", "goto", g.Label.Num.ToString(), "", "");

        public override void VisitWhileNode(WhileNode w)
        {
            var numStr = Instructions.Count;
            var exprTmpName = Gen(w.Expr);

            var whileHeadLabel = ThreeAddressCodeTmp.GenTmpLabel();
            var whileBodyLabel = w.Stat is LabelStatementNode label
                ? label.Label.Num.ToString()
                : w.Stat is BlockNode block
                                && block.List.StatChildren[0] is LabelStatementNode labelB
                    ? labelB.Label.Num.ToString()
                    : ThreeAddressCodeTmp.GenTmpLabel();

            var exitLabel = ThreeAddressCodeTmp.GenTmpLabel();

            // Т.е. условие while() - константа
            if (numStr == Instructions.Count)
            {
                GenCommand("", "noop", "", "", "");
            }

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
            // Делаем допущение, что for шагает на +1 до границы, не включая её
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
            if (ex is BinOpNode binOp)
            {
                var argument1 = Gen(binOp.Left);
                var argument2 = Gen(binOp.Right);
                var result = ThreeAddressCodeTmp.GenTmpName();
                GenCommand("", binOp.Op.ToString(), argument1, argument2, result);
                return result;
            }
            else if (ex is UnOpNode unOp)
            {
                var argument1 = Gen(unOp.Expr);
                var result = ThreeAddressCodeTmp.GenTmpName();
                GenCommand("", unOp.Op.ToString(), argument1, "", result);
                return result;
            }
            else if (ex is IdNode id)
            {
                return id.Name;
            }
            else if (ex is IntNumNode intNum)
            {
                return intNum.Num.ToString();
            }
            else if (ex is BoolValNode bl)
            {
                return bl.Val.ToString();
            }

            return null;
        }
    }
}
