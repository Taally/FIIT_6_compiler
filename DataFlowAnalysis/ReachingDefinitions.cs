using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using InOutInfo = InOutData<IEnumerable<Instruction>>;
    public class ReachingDefinitions : GenericIterativeAlgorithm<IEnumerable<Instruction>>
    {
        /// <inheritdoc/>
        public override Func<IEnumerable<Instruction>, IEnumerable<Instruction>, IEnumerable<Instruction>> CollectingOperator
            => (a, b) => a.Union(b);

        /// <inheritdoc/>
        public override Func<IEnumerable<Instruction>, IEnumerable<Instruction>, bool> Compare
            => (a, b) => !a.Except(b).Any() && !b.Except(a).Any();

        /// <inheritdoc/>
        public override IEnumerable<Instruction> Init { get => Enumerable.Empty<Instruction>(); protected set { } }

        /// <inheritdoc/>
        public override Func<BasicBlock, IEnumerable<Instruction>, IEnumerable<Instruction>> TransferFunction { get; protected set; }

        public override InOutInfo Execute(ControlFlowGraph graph)
        {
            TransferFunction = new ReachingTransferFunc(graph).Transfer;
            return base.Execute(graph);
        }
    }
}
