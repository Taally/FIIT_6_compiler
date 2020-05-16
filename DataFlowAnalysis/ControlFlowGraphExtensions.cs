using System.Collections.Generic;

namespace SimpleLang
{
    public static class ControlFlowGraphExtensions
    {
        public static List<(int, BasicBlock)> GetChildrenBasicBlocks(this ControlFlowGraph graph, BasicBlock block) => 
            graph.GetChildrenBasicBlocks(graph.GetCurrentBasicBlocks().IndexOf(block));

        public static List<(int, BasicBlock)> GetParentsBasicBlocks(this ControlFlowGraph graph, BasicBlock block) => 
            graph.GetParentsBasicBlocks(graph.GetCurrentBasicBlocks().IndexOf(block));
    }
}
