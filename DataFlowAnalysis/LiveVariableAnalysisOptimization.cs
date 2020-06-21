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
            var divResult = BasicBlockLeader.DivideLeaderToLeader(instructions);
            var cfg = new ControlFlowGraph(divResult);
            var activeVariable = new LiveVariableAnalysis();
            var resActiveVariable = activeVariable.Execute(cfg);
            foreach (var x in divResult)
            {
                var instructionsTemp = x.GetInstructions();
                if (resActiveVariable.ContainsKey(x))
                {
                    var InOutTemp = resActiveVariable[x];
                    foreach (var i in instructionsTemp)
                    {
                        if (!InOutTemp.Out.Contains(i.Result) && i.Operation == "assign" && i.Argument1 != i.Result)
                        {
                            wasChanged = true;
                            if (i.Label != "")
                            {
                                newInstructions.Add(new Instruction(i.Label, "noop", "", "", ""));
                            }
                        }
                        else
                        {
                            newInstructions.Add(i);
                        }
                    }
                }
            }
            return (wasChanged, newInstructions);
        }
    }
}
