using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;

namespace SimpleLang
{
    static class ThreeAddressCodeOptimizer
    {
        static public bool Changed { get; set; }

        public static List<Instruction> Optimize(List<Instruction> instructions)
        {
            List<Func<List<Instruction>, Tuple<bool, List<Instruction>>>> ListOptimization
                = new List<Func<List<Instruction>, Tuple<bool, List<Instruction>>>>()
                {
                    ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
                    ThreeAddressCodeDefUse.DeleteDeadCode,
                    ThreeAddressCodeFoldConstants.FoldConstants
                };

            var result = instructions;
            int currentOpt = 0, enabledOpt = 0;

            while (true) {
                while (currentOpt <= enabledOpt) {
                    var answer = ListOptimization[currentOpt](result);
                    if (answer.Item1){
                        currentOpt = 0;
                        result = answer.Item2;
                    }
                    else {
                        ++currentOpt;
                    }
                }
                if (++enabledOpt == ListOptimization.Count)
                    break;
            }

            #region
            /* while (true)
            {
                // FoldConstants(instructions);
                // if (Changed)
                //     continue;

                //DeleteDeadCodeWithDeadVars(instructions);
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

            /*var res = ThreeAddressCodeRemoveNoop.RemoveEmptyNodes(result);
            result = res.Item2;
            if (res.Item1) continue;

            break;
        }*/
            #endregion

            return result;
        }

        static public void DeleteDeadCodeWithDeadVars(List<Instruction> instructions)
        {
            Changed = false;
            Dictionary<string, bool> varStatus = new Dictionary<string, bool>();

            var last = instructions.Last();
            varStatus.Add(last.Result, false);
            if (!int.TryParse(last.Argument1, out _) && last.Argument1 != "True" && last.Argument1 != "False")
                varStatus[last.Argument1] = true;
            if (!int.TryParse(last.Argument2, out _) && last.Argument2 != "True" && last.Argument2 != "False")
                varStatus[last.Argument2] = true;

            for (int i = instructions.Count - 2; i >= 0; --i)
            {
                var instruction = instructions[i];
                if (instruction.Operation == "noop")
                    continue;
                if (varStatus.ContainsKey(instruction.Result) && !varStatus[instruction.Result])
                {
                    instructions[i] = new Instruction(instruction.Label, "noop", null, null, null);
                    Changed = true;
                    continue;
                }

                varStatus[instruction.Result] = false;
                if (!int.TryParse(instruction.Argument1, out _) && instruction.Argument1 != "True" && instruction.Argument1 != "False")
                    varStatus[instruction.Argument1] = true;
                if (!int.TryParse(instruction.Argument2, out _) && instruction.Argument2 != "True" && instruction.Argument2 != "False")
                    varStatus[instruction.Argument2] = true;
            }
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
