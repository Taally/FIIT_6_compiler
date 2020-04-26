using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{
    class DeleteDeadCodeWithDeadVars
    {
        public static Tuple<bool, List<Instruction>> DeleteDeadCode(List<Instruction> instructions)
        {
            var isChanged = false;
            var newInstructions = new List<Instruction>();
            Dictionary<string, bool> varStatus = new Dictionary<string, bool>();

            var last = instructions.Last();
            newInstructions.Add(last);
            varStatus.Add(last.Result, false);
            if (!int.TryParse(last.Argument1, out _) && last.Argument1 != "True" && last.Argument1 != "False")
                varStatus[last.Argument1] = true;
            if (!int.TryParse(last.Argument2, out _) && last.Argument2 != "True" && last.Argument2 != "False")
                varStatus[last.Argument2] = true;

            for (int i = instructions.Count - 2; i >= 0; --i)
            {
                var instruction = instructions[i];
                if (instruction.Operation == "noop")
                {
                    newInstructions.Add(instruction);
                    continue;
                }
                if (varStatus.ContainsKey(instruction.Result) && !varStatus[instruction.Result])
                {
                    newInstructions.Add(new Instruction(instruction.Label, "noop", null, null, null));
                    isChanged = true;
                    continue;
                }

                varStatus[instruction.Result] = false;
                if (!int.TryParse(instruction.Argument1, out _) && instruction.Argument1 != "True" && instruction.Argument1 != "False")
                    varStatus[instruction.Argument1] = true;
                if (!int.TryParse(instruction.Argument2, out _) && instruction.Argument2 != "True" && instruction.Argument2 != "False")
                    varStatus[instruction.Argument2] = true;
                newInstructions.Add(instruction);
            }
            newInstructions.Reverse();
            return Tuple.Create(isChanged, newInstructions);
        }
    }
}
