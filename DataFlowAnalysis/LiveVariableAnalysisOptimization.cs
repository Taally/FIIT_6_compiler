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
            foreach (var x in divResult)
            {
                var instructionsTemp = x.GetInstructions();
                newInstructions = instructionsTemp.GetRange(0, instructionsTemp.Count - 1);
                var InOutTemp = resActiveVariable[x];
                foreach(var y in InOutTemp.Out)
                {
                    if (instructionsTemp[instructionsTemp.Count - 1].Result != y && instructionsTemp[instructionsTemp.Count - 1].Operation == "assign" && instructionsTemp[instructionsTemp.Count - 1].Argument1 != y)
                    {
                        wasChanged = true;
                        newInstructions.Add(instructionsTemp[instructionsTemp.Count - 1]);
                    }
                }
            }
            return (wasChanged, newInstructions);
        }
    }
}
