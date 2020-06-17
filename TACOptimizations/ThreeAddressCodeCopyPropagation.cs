using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodeCopyPropagation
    {
        public static (bool wasChanged, List<Instruction> instructions) PropagateCopies(List<Instruction> instructions)
        {
            var count = instructions.Count;
            var wasChanged = false;
            var result = new List<Instruction>();
            for (var i = 0; i < count; i++)
            {
                string currentArg1 = instructions[i].Argument1, currentArg2 = instructions[i].Argument2;
                var currentOp = instructions[i].Operation;
                if (!int.TryParse(instructions[i].Argument1, out var arg))
                {
                    var index1 = instructions.GetRange(0, i).FindLastIndex(x => x.Result == instructions[i].Argument1);
                    if (index1 != -1
                        && instructions[index1].Operation == "assign"
                        && !int.TryParse(instructions[index1].Argument1, out arg)
                        && instructions.GetRange(index1, i - index1).FindLastIndex(x => x.Result == instructions[index1].Argument1) == -1)
                    {
                        currentArg1 = instructions[index1].Argument1;
                        wasChanged = true;
                    }

                }
                if (!int.TryParse(instructions[i].Argument2, out arg))
                {
                    var index2 = instructions.GetRange(0, i).FindLastIndex(x => x.Result == instructions[i].Argument2);
                    if (index2 != -1
                        && instructions[index2].Operation == "assign"
                        && !int.TryParse(instructions[index2].Argument1, out arg)
                        && instructions.GetRange(index2, i - index2).FindLastIndex(x => x.Result == instructions[index2].Argument1) == -1)
                    {
                        currentArg2 = instructions[index2].Argument1;
                        wasChanged = true;
                    }
                }
                result.Add(new Instruction(instructions[i].Label, instructions[i].Operation, currentArg1, currentArg2, instructions[i].Result));
            }
            return (wasChanged, result);
        }
    }
}
