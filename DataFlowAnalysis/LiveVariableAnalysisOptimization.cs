using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{
    public class LiveVariableAnalysisOptimization
    {
        public static void DeleteDeadCode(ControlFlowGraph cfg)
        {
            var info = new LiveVariableAnalysis().Execute(cfg);
            foreach (var block in cfg.GetCurrentBasicBlocks())
            {
                var blockInfo = info[block].Out;
                var newInstructions = DeleteDeadCodeWithDeadVars.DeleteDeadCode(block.GetInstructions(), blockInfo).instructions;

                for (var i = block.GetInstructions().Count - 1; i >= 0; --i)
                {
                    block.RemoveInstructionByIndex(i);
                }

                block.AddRangeOfInstructions(newInstructions.ToList());
            }
        }
    }
}
