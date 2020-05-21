using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SimpleLang
{
    public class ReachingDefinitions
    {
        public InOutInfo Execute(ControlFlowGraph graph)
        {
            // 0. Skipping #in and #out basic blocks
            var basicBlocks = graph.GetCurrentBasicBlocks().Skip(1).Take(graph.GetCurrentBasicBlocks().Count - 2);

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
                    var parents = graph.GetParentsBasicBlocks(block)
                        .Select(z => z.Item2)
                        .Where(bl => bl.GetInstructions()[0].Label != "#in" && bl.GetInstructions()[0].Label != "#out");
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

        private class DefinitionInfo
        {
            public BasicBlock BasicBlock { get; set; }
            public Instruction Instruction { get; set; }
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