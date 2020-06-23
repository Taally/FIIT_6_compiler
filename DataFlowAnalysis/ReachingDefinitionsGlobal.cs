using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using InOutInfo = InOutData<IEnumerable<Instruction>>;

    public class ReachingDefinitionsGlobal
    {
        public void DeleteDeadCode(ControlFlowGraph graph, InOutInfo info)
        {
            var usedVars = CreateUsedVarsSets(graph);
            var reachabilityMatrix = CreateReachabilityMatrix(graph);

            foreach (var block in graph.GetCurrentBasicBlocks().Reverse())
            {
                var deadDefinitions = info[block].In.Except(info[block].Out);
                foreach (var oldDef in deadDefinitions)
                {
                    var variable = oldDef.Result;

                    // find first assign in current block that rewrites variable
                    var newDefIndex = block.GetInstructions()
                        .Select((t, i) => new { Instruction = t, Index = i })
                        .First(t => t.Instruction.Operation == "assign" && t.Instruction.Result == variable)
                        .Index;

                    var blockWithOldDef = graph.GetCurrentBasicBlocks()
                        .Single(z => !info[z].In.Contains(oldDef) && info[z].Out.Contains(oldDef));
                    var oldDefIndex = block.GetInstructions().IndexOf(oldDef);

                    var oldBlockIndex = graph.VertexOf(blockWithOldDef);
                    var newBlockIndex = graph.VertexOf(block);

                    if (IsUsedInCurrentBlock(block, variable, newDefIndex)
                        || IsUsedInOriginalBlock(blockWithOldDef, variable, oldDefIndex)
                        || IsUsedInOtherBlocks(graph, oldBlockIndex, newBlockIndex, variable, usedVars, reachabilityMatrix))
                    {
                        continue;
                    }

                    // if we delete oldDef, we must remove it from all INs and OUTs
                    blockWithOldDef.RemoveInstructionByIndex(oldDefIndex);
                    info = (InOutInfo)info.ToDictionary(
                        z => z.Key,
                        z => (z.Value.In.Where(t => t != oldDef), 
                              z.Value.Out.Where(t => t != oldDef)));
                }
            }
        }

        private bool IsUsedInCurrentBlock(BasicBlock block, string variable, int newDefIndex)
        {
            for (var i = 0; i <= newDefIndex; ++i)
            {
                if (block.GetInstructions()[i].Argument1 == variable
                    || block.GetInstructions()[i].Argument2 == variable)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsUsedInOriginalBlock(BasicBlock block, string variable, int oldDefIndex)
        {
            for (var i = oldDefIndex + 1; i < block.GetInstructions().Count(); ++i)
            {
                if (block.GetInstructions()[i].Argument1 == variable
                    || block.GetInstructions()[i].Argument2 == variable)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsUsedInOtherBlocks(
            ControlFlowGraph graph, 
            int oldBlockIndex, 
            int newBlockIndex,
            string variable,
            Dictionary<int, HashSet<string>> usedVars,
            bool[,] reachabilityMatrix)
        {
            for (var i = 0; i < graph.GetCurrentBasicBlocks().Count(); ++i)
            {
                if (i == oldBlockIndex || i == newBlockIndex)
                {
                    continue;
                }
                if (reachabilityMatrix[oldBlockIndex, i]
                    && reachabilityMatrix[i, newBlockIndex]
                    && usedVars[i].Contains(variable))
                {
                    return true;
                }
            }
            return false;
        }

        private Dictionary<int, HashSet<string>> CreateUsedVarsSets(ControlFlowGraph graph)
        {
            var result = new Dictionary<int, HashSet<string>>();

            for (var i = 0; i < graph.GetCurrentBasicBlocks().Count(); ++i)
            {
                result[i] = new HashSet<string>();
                var block = graph.GetCurrentBasicBlocks()[i];
                foreach (var instruction in block.GetInstructions())
                {
                    if (IsVariable(instruction.Argument1))
                    {
                        result[i].Add(instruction.Argument1);
                    }
                    if (IsVariable(instruction.Argument2))
                    {
                        result[i].Add(instruction.Argument2);
                    }
                }
            }

            return result;
        }

        private bool[,] CreateReachabilityMatrix(ControlFlowGraph graph)
        {
            var blocksCount = graph.GetCurrentBasicBlocks().Count;
            var result = new bool[blocksCount, blocksCount];

            // fill adjacency matrix
            foreach (var block1 in graph.GetCurrentBasicBlocks())
            {
                var index1 = graph.VertexOf(block1);
                foreach (var block2 in graph.GetCurrentBasicBlocks())
                {
                    var index2 = graph.VertexOf(block2);
                    result[index1, index2] =
                        graph.GetChildrenBasicBlocks(index1).Select(x => x.vertex).Contains(index2);
                }
            }

            // find reachability matrix
            for (var k = 0; k < blocksCount; ++k)
            {
                for (var i = 0; i < blocksCount; ++k)
                {
                    for (var j = 0; j < blocksCount; ++j)
                    {
                        result[i, j] = result[i, j] || result[i, k] && result[k, j];
                    }
                }
            }

            return result;
        }

        private bool IsVariable(string s) => 
            !string.IsNullOrEmpty(s) 
            && !int.TryParse(s, out _)
            && !(s == "true")
            && !(s == "false");
    }
}
