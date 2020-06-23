using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class DeleteDeadCodeWithDeadVars
    {
        public static (bool wasChanged, IReadOnlyList<Instruction> instructions) DeleteDeadCode(
            IReadOnlyList<Instruction> instructions) => DeleteDeadCode(instructions, null);

        public static (bool wasChanged, IReadOnlyList<Instruction> instructions) DeleteDeadCode(
            IReadOnlyList<Instruction> instructions,
            IEnumerable<string> liveVariables = null)
        {
            var wasChanged = false;
            var newInstructions = new List<Instruction>();
            var varStatus = new Dictionary<string, bool>();
            if (liveVariables != null)
            {
                foreach (var variable in liveVariables)
                {
                    varStatus[variable] = true;
                }
            }

            if (liveVariables == null)
            {
                var last = instructions.Last();
                newInstructions.Add(last);
                varStatus.Add(last.Result, false);
                if (!int.TryParse(last.Argument1, out _) && last.Argument1 != "True" && last.Argument1 != "False")
                {
                    varStatus[last.Argument1] = true;
                }
                if (!int.TryParse(last.Argument2, out _) && last.Argument2 != "True" && last.Argument2 != "False")
                {
                    varStatus[last.Argument2] = true;
                }
            }

            var iStart = liveVariables == null
                ? instructions.Count - 2
                : instructions.Count - 1;

            for (var i = iStart; i >= 0; --i)
            {
                var instruction = instructions[i];
                if (instruction.Operation == "noop")
                {
                    newInstructions.Add(instruction);
                    continue;
                }
                if (varStatus.ContainsKey(instruction.Result) && !varStatus[instruction.Result]
                    || instruction.Result.FirstOrDefault() == '#' && !varStatus.ContainsKey(instruction.Result))
                {
                    newInstructions.Add(new Instruction(instruction.Label, "noop", null, null, null));
                    wasChanged = true;
                    continue;
                }

                varStatus[instruction.Result] = false;
                if (!int.TryParse(instruction.Argument1, out _) && instruction.Argument1 != "True" && instruction.Argument1 != "False")
                {
                    varStatus[instruction.Argument1] = true;
                }
                if (instruction.Operation != "UNMINUS" && instruction.Operation != "NOT"
                    && !int.TryParse(instruction.Argument2, out _) && instruction.Argument2 != "True" && instruction.Argument2 != "False")
                {
                    varStatus[instruction.Argument2] = true;
                }
                newInstructions.Add(instruction);
            }
            newInstructions.Reverse();
            return (wasChanged, newInstructions);
        }
    }
}
