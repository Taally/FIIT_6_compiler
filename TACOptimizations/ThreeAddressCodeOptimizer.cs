using System;
using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodeOptimizer
    {
        public static bool Changed { get; set; }
        public static List<Func<Dictionary<int, List<Instruction>>, Tuple<bool, Dictionary<int, List<Instruction>>>>> Optimizations { get; set; }
        = new List<Func<Dictionary<int, List<Instruction>>, Tuple<bool, Dictionary<int, List<Instruction>>>>>()
        {
            //ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
            ThreeAddressCodeDefUse.DeleteDeadCode,
            //ThreeAddressCodeFoldConstants.FoldConstants,
            //ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto,
            //ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
            //ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities,
            //DeleteDeadCodeWithDeadVars.DeleteDeadCode,
            //ThreeAddressCodeConstantPropagation.PropagateConstants,
            //ThreeAddressCodeCopyPropagation.PropagateCopies
        };

        public static Dictionary<int, List<Instruction>> Optimize(Dictionary<int, List<Instruction>> bBlocks)
        {
            var result = bBlocks;
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
