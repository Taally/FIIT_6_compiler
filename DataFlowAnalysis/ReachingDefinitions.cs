using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class ReachingDefinitions
    {
        public InOutData<IEnumerable<Instruction>> Execute(ControlFlowGraph graph)
        {
            var iterativeAlgorithm = new GenericIterativeAlgorithm<IEnumerable<Instruction>>();
            return iterativeAlgorithm.Analyze(graph, new Operation(), new ReachingTransferFunc(graph));
        }

        private class Operation : ICompareOperations<IEnumerable<Instruction>>
        {
            public IEnumerable<Instruction> Lower =>
                new List<Instruction>();

            // I don't even know why it's needed as it's never used
            public IEnumerable<Instruction> Upper =>
                null;

            public (IEnumerable<Instruction>, IEnumerable<Instruction>) Init =>
                (Lower, Lower);

            public IEnumerable<Instruction> Operator(IEnumerable<Instruction> a, IEnumerable<Instruction> b)
                => a.Union(b);
            
            public bool Compare(IEnumerable<Instruction> a, IEnumerable<Instruction> b)
                => !a.Except(b).Any();
        }
    }
}
