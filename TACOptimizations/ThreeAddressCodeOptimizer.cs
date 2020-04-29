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
                    ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto,
                    //ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities,
                    //DeleteDeadCodeWithDeadVars.DeleteDeadCode,
                    ThreeAdressCodeGotoToGoto.ReplaceGotoToGoto
                };

            var result = instructions;
            int currentOpt = 0;

            while (currentOpt < ListOptimization.Count){
                 var answer = ListOptimization[currentOpt](result);
                 if (answer.Item1)
                 {
                     currentOpt = 0;
                     result = answer.Item2;
                 }
                 else
                 {
                     ++currentOpt;
                 }
            }

            #region
            /*while (true) {
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
            }*/
            #endregion

            return result;
        }
    }
}
