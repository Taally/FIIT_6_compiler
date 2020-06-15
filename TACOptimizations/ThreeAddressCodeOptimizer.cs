using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using Optimization = Func<List<Instruction>, (bool wasChanged, List<Instruction> instructions)>;

    public static class ThreeAddressCodeOptimizer
    {
        private static List<Optimization> BasicBlockOptimizations => new List<Optimization>()
        {
            ThreeAddressCodeDefUse.DeleteDeadCode,
            ThreeAddressCodeFoldConstants.FoldConstants,
            ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities,
            DeleteDeadCodeWithDeadVars.DeleteDeadCode,
            ThreeAddressCodeConstantPropagation.PropagateConstants,
            ThreeAddressCodeCopyPropagation.PropagateCopies
        };

        private static List<Optimization> AllCodeOptimizations => new List<Optimization>
        {
            ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
            ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto,
            ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
        };

        public static List<Instruction> OptimizeAll(List<Instruction> instructions) =>
            Optimize(instructions, BasicBlockOptimizations, AllCodeOptimizations);

        public static List<Instruction> Optimize(
            List<Instruction> instructions,
            List<Optimization> basicBlockOptimizations = null,
            List<Optimization> allCodeOptimizations = null)
        {
            basicBlockOptimizations = basicBlockOptimizations ?? new List<Optimization>();
            allCodeOptimizations = allCodeOptimizations ?? new List<Optimization>();

            var blocks = BasicBlockLeader.DivideLeaderToLeader(instructions);
            for (var i = 0; i < blocks.Count; ++i)
            {
                blocks[i] = OptimizeBlock(blocks[i], basicBlockOptimizations);
            }

            var preResult = blocks.SelectMany(b => b.GetInstructions()).ToList();
            var result = OptimizeAllCode(preResult, allCodeOptimizations);
            return result;
        }

        private static BasicBlock OptimizeBlock(BasicBlock block, List<Optimization> opts)
        {
            var result = block.GetInstructions();
            var currentOpt = 0;
            while (currentOpt < opts.Count)
            {
                var (wasChanged, instructions) = opts[currentOpt++](result);
                if (wasChanged)
                {
                    currentOpt = 0;
                    result = instructions;
                }
            }
            return new BasicBlock(result);
        }

        private static List<Instruction> OptimizeAllCode(List<Instruction> instructions, List<Optimization> opts)
        {
            var result = instructions;
            var currentOpt = 0;
            while (currentOpt < opts.Count)
            {
                var answer = opts[currentOpt++](result);
                if (answer.wasChanged)
                {
                    currentOpt = 0;
                    result = answer.instructions;
                }
            }
            return result;
        }
    }
}
