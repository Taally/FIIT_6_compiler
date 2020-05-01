using System;
using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodePullingConstants
    {
        static public Tuple<bool, List<Instruction>> PullingConstants(List<Instruction> instructions)
        {
            int count = instructions.Count;
            bool Changed = false;
            var result = new List<Instruction>();
            for (int i = 0; i < count; i++)
            {
                string currentArg1 = instructions[i].Argument1, currentArg2 = instructions[i].Argument2;
                int arg1, arg2;
                string currentOp = instructions[i].Operation;
                if (instructions[i].Operation == "assign"
                    && instructions.GetRange(0, i).FindLast(x => x.Result.ToString() == instructions[i].Argument1) is Instruction cmnd)
                {
                    if (cmnd.Operation == "assign"
                        && int.TryParse(cmnd.Argument1, out arg1))
                    {
                        currentArg1 = cmnd.Argument1;
                        Changed = true;
                    }
                    result.Add(new Instruction(instructions[i].Label, currentOp, currentArg1, currentArg2, instructions[i].Result));
                    continue;
                }
                else if (instructions[i].Operation != "assign")
                {
                    if (instructions.GetRange(0, i).FindLast(x => x.Result.ToString() == instructions[i].Argument1) is Instruction cmnd1
                        && cmnd1.Operation == "assign"
                        && int.TryParse(cmnd1.Argument1, out arg1))
                    {
                        currentArg1 = cmnd1.Argument1;
                        Changed = true;
                    }
                    if (instructions.GetRange(0, i).FindLast(x => x.Result.ToString() == instructions[i].Argument2) is Instruction cmnd2
                        && cmnd2.Operation == "assign"
                        && int.TryParse(cmnd2.Argument1, out arg2))
                    {
                        currentArg2 = cmnd2.Argument1;
                        Changed = true;
                    }
                    result.Add(new Instruction(instructions[i].Label, currentOp, currentArg1, currentArg2, instructions[i].Result));
                    continue;
                }
                result.Add(instructions[i]);
            }
            return Tuple.Create(Changed, result);
        }
    }
}