using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using InOutInfo = InOutData<IEnumerable<BasicBlock>>;
    public class DominatorTree
    {
        private class TransferFunc : ITransFunc<IEnumerable<BasicBlock>>
        {
            public IEnumerable<BasicBlock> Transfer(BasicBlock basicBlock, IEnumerable<BasicBlock> input) =>
                input.Union(Enumerable.Repeat(basicBlock, 1));
        }

        public InOutInfo Execute(ControlFlowGraph graph)
        {
            var iterativeAlgorithm = new GenericIterativeAlgorithm<IEnumerable<BasicBlock>>();
            return iterativeAlgorithm.Analyze(
                graph,
                new Operation()
                {
                    Lower = graph.GetCurrentBasicBlocks(),
                    EnterInit = (graph.GetCurrentBasicBlocks().Take(1), graph.GetCurrentBasicBlocks().Take(1)),
                },
                new TransferFunc());
        }

        private class Operation : ICompareOperations<IEnumerable<BasicBlock>>
        {
            public IEnumerable<BasicBlock> Lower { get; set; }

            public IEnumerable<BasicBlock> Upper =>
                throw new NotImplementedException("I don't even know why it's needed as it's never used.");

            public (IEnumerable<BasicBlock>, IEnumerable<BasicBlock>) Init =>
                (Lower, Lower);

            public (IEnumerable<BasicBlock>, IEnumerable<BasicBlock>) EnterInit { get; set; }

            public IEnumerable<BasicBlock> Operator(IEnumerable<BasicBlock> a, IEnumerable<BasicBlock> b) =>
                a.Intersect(b);

            public bool Compare(IEnumerable<BasicBlock> a, IEnumerable<BasicBlock> b) =>
                !a.Except(b).Any();
        }
    }
}
