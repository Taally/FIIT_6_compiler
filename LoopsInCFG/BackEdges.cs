using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    /// <summary>
    /// Класс обратных рёбер
    /// </summary>
    public class BackEdges
    {
        /// <summary>
        /// Цвет, которым помечается вершина при исследование CFG на приводимость.
        /// </summary>
        private enum BlockColor
        {
            White,
            Gray,
            Black
        }
        /// <summary>
        /// Обратные ребра графа
        /// </summary>
        /// <returns> Список обратных рёбер (From BasicBlock, To BasicBlock) </returns>
        public IReadOnlyList<(BasicBlock From, BasicBlock To)> BackEdgesFromGraph
        {
            get => enumBackEdges.Select(edge => (edge.From, edge.To)).ToList();
            private set { }
        }
        /// <summary>
        /// Свойство приводимости графа
        /// </summary>
        /// <returns>True - граф приводим, false - граф не приводим  </returns>
        public bool GraphIsReducible => CheckReducibility();
        #region
        private readonly List<Edge> enumBackEdges = new List<Edge>();
        private readonly List<Edge> enumEdgesCFG = new List<Edge>();
        private readonly ControlFlowGraph controlFlowGraph;
        private readonly Dictionary<BasicBlock, BlockColor> BlockColorDictionary = new Dictionary<BasicBlock, BlockColor>();
        public BackEdges(ControlFlowGraph cfg)
        {
            controlFlowGraph = cfg;
            EdgesFromCFG();
            GetBackEdges();
        }
        private void EdgesFromCFG()
        {
            foreach (var block in controlFlowGraph.GetCurrentBasicBlocks())
            {
                foreach (var element in controlFlowGraph
                    .GetChildrenBasicBlocks(controlFlowGraph.VertexOf(block)))
                {
                    enumEdgesCFG.Add(new Edge(block, element.block));
                }
            }
        }
        private void GetBackEdges()
        {
            var dominators = new DominatorTree().GetDominators(controlFlowGraph);
            foreach (var edge in enumEdgesCFG)
            {
                if (dominators[edge.From].ToList().Contains(edge.To))
                {
                    enumBackEdges.Add(new Edge(edge.From, edge.To));
                }
            }
        }
        private bool CheckReducibility()
        {
            foreach (var block in controlFlowGraph.GetCurrentBasicBlocks())
            {
                BlockColorDictionary[block] = BlockColor.White;
            }
            foreach (var block in controlFlowGraph.GetCurrentBasicBlocks())
            {
                if (BlockColorDictionary[block] == BlockColor.White && OpenBlock(block) == false)
                {
                    return false;
                }
            }
            return true;
        }
        private bool OpenBlock(BasicBlock block)
        {
            if (BlockColorDictionary[block] == BlockColor.White)
            {
                BlockColorDictionary[block] = BlockColor.Gray;
            }
            foreach (var child in controlFlowGraph.GetChildrenBasicBlocks(controlFlowGraph.VertexOf(block)))
            {
                var isNotBackEdge = !ContainsEdge(enumBackEdges, new Edge(block, child.block));
                if (isNotBackEdge && BlockColorDictionary[child.block] == BlockColor.Gray)
                {
                    return false;
                }
                if (isNotBackEdge
                    && BlockColorDictionary[child.block] == BlockColor.White
                    && OpenBlock(child.block) == false)
                {
                    return false;
                }
            }
            BlockColorDictionary[block] = BlockColor.Black;
            return true;
        }
        private bool ContainsEdge(List<Edge> edges, Edge edgeGraph)
        {
            foreach (var edge in edges)
            {
                if (edge.From == edgeGraph.From && edge.To == edgeGraph.To)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// Класс ребро
    /// </summary>
    public class Edge
    {
        public BasicBlock From { get; protected set; }
        public BasicBlock To { get; protected set; }
        public Edge(BasicBlock fromNode, BasicBlock toNode)
        {
            From = fromNode;
            To = toNode;
        }
    }
}
