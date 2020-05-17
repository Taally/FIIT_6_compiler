using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class ReachingDefinitions
    {
        public InOutInfo Execute(ControlFlowGraph graph)
        {
            // 0. Skipping #in and #out basic blocks
            var basicBlocks = graph.GetCurrentBasicBlocks().Skip(1).Take(graph.GetCurrentBasicBlocks().Count - 2);
            // 1. Searching for all assingns in all BBl and excluding repeating ones within a single block
            var definitions = new List<DefinitionInfo>();
            foreach (var block in basicBlocks)
            {
                var used = new HashSet<string>();
                foreach (var inst in block.GetInstructions().Reverse<Instruction>())
                    // adding PLUS here because "for" cycle is implemented using this op
                    if ((inst.Operation == "assign" || inst.Operation == "input" ||
                        inst.Operation == "PLUS" && !inst.Result.StartsWith("#")) &&
                        !used.Contains(inst.Result))
                    {
                        definitions.Add(new DefinitionInfo { Instruction = inst, BasicBlock = block });
                        used.Add(inst.Result);
                    }
            }

            // 2. Finding all 'gen' and 'kill'
            var killRaw = new List<DefinitionInfo>();
            var lookup = definitions.GroupBy(info => info.Instruction.Result);
            foreach (var group in lookup)
            {
                foreach (var def in group)
                {
                    var killBlocks = group.Select(g => g.BasicBlock).Where(b => b != def.BasicBlock);
                    killRaw.AddRange(killBlocks.Select(b => new DefinitionInfo { Instruction = def.Instruction, BasicBlock = b }));
                }
            }

            var gen = definitions.ToLookup(z => z.BasicBlock, z => z.Instruction);
            var kill = killRaw.ToLookup(z => z.BasicBlock, z => z.Instruction);

            // 3. Running an algorithm
            var resultIn = new Dictionary<BasicBlock, IEnumerable<Instruction>>();
            var resultOut = new Dictionary<BasicBlock, IEnumerable<Instruction>>();
            foreach (var block in basicBlocks)
                resultOut[block] = new List<Instruction>();

            var outWasChanged = true;
            while (outWasChanged)
            {
                outWasChanged = false;
                foreach (var block in basicBlocks)
                {
                    var parents = graph.GetParentsBasicBlocks(block)
                        .Select(z => z.Item2)
                        .Where(bl => bl.GetInstructions()[0].Label != "#in" && bl.GetInstructions()[0].Label != "#out");
                    resultIn[block] = new List<Instruction>(parents.SelectMany(b => resultOut[b]).Distinct());
                    var outNew = gen[block].Union(resultIn[block].Except(kill[block]));
                    if (outNew.Except(resultOut[block]).Any())
                    {
                        outWasChanged = true;
                        resultOut[block] = outNew;
                    }
                }
            }

            return new InOutInfo { In = resultIn, Out = resultOut };
        }

        public class InOutInfo
        {
            public Dictionary<BasicBlock, IEnumerable<Instruction>> In { get; set; }
            public Dictionary<BasicBlock, IEnumerable<Instruction>> Out { get; set; }
        }

        private class DefinitionInfo
        {
            public BasicBlock BasicBlock { get; set; }
            public Instruction Instruction { get; set; }
        }
    }
}
