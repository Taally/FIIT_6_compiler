using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using InOutInfo = InOutData<IEnumerable<BasicBlock>>;
    public class DominatorTree
    {
        public InOutInfo Execute(ControlFlowGraph graph) =>
            GenericIterativeAlgorithm<IEnumerable<BasicBlock>>.Analyze(
                graph,
                new AlgorithmInfo<IEnumerable<BasicBlock>>
                {
                    CollectingOperator = (x, y) => x.Intersect(y),
                    Compare = (x, y) => !x.Except(y).Any() && !y.Except(x).Any(),
                    Init = () => graph.GetCurrentBasicBlocks(),
                    InitFirst = () => graph.GetCurrentBasicBlocks().Take(1),
                    TransferFunction = (block, blockList) => blockList.Union(new[] { block }),
                    Direction = Direction.Forward
                });
    }
}
