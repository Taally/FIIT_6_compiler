using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using InOutInfo = InOutData<IEnumerable<Instruction>>;
    public class ReachingDefinitions
    {
        public InOutInfo Execute(ControlFlowGraph graph) =>
            GenericIterativeAlgorithm<IEnumerable<Instruction>>.Analyze(
                graph,
                new AlgorithmInfo<IEnumerable<Instruction>>
                {
                    CollectingOperator = (a, b) => a.Union(b),
                    Compare = (a, b) => !a.Except(b).Any() && !b.Except(a).Any(),
                    Init = () => Enumerable.Empty<Instruction>(),
                    InitFirst = () => Enumerable.Empty<Instruction>(),
                    TransferFunction = new ReachingTransferFunc(graph).Transfer,
                    Direction = Direction.Forward
                });
    }
}
