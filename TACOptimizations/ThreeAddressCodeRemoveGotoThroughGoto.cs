using System;
using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodeRemoveGotoThroughGoto
    {
        // устранение переходов через переходы (пример из презентации Opt5.pdf слайд 32)
        public static (bool wasChanged, IReadOnlyList<Instruction> instructions) RemoveGotoThroughGoto(IReadOnlyList<Instruction> instructions)
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

                        var label = com2.Label.StartsWith("L") && uint.TryParse(com2.Label.Substring(1), out _) ? "" : com2.Label;
                        newInstructions.Add(new Instruction(label, com2.Operation, com2.Argument1, com2.Argument2, com2.Result));
                        newInstructions.Add(com3.Copy());

                        wasChanged = true;
                        i += 3;
                        continue;
                    }
                }
                newInstructions.Add(instructions[i].Copy());
            }
            return (wasChanged, newInstructions);
        }
    }
}
