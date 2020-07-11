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
                (var wasChanged, var newInstructions) = DeleteDeadCodeWithDeadVars.DeleteDeadCode(block.GetInstructions(), blockInfo);
                if (wasChanged)
                {
                    block.ClearInstructions();
                    block.AddRangeOfInstructions(newInstructions);
                }
            }
        }
    }
}
