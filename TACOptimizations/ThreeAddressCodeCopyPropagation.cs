using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public static class ThreeAddressCodeCopyPropagation
    {
        public static (bool wasChanged, IReadOnlyList<Instruction> instructions) PropagateCopies(IReadOnlyList<Instruction> instructions)
        {
            var count = 0;
            var wasChanged = false;
            var result = new List<Instruction>();
            foreach (var instruction in instructions)
            {
                string currentArg1 = instruction.Argument1, currentArg2 = instruction.Argument2;
                var currentOp = instruction.Operation;
                if (!int.TryParse(instruction.Argument1, out var arg))
                {
                    var index1 = instructions.Take(count).ToList().FindLastIndex(x => x.Result == instruction.Argument1);
                    if (index1 != -1
                        && instructions[index1].Operation == "assign"
                        && !int.TryParse(instructions[index1].Argument1, out arg)
                        && instructions.Skip(index1).Take(count - index1).ToList().FindLastIndex(x => x.Result == instructions[index1].Argument1) == -1)
                    {
                        currentArg1 = instructions[index1].Argument1;
                        wasChanged = true;
                    }

                }
                if (!int.TryParse(instruction.Argument2, out arg))
                {
                    var index2 = instructions.Take(count).ToList().FindLastIndex(x => x.Result == instruction.Argument2);
                    if (index2 != -1
                        && instructions[index2].Operation == "assign"
                        && !int.TryParse(instructions[index2].Argument1, out arg)
                        && instructions.Skip(index2).Take(count - index2).ToList().FindLastIndex(x => x.Result == instructions[index2].Argument1) == -1)
                    {
                        currentArg2 = instructions[index2].Argument1;
                        wasChanged = true;
                    }
                }
                result.Add(new Instruction(instruction.Label, instruction.Operation, currentArg1, currentArg2, instruction.Result));
                ++count;
            }
            return (wasChanged, result);
        }
    }
}
