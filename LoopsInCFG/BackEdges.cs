using System;
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
        public bool GraphIsReducible { get; }
        private readonly List<Edge> enumBackEdges = new List<Edge>();
        private readonly List<Edge> enumEdgesCFG = new List<Edge>();
        private readonly ControlFlowGraph controlFlowGraph;
        private IReadOnlyList<BasicBlock> BasicBlocks { get; }
        private readonly Dictionary<BasicBlock, BlockColor> BlockColorDictionary = new Dictionary<BasicBlock, BlockColor>();
        public BackEdges(ControlFlowGraph cfg)
        {
            controlFlowGraph = cfg;
            BasicBlocks = controlFlowGraph.GetCurrentBasicBlocks();
            EdgesFromCFG();
            GetBackEdges();
            GraphIsReducible = CheckReducibility();
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
            foreach (var block in BasicBlocks)
            {
                BlockColorDictionary[block] = BlockColor.White;
            }
            foreach (var block in BasicBlocks)
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
            var blockNumber = controlFlowGraph.VertexOf(block);
            foreach (var child in controlFlowGraph.GetChildrenBasicBlocks(blockNumber))
            {
                var childBlock = child.block;
                var isNotBackEdge = !enumBackEdges.ContainsEdge(block, childBlock);
                if (isNotBackEdge && BlockColorDictionary[childBlock] == BlockColor.Gray)
                {
                    return false;
                }
                if (isNotBackEdge
                    && BlockColorDictionary[childBlock] == BlockColor.White
                    && OpenBlock(childBlock) == false)
                {
                    return false;
                }
            }
            BlockColorDictionary[block] = BlockColor.Black;
            return true;
        }
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

    public static class ListExtension
    {
        public static bool ContainsEdge(this List<Edge> edges, BasicBlock f, BasicBlock t)
        {
            foreach (var edge in edges)
            {
                if (edge.From == f && edge.To == t)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
