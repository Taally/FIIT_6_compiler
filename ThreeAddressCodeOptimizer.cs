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
                    ThreeAddressCodeFoldConstants.FoldConstants,
                    ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto
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
    }
}
