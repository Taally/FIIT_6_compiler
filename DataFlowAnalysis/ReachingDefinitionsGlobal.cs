using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class ReachingDefinitionsGlobal
    {
        public void DeleteDeadCode(ControlFlowGraph graph)
        {
            var usedVars = CreateUsedVarsSets(graph);
            var reachabilityMatrix = CreateReachabilityMatrix(graph);
            var info = new ReachingDefinitions().Execute(graph);

            var usedDefinitions = new HashSet<Instruction>(
                info[graph.GetCurrentBasicBlocks().Last()].In, 
                new InstructionComparer());
            var toDelete = new List<(BasicBlock Block, int Index)>();

            var possibleOperationTypes = new[] { "assign", "input", "PLUS" };

            foreach (var block in graph.GetCurrentBasicBlocks().Reverse())
            {
                var deadDefinitions = info[block].In.Except(info[block].Out);
                foreach (var oldDef in deadDefinitions)
                {
                    var variable = oldDef.Result;

                    // find first assign in current block that rewrites variable
                    var newDefIndex = block.GetInstructions()
                        .Select((t, i) => new { Instruction = t, Index = i })
                        .First(t => possibleOperationTypes.Contains(t.Instruction.Operation) && t.Instruction.Result == variable) 
                        .Index;

                    var blockWithOldDef = graph.GetCurrentBasicBlocks()
                        .SingleOrDefault(z => !info[z].In.Contains(oldDef) && info[z].Out.Contains(oldDef));
                    // if we can't find the block with old definition, that means these definition used in a loop
                    // so it can't be removed
                    if (blockWithOldDef == null)
                    {
                        usedDefinitions.Add(oldDef);
                        continue;
                    }

                    var oldDefIndex = blockWithOldDef.GetInstructions()
                        .Select((t, i) => new { Instruction = t, Index = i })
                        .Single(t => t.Instruction == oldDef)
                        .Index;

                    var oldBlockIndex = graph.VertexOf(blockWithOldDef);
                    var newBlockIndex = graph.VertexOf(block);

                    if (usedDefinitions.Contains(oldDef)
                        || IsUsedInCurrentBlock(block, variable, newDefIndex)
                        || IsUsedInOriginalBlock(blockWithOldDef, variable, oldDefIndex)
                        || IsUsedInOtherBlocks(graph, oldBlockIndex, newBlockIndex, variable, usedVars, reachabilityMatrix))
                    {
                        usedDefinitions.Add(oldDef);
                        continue;
                    }

                    toDelete.Add((blockWithOldDef, oldDefIndex));
                }
            }

            foreach (var block in toDelete.ToLookup(z => z.Block, z => z.Index))
            {
                foreach (var index in block.Distinct().OrderByDescending(z => z))
                {
                    var label = block.Key.GetInstructions()[index].Label;
                    block.Key.RemoveInstructionByIndex(index);
                    if (!string.IsNullOrEmpty(label))
                    {
                        block.Key.InsertInstruction(index, new Instruction(label, "noop", null, null, null));
                    }
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
                for (var i = 0; i < blocksCount; ++i)
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

        private class InstructionComparer : IEqualityComparer<Instruction>
        {
            public bool Equals(Instruction x, Instruction y) => ReferenceEquals(x, y);

            public int GetHashCode(Instruction obj) => (obj as object).GetHashCode();
        }
    }
}
