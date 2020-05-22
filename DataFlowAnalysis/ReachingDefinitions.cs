using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class ReachingDefinitions
    {
        public InOutInfo Execute(ControlFlowGraph graph)
        {
            var basicBlocks = graph.GetCurrentBasicBlocks();
            var transferFunc = new ReachingTransferFunc(graph);
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
                    var parents = graph.GetParentsBasicBlocks(block).Select(z => z.Item2);
                    resultIn[block] = new List<Instruction>(parents.SelectMany(b => resultOut[b]).Distinct());
                    var outNew = transferFunc.ApplyTransferFunc(resultIn[block], block);
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

        public class Operation : ICompareOperations<IEnumerable<Instruction>>
        {
            List<Instruction> _instructions;
            public Operation(List<Instruction> instructions)
                => _instructions = instructions.Where(x => x.Operation == "assign").ToList();
            
            public IEnumerable<Instruction> Lower => new List<Instruction>();

            public IEnumerable<Instruction> Upper => _instructions;

            public (IEnumerable<Instruction>, IEnumerable<Instruction>) Init()
                => (Lower, Lower);

            public IEnumerable<Instruction> Operator(IEnumerable<Instruction> a, IEnumerable<Instruction> b)
                => a.Union(b);
            
            public bool Compare(IEnumerable<Instruction> a, IEnumerable<Instruction> b)
                => !a.Except(b).Any();
        }
    }
}