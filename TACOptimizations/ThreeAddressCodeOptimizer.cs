using System;
using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodeOptimizer
    {
        public static List<BasicBlock> Optimize(List<Instruction> instructions)
        {
            var blocks = BasicBlockLeader.DivideLeaderToLeader(instructions);
            for (int i = 0; i < blocks.Count; ++i) {
                var blockInstructions = blocks[i].GetInstructions();
                blocks[i] = new BasicBlock(OptimizeBlocks(blockInstructions));
            }
               
            //Где здесь сбор из блоков обратно в целое представление
            // и прогон глобальных оптимизаций
            // и возможно разбитие на блоки обратно

            return blocks;
        }

        public static List<Func<List<Instruction>, Tuple<bool, List<Instruction>>>> Optimizations { get; set; }
        = new List<Func<List<Instruction>, Tuple<bool, List<Instruction>>>>(){
            //ThreeAddressCodeRemoveNoop.RemoveEmptyNodes,
            ThreeAddressCodeDefUse.DeleteDeadCode,
            ThreeAddressCodeFoldConstants.FoldConstants,
            //ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto,
            //ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
            ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities,
            DeleteDeadCodeWithDeadVars.DeleteDeadCode,
            ThreeAddressCodeConstantPropagation.PropagateConstants,
            ThreeAddressCodeCopyPropagation.PropagateCopies
        };

        public static List<Instruction> OptimizeBlocks(List<Instruction> bBlocks)
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
