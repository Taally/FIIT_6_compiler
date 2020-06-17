using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class ReachingTransferFunc
    {
        private ILookup<string, Instruction> defs_groups;
        private ILookup<BasicBlock, Instruction> gen_block;
        private ILookup<BasicBlock, Instruction> kill_block;

        private void GetDefs(IReadOnlyCollection<BasicBlock> blocks)
        {
            var defs = new List<Instruction>();
            foreach (var block in blocks)
            {
                foreach (var instruction in block.GetInstructions())
                {
                    if (instruction.Operation == "assign" ||
                        instruction.Operation == "input" ||
                        instruction.Operation == "PLUS" && !instruction.Result.StartsWith("#"))
                    {
                        defs.Add(instruction);
                    }
                }
            }
            defs_groups = defs.ToLookup(x => x.Result, x => x);
        }

        private void GetGenKill(IReadOnlyCollection<BasicBlock> blocks)
        {
            var gen = new List<DefinitionInfo>();
            var kill = new List<DefinitionInfo>();
            foreach (var block in blocks)
            {
                var used = new HashSet<string>();
                foreach (var instruction in block.GetInstructions().Reverse<Instruction>())
                {
                    if (!used.Contains(instruction.Result) &&
                        (instruction.Operation == "assign" ||
                        instruction.Operation == "input" ||
                        instruction.Operation == "PLUS" && !instruction.Result.StartsWith("#")))
                    {
                        gen.Add(new DefinitionInfo { BasicBlock = block, Instruction = instruction });
                        used.Add(instruction.Result);
                    }
                    foreach (var killed_def in defs_groups[instruction.Result].Where(x => x != instruction))
                    {
                        kill.Add(new DefinitionInfo { BasicBlock = block, Instruction = killed_def });
                    }
                }
            }
            gen_block = gen.ToLookup(x => x.BasicBlock, x => x.Instruction);
            // delete duplicates
            kill = kill.Distinct().ToList();
            kill_block = kill.ToLookup(x => x.BasicBlock, x => x.Instruction);
        }

        public ReachingTransferFunc(ControlFlowGraph g)
        {
            var basicBlocks = g.GetCurrentBasicBlocks();
            GetDefs(basicBlocks);
            GetGenKill(basicBlocks);
        }

        public ReachingTransferFunc(List<BasicBlock> g)
        {
            GetDefs(g);
            GetGenKill(g);
        }

        public IEnumerable<Instruction> ApplyTransferFunc(IEnumerable<Instruction> In, BasicBlock block) =>
            gen_block[block].Union(In.Except(kill_block[block]));

        public IEnumerable<Instruction> Transfer(BasicBlock basicBlock, IEnumerable<Instruction> input) =>
            ApplyTransferFunc(input, basicBlock);
    }
}
