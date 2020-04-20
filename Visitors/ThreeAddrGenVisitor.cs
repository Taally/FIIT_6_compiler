using System.Collections.Generic;
using ProgramTree;

namespace SimpleLang.Visitors
{

    class ThreeAddrGenVisitor : AutoVisitor
    {
        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public override void VisitAssignNode(AssignNode a)
        {
            string argument1 = Gen(a.Expr);
            GenCommand("", "assign", argument1, "", a.Id.Name);
        }

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
