using System;
using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodeOptimizer
    {
        public static bool Changed { get; set; }
        public static List<Func<List<Instruction>, Tuple<bool, List<Instruction>>>> Optimizations { get; set; }
        = new List<Func<List<Instruction>, Tuple<bool, List<Instruction>>>>()
        {
            ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
            ThreeAddressCodeDefUse.DeleteDeadCode,
            ThreeAddressCodeFoldConstants.FoldConstants,
            ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto,
            //ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities,
            //DeleteDeadCodeWithDeadVars.DeleteDeadCode,
            ThreeAdressCodeGotoToGoto.ReplaceGotoToGoto,
            ThreeAddressCodePullingConstants.PullingConstants,
            ThreeAddressCodePullingCopies.PullingCopies
        };

        public static List<Instruction> Optimize(List<Instruction> instructions)
        {
            var result = instructions;
            int currentOpt = 0;

            while (currentOpt < Optimizations.Count)
            {
                var answer = Optimizations[currentOpt++](result);
                if (answer.Item1)
                {
                    currentOpt = 0;
                    result = answer.Item2;
                }
            }

            return result;
        }
    }
}
