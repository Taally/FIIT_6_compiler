using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{
    public class LiveVariableAnalysisOptimization
    {
        public static (bool wasChanged, List<Instruction> instructions) LiveVariableDeleteDeadCode(List<Instruction> instructions)
        {
            var wasChanged = false;
            var newInstructions = new List<Instruction>();
            var divResult = BasicBlockLeader.DivideLeaderToLeader(newInstructions);
            var cfg = new ControlFlowGraph(divResult);
            var activeVariable = new LiveVariableAnalysis();
            var resActiveVariable = activeVariable.Execute(cfg);
            newInstructions = instructions.GetRange(0, instructions.Count - 1);
            foreach (var x in resActiveVariable)
            {
                foreach (var y in x.Value.Out)
                {
                    if (instructions[instructions.Count-1].Result != y && instructions[instructions.Count - 1].Operation == "assign" && instructions[instructions.Count - 1].Argument1 != y)
                    {
                        wasChanged = true;
                    }
                    else
                    {
                        return (wasChanged, newInstructions);
                    }
                }
            }
            newInstructions.Add(instructions[instructions.Count - 1]);
            return (wasChanged, newInstructions);
        }
    }
}
