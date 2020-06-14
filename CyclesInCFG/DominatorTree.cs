using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using DominatorInfo = Dictionary<BasicBlock, IEnumerable<BasicBlock>>;

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
        public new Tree Execute(ControlFlowGraph graph)
        {
            var start = graph.GetCurrentBasicBlocks().First();
            var treeLayers = GetDominators(graph)
                .Where(z => z.Key != start)
                .GroupBy(z => z.Value.Count())
                .OrderBy(z => z.Key);

            var tree = new Tree(start);
            var prevLayer = new List<BasicBlock>(new[] { start });

            foreach (var layer in treeLayers)
            {
                var currLayer = layer.ToDictionary(z => z.Key, z => z.Value);
                foreach (var block in currLayer)
                {
                    var parent = prevLayer.Single(z => block.Value.Contains(z));
                    tree.AddNode(block.Key, parent);
                }
                prevLayer = currLayer.Keys.ToList();
            }

            return tree;
        }

        /// <summary>
        /// Получить доминаторы для каждого базового блока
        /// </summary>
        /// <param name="graph"> Граф потоков управления </param>
        /// <returns> Словарь базовый блок - доминаторы </returns>
        public DominatorInfo GetDominators(ControlFlowGraph graph)
        {
            Init = graph.GetCurrentBasicBlocks();
            InitFirst = graph.GetCurrentBasicBlocks().Take(1);
            return base.Execute(graph).ToDictionary(z => z.Key, z => z.Value.Out);
        }

        /// <summary>
        /// Класс дерева
        /// </summary>
        public class Tree
        {
            private readonly List<BasicBlock> nodes = new List<BasicBlock>();

            private readonly Dictionary<BasicBlock, List<BasicBlock>> children = new Dictionary<BasicBlock, List<BasicBlock>>();

            private readonly Dictionary<BasicBlock, BasicBlock> parents = new Dictionary<BasicBlock, BasicBlock>();

            /// <summary>
            /// Получить потомков узла
            /// </summary>
            /// <param name="node"> Узел </param>
            /// <returns> Потомки </returns>
            public IReadOnlyCollection<BasicBlock> Children(BasicBlock node)
            {
                if (!nodes.Contains(node))
                {
                    throw new ArgumentException("The tree doesn't contain the node");
                }
                return children.TryGetValue(node, out var result) ? result : new List<BasicBlock>();
            }

            /// <summary>
            /// Получить предка узла
            /// </summary>
            /// <param name="node"> Узел </param>
            /// <returns> Предок </returns>
            public BasicBlock Parent(BasicBlock node)
            {
                if (!nodes.Contains(node))
                {
                    throw new ArgumentException("The tree doesn't contain the node");
                }
                return parents.TryGetValue(node, out var result) ? result : null;
            }

            /// <summary>
            /// Создать дерево
            /// </summary>
            /// <param name="root"> Корень </param>
            public Tree(BasicBlock root) => nodes.Add(root);

            /// <summary>
            /// Добавить узел в дерево
            /// </summary>
            /// <param name="node"> Узел </param>
            /// <param name="parent"> Предок узла </param>
            public void AddNode(BasicBlock node, BasicBlock parent)
            {
                if (nodes.Contains(node))
                {
                    throw new ArgumentException("Node has already been added to the tree");
                }
                nodes.Add(node);
                parents[node] = parent;
                if (!children.ContainsKey(parent))
                {
                    children[parent] = new List<BasicBlock>();
                }
                children[parent].Add(node);
            }
        }
    }
}
