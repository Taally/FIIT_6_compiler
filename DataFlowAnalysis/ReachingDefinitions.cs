using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using InOutInfo = InOutData<Instruction>;
    public class ReachingDefinitions
    {
        public InOutInfo Execute(ControlFlowGraph graph)
        {
            var iterativeAlgorithm = new GenericIterativeAlgorithm<Instruction>();
            return iterativeAlgorithm.Analyze(graph, new Operation(), new ReachingTransferFunc(graph));
        }

        private class Operation : ICompareOperations<Instruction>
        {
            public IEnumerable<Instruction> Lower =>
                new List<Instruction>();

            public IEnumerable<Instruction> Upper =>
                throw new NotImplementedException("I don't even know why it's needed as it's never used.");

            public (IEnumerable<Instruction>, IEnumerable<Instruction>) Init =>
                (Lower, Lower);

            public (IEnumerable<Instruction>, IEnumerable<Instruction>) EnterInit => Init;

            public IEnumerable<Instruction> Operator(IEnumerable<Instruction> a, IEnumerable<Instruction> b) =>
                a.Union(b);
            
            public bool Compare(IEnumerable<Instruction> a, IEnumerable<Instruction> b) =>
                !a.Except(b).Any();
        }
    }
}
