using System;
using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodeRemoveGotoThroughGoto
    {
        // устранение переходов через переходы (пример из презентации Opt5.pdf слайд 32)
        public static (bool wasChanged, List<Instruction> instructions) RemoveGotoThroughGoto(List<Instruction> instructions)
        {
            if (instructions is null)
            {
                throw new ArgumentNullException("instructions is null");
            }

            var wasChanged = false;
            var newInstructions = new List<Instruction>();

            for (var i = 0; i < instructions.Count; ++i)
            {
                if (instructions[i].Operation == "ifgoto" && 4 <= (instructions.Count - i))
                {
                    var com0 = instructions[i];
                    var com1 = instructions[i + 1];
                    var com2 = instructions[i + 2];
                    var com3 = instructions[i + 3];

                    // только одна операция
                    if (com1.Operation == "goto" && com1.Label == "" && com2.Operation != "noop" && com0.Argument2 == com2.Label && com1.Argument1 == com3.Label)
                    {
                        var tmpName = ThreeAddressCodeTmp.GenTmpName();
                        newInstructions.Add(new Instruction(com0.Label, "NOT", com0.Argument1, "", tmpName));
                        newInstructions.Add(new Instruction("", "ifgoto", tmpName, com3.Label, ""));
                        newInstructions.Add(new Instruction(com2.Label.StartsWith("L") && uint.TryParse(com2.Label.Substring(1), out _) ? "" : com2.Label, com2.Operation, com2.Argument1, com2.Argument2, com2.Result));
                        newInstructions.Add(new Instruction(com3.Label, com3.Operation, com3.Argument1, com3.Argument2, com3.Result));

                        wasChanged = true;
                        i += 3;
                        continue;
                    }
                }
                newInstructions.Add(new Instruction(instructions[i].Label, instructions[i].Operation, instructions[i].Argument1, instructions[i].Argument2, instructions[i].Result));
            }
            return (wasChanged, newInstructions);
        }
    }
}
