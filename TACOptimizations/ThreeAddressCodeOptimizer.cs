using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using Optimization = Func<IReadOnlyList<Instruction>, (bool wasChanged, IReadOnlyList<Instruction> instructions)>;

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

        public static IReadOnlyList<Instruction> OptimizeAll(List<Instruction> instructions)
        {
            var cfg = new ControlFlowGraph(instructions);
            cfg.ReBuildCFG(Optimize(cfg.GetInstructions(), BasicBlockOptimizations, AllCodeOptimizations));
            return cfg.GetInstructions();
        }

        public static IReadOnlyList<Instruction> Optimize(
           IReadOnlyList<Instruction> instructions,
           List<Optimization> basicBlockOptimizations = null,
           List<Optimization> allCodeOptimizations = null,
           bool UnreachableCodeElimination = false)
        {
            basicBlockOptimizations = basicBlockOptimizations ?? new List<Optimization>();
            allCodeOptimizations = allCodeOptimizations ?? new List<Optimization>();


            var blocks = UnreachableCodeElimination ?
                BasicBlockLeader.DivideLeaderToLeader(new ControlFlowGraph(instructions).GetInstructions()) :
                BasicBlockLeader.DivideLeaderToLeader(instructions);

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

        private static IReadOnlyList<Instruction> OptimizeAllCode(IReadOnlyList<Instruction> instructions, List<Optimization> opts)
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
