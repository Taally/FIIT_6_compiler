using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class ReachingDefinitionBinary
    {
        public InOutData<IEnumerable<Instruction>> Execute(ControlFlowGraph graph)
        {
            var assigns = graph.GetAssigns().ToList();

            var idByInstruction = assigns
                .Select((value, index) => new { value, index })
                .ToDictionary(x => x.value, x => x.index);

            var instructions = assigns;

            var iterativeAlgorithm = new GenericIterativeAlgorithm<BitArray>();

            var inOutData = iterativeAlgorithm.Analyze(graph, new Operation(graph.GetAmountOfAssigns()), new ReachingTransferFunc(graph, idByInstruction));

            var modifiedBackData = inOutData
                .Select(x => new { x.Key, ModifyInOutBack = ModifyInOutBack(x.Value, instructions) })
                .ToDictionary(x => x.Key, x => x.ModifyInOutBack);

            return new InOutData<IEnumerable<Instruction>>(modifiedBackData);
        }

        private (IEnumerable<Instruction>, IEnumerable<Instruction>) ModifyInOutBack((BitArray, BitArray) inOut, List<Instruction> instructions)
        {
            var (In, Out) = inOut;
            return (BitUtils.TurnIntoInstructions(In, instructions), BitUtils.TurnIntoInstructions(Out, instructions));
        }
        public class Operation : ICompareOperations<BitArray>
        {
            private readonly int _size;

            public Operation(int assigns)
            {
                _size = assigns;
            }

            public BitArray Lower =>
                new BitArray(_size, false);

            public BitArray Upper =>
                throw new NotImplementedException("Upper shouldn't be used in Reaching Definitions");

            public (BitArray, BitArray) Init =>
                (Lower, Lower);

            public (BitArray, BitArray) EnterInit =>
                Init;

            public BitArray Operator(BitArray a, BitArray b)
                => a.Or(b);

            public bool Compare(BitArray a, BitArray b) =>
                BitUtils.AreEqual(a, b);
        }

        public class ReachingTransferFunc : ITransFunc<BitArray>
        {
            private readonly Dictionary<Instruction, int> _ids_by_instruction;
            private ILookup<string, Instruction> defs_groups;
            private IDictionary<BasicBlock, BitArray> gen_block;
            private IDictionary<BasicBlock, BitArray> kill_block;

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
                        foreach (var killedDef in defs_groups[instruction.Result].Where(x => x != instruction))
                        {
                            kill.Add(new DefinitionInfo { BasicBlock = block, Instruction = killedDef });
                        }
                    }
                }

                var gb = BitUtils.GroupByBlockAndTurnIntoInstructions(gen, _ids_by_instruction);
                gen_block = gb;
                // delete duplicates
                kill = kill.Distinct().ToList();
                var kb = BitUtils.GroupByBlockAndTurnIntoInstructions(kill, _ids_by_instruction);
                kill_block = kb;

                Console.WriteLine("Start blocks: " + blocks.Count + "; genBlocks: " + gen_block.Count + "; killBlocks: " + kill_block.Count);
            }

            public ReachingTransferFunc(ControlFlowGraph g, Dictionary<Instruction, int> idByInstruction)
            {
                _ids_by_instruction = idByInstruction;
                var basicBlocks = g.GetCurrentBasicBlocks();
                GetDefs(basicBlocks);
                GetGenKill(basicBlocks);
            }

            public BitArray Transfer(BasicBlock basicBlock, BitArray input) =>
                ApplyTransferFunc(input, basicBlock);

            private BitArray ApplyTransferFunc(BitArray @in, BasicBlock block)
            {
                var gen = gen_block.ContainsKey(block) ? gen_block[block] : new BitArray(@in.Count, false);
                var kill = kill_block.ContainsKey(block) ? kill_block[block] : new BitArray(@in.Count, false);
                return gen.Or(BitUtils.Except(@in, kill));
            }
        }
    }
}
