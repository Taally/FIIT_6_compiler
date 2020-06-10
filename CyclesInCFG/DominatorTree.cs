using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using InOutInfo = InOutData<IEnumerable<BasicBlock>>;

    public class DominatorTree : GenericIterativeAlgorithm<IEnumerable<BasicBlock>>
    {
        /// <inheritdoc/>
        public override Func<IEnumerable<BasicBlock>, IEnumerable<BasicBlock>, IEnumerable<BasicBlock>> CollectingOperator
            => (x, y) => x.Intersect(y);

        /// <inheritdoc/>
        public override Func<IEnumerable<BasicBlock>, IEnumerable<BasicBlock>, bool> Compare
            => (x, y) => !x.Except(y).Any() && !y.Except(x).Any();

        /// <inheritdoc/>
        public override IEnumerable<BasicBlock> Init { get; protected set; }

        /// <inheritdoc/>
        public override IEnumerable<BasicBlock> InitFirst { get; protected set; }

        /// <inheritdoc/>
        public override Func<BasicBlock, IEnumerable<BasicBlock>, IEnumerable<BasicBlock>> TransferFunction
        {
            get => (block, blockList) => blockList.Union(new[] { block });
            protected set { }
        }

        /// <inheritdoc/>
        public override InOutInfo Execute(ControlFlowGraph graph)
        {
            Init = graph.GetCurrentBasicBlocks();
            InitFirst = graph.GetCurrentBasicBlocks().Take(1);
            return base.Execute(graph);
        }
    }
}
