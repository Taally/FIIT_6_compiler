using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    static class ThreeAddressCodeOptimizer
    {
        public static bool Changed { get; set; }

        public static List<Instruction> Optimize(List<Instruction> instructions)
        {
            var result = instructions;
            while (true)
            {
                //(Changed, instructions) = ThreeAddressCodeFoldConstants.FoldConstants(instructions);
                //result = instructions;
                //if (Changed)
                //    continue;

                //(Changed, instructions) = DeleteDeadCodeWithDeadVars.DeleteDeadCode(instructions);
                //result = instructions;
                //if (Changed)
                //    continue;

                // RemoveGotoThroughGoto(instructions);
                // if (Changed)
                //     continue;
                //
                // break;

                /* Check Def-Use
                 * var res = ThreeAddressCodeDefUse.DeleteDeadCode(instructions);
                instructions = res.Item2;
                if (res.Item1 || Changed) continue;
                break;*/

                //ThreeAdressCodeGotoToGoto.ReplaceGotoToGoto(instructions);
                //if (Changed)
                //    continue;
                ////break;

                //var res = ThreeAddressCodeRemoveNoop.RemoveEmptyNodes(result);
                //result = res.Item2;
                //if (res.Item1) continue;

                break;

            }

            return result;
        }

        // устранение переходов через переходы
        public static void RemoveGotoThroughGoto(List<Instruction> commands)
        {
            for (int i = 0; i < commands.Count; ++i)
            {
                if (commands[i].Operation == "ifgoto" && 4 <= (commands.Count - i))
                {
                    var com0 = commands[i];
                    var com1 = commands[i + 1];
                    var com2 = commands[i + 2];
                    var com3 = commands[i + 3];

                    // только одна операция
                    if (com1.Operation == "goto" && com2.Operation != "noop" && com3.Operation == "noop" && com0.Argument2 == com2.Label && com1.Argument1 == com3.Label)
                    {
                        string tmpName = ThreeAddressCodeTmp.GenTmpName();
                        commands[i] = new Instruction(com0.Label, "EQUAL", "False", com0.Argument1, tmpName); // операция отрицания через EQUAL
                        commands[i+1] = new Instruction("", "ifgoto", tmpName, com3.Label, "");
                        commands[i+2] = new Instruction("", com2.Operation, com2.Argument1, com2.Argument2, com2.Result);
                        commands[i+3] = new Instruction(com3.Label, com3.Operation, com3.Argument1, com3.Argument2, com3.Result);

                        i += 3;
                    }
                }
            }
        }
    }
}
