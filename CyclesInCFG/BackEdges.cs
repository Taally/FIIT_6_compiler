using System;
using System.Collections.Generic;
using System.Linq;
namespace SimpleLang.CyclesInCFG
{
    /// <summary>
    /// Класс обратных рёбер
    /// </summary>
    public class BackEdges
    {
        /// <summary>
        /// Обратные ребра графа
        /// </summary>
        /// <returns> Список обратных рёбер (From BasicBlock, To BasicBlock) </returns>
        public IReadOnlyList<(BasicBlock, BasicBlock)> BackEdgesFromGraph
        {
            get => enumBackEdges.Select(edge => (edge.From, edge.To)).ToList();
            private set { }
        }

        private readonly List<Edge> enumBackEdges = new List<Edge>();
        private readonly List<Edge> enumEdgesCFG = new List<Edge>();
        private readonly ControlFlowGraph controlFlowGraph;

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
                    enumEdgesCFG.Add(new Edge(block, element.Item2));
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
    }
    /// <summary>
    /// Класс ребро
    /// </summary>
    internal class Edge
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
