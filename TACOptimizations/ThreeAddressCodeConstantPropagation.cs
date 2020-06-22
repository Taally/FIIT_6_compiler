using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public static class ThreeAddressCodeConstantPropagation
    {
        public static (bool wasChanged, IReadOnlyList<Instruction> instructions) PropagateConstants(IReadOnlyCollection<Instruction> instructions)
        {
            var count = 0;
            var wasChanged = false;
            var result = new List<Instruction>();
            foreach (var instruction in instructions)
            {
                string currentArg1 = instruction.Argument1, currentArg2 = instruction.Argument2;
                int arg1;
                var currentOp = instruction.Operation;
                if (instruction.Operation == "assign"
                    && instructions.Take(count).ToList().FindLast(x => x.Result == instruction.Argument1) is Instruction cmnd)
                {
                    if (cmnd.Operation == "assign"
                        && int.TryParse(cmnd.Argument1, out arg1))
                    {
                        currentArg1 = cmnd.Argument1;
                        wasChanged = true;
                    }
                    result.Add(new Instruction(instruction.Label, currentOp, currentArg1, currentArg2, instruction.Result));
                }
                else if (instruction.Operation != "assign")
                {
                    if (instructions.Take(count).ToList().FindLast(x => x.Result == instruction.Argument1) is Instruction cmnd1
                        && cmnd1.Operation == "assign"
                        && int.TryParse(cmnd1.Argument1, out arg1))
                    {
                        currentArg1 = cmnd1.Argument1;
                        wasChanged = true;
                    }
                    if (instructions.Take(count).ToList().FindLast(x => x.Result == instruction.Argument2) is Instruction cmnd2
                        && cmnd2.Operation == "assign"
                        && int.TryParse(cmnd2.Argument1, out var arg2))
                    {
                        currentArg2 = cmnd2.Argument1;
                        wasChanged = true;
                    }
                    result.Add(new Instruction(instruction.Label, currentOp, currentArg1, currentArg2, instruction.Result));
                }
                else
                {
                    result.Add(instruction);
                }
                ++count;
            }

            return (wasChanged, result);
        }
    }
}
