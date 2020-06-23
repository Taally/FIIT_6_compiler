using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{
    public class LiveVariableAnalysisOptimization
    {
        public static ControlFlowGraph LiveVariableDeleteDeadCode(ControlFlowGraph cfg)
        {
            var newInstructions = new List<Instruction>();

            var activeVariable = new LiveVariableAnalysis();
            var resActiveVariable = activeVariable.Execute(cfg);
            foreach (var x in cfg.GetCurrentBasicBlocks().Take(cfg.GetCurrentBasicBlocks().Count-1).Skip(1))
            {
                var instructionsTemp = x.GetInstructions();
                if (resActiveVariable.ContainsKey(x))
                {
                    var InOutTemp = resActiveVariable[x];
                    foreach (var i in instructionsTemp)
                    {
                        if (!InOutTemp.Out.Contains(i.Result) && i.Operation == "assign" && i.Argument1 != i.Result)
                        {
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
            return new ControlFlowGraph(newInstructions);
        }

        public static ControlFlowGraph DeleteDeadCode(ControlFlowGraph cfg)
        {
            var newInstructions = new List<Instruction>();

            var activeVariable = new LiveVariableAnalysis();
            var resActiveVariable = activeVariable.Execute(cfg);
            foreach (var x in cfg.GetCurrentBasicBlocks().Take(cfg.GetCurrentBasicBlocks().Count - 1).Skip(1))
            {
                var instructionsTemp = x.GetInstructions();
                if (resActiveVariable.ContainsKey(x))
                {
                    var InOutTemp = resActiveVariable[x];
                    foreach (var i in instructionsTemp)
                    {
                        if (!InOutTemp.Out.Contains(i.Result) && i.Operation == "assign" && i.Argument1 != i.Result)
                        {
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
            return new ControlFlowGraph(newInstructions);
        }
    }
}
