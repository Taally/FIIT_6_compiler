using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using InOutInfo = InOutData<IEnumerable<Instruction>>;

    public class ReachingDefinitionsGlobal
    {
        public void DeleteDeadCode(ControlFlowGraph graph)
        {
            var usedVars = CreateUsedVarsSets(graph);
            var info = new ReachingDefinitions().Execute(graph);

            var usedDefinitions = new HashSet<Instruction>(
                info[graph.GetCurrentBasicBlocks().Last()].In,
                new InstructionComparer());

            var possibleOperationTypes = new[] { "assign", "input", "PLUS" };

            var wasChanged = true;
            while (wasChanged)
            {
                wasChanged = false;

                foreach (var block in graph.GetCurrentBasicBlocks())
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
                            .Single(z => z.GetInstructions().Any(t => t == oldDef));
                        var oldDefIndex = blockWithOldDef.GetInstructions()
                            .Select((t, i) => new { Instruction = t, Index = i })
                            .Single(t => t.Instruction == oldDef)
                            .Index;

                        if (usedDefinitions.Contains(oldDef)
                            || IsUsedInCurrentBlock(block, variable, newDefIndex)
                            || IsUsedInOriginalBlock(blockWithOldDef, variable, oldDefIndex)
                            || IsUsedInOtherBlocks(graph, blockWithOldDef, oldDef, usedVars, info))
                        {
                            usedDefinitions.Add(oldDef);
                            continue;
                        }

                        // remove useless definition
                        blockWithOldDef.RemoveInstructionByIndex(oldDefIndex);
                        if (!string.IsNullOrEmpty(oldDef.Label))
                        {
                            blockWithOldDef.InsertInstruction(oldDefIndex, new Instruction(oldDef.Label, "noop", null, null, null));
                        }
                        info = new ReachingDefinitions().Execute(graph);
                        wasChanged = true;
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
            BasicBlock blockWithDefinition,
            Instruction definitionToCheck,
            Dictionary<BasicBlock, HashSet<string>> usedVars,
            InOutInfo info)
        {
            var queue = new Queue<BasicBlock>();
            queue.Enqueue(blockWithDefinition);

            while (queue.Count != 0)
            {
                var block = queue.Dequeue();
                foreach (var child in graph.GetChildrenBasicBlocks(graph.VertexOf(block)).Select(z => z.block))
                {
                    var isRewritten = !info[child].Out.Contains(definitionToCheck);
                    var isUsed = usedVars[block].Contains(definitionToCheck.Result);

                    if (!isRewritten)
                    {
                        if (isUsed)
                        {
                            return true;
                        }
                        else
                        {
                            queue.Enqueue(child);
                        }
                    }
                    else
                    {
                        if (!isUsed)
                        {
                            continue;
                        }
                        else
                        {
                            // we need to check instructions before definitionToCheck is rewritten
                            foreach (var instruction in child.GetInstructions())
                            {
                                if (instruction.Argument1 == definitionToCheck.Result
                                    || instruction.Argument2 == definitionToCheck.Result)
                                {
                                    return true;
                                }

                                if (instruction.Result == definitionToCheck.Result)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private Dictionary<BasicBlock, HashSet<string>> CreateUsedVarsSets(ControlFlowGraph graph)
        {
            var result = new Dictionary<BasicBlock, HashSet<string>>();

            foreach (var block in graph.GetCurrentBasicBlocks())
            {
                result[block] = new HashSet<string>();
                foreach (var instruction in block.GetInstructions())
                {
                    if (IsVariable(instruction.Argument1))
                    {
                        result[block].Add(instruction.Argument1);
                    }
                    if (IsVariable(instruction.Argument2))
                    {
                        result[block].Add(instruction.Argument2);
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
