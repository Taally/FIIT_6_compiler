using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class ReachingTransferFunc : ITransFunc<IEnumerable<Instruction>>
    {
        private ILookup<string, Instruction> defs_groups;
        private ILookup<BasicBlock, Instruction> gen_block;
        private ILookup<BasicBlock, Instruction> kill_block;

        private void GetDefs(List<BasicBlock> blocks)
        {
            List<Instruction> defs = new List<Instruction>();
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

        private void GetGenKill(List<BasicBlock> blocks)
        {
            List<(BasicBlock, Instruction)> gen = new List<ValueTuple<BasicBlock, Instruction>>();
            List<(BasicBlock, Instruction)> kill = new List<ValueTuple<BasicBlock, Instruction>>();
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
                        gen.Add((block, instruction));
                        used.Add(instruction.Result);
                    }
                    foreach (var killed_def in defs_groups[instruction.Result].Where(x => x != instruction))
                    {
                        kill.Add((block, killed_def));
                    }
                }
            }
            gen_block = gen.ToLookup(x => x.Item1, x => x.Item2);
            //delete duplicates
            kill = kill.Distinct().ToList();
            kill_block = kill.ToLookup(x => x.Item1, x => x.Item2);
        }

        public ReachingTransferFunc(ControlFlowGraph g)
        {
            var basicBlocks = g.GetCurrentBasicBlocks().Skip(1).Take(g.GetCurrentBasicBlocks().Count - 2).ToList();
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

        public IEnumerable<Instruction> Transfer(BasicBlock basicBlock, IEnumerable<Instruction> input)
            => ApplyTransferFunc(input, basicBlock);
        
    }
}
