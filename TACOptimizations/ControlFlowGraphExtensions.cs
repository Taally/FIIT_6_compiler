using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang.TACOptimizations
{
    public static class ControlFlowGraphExtensions
    {

        public static List<(int, BasicBlock)> GetChildsBasicBlocks(this ControlFlowGraph graph, BasicBlock block) => 
            graph.GetChildsBasicBlocks(graph.GetCurrentBasicBlocks().IndexOf(block));

        public static List<(int, BasicBlock)> GetParentBasicBlocks(this ControlFlowGraph graph, BasicBlock block) => 
            graph.GetParentBasicBlocks(graph.GetCurrentBasicBlocks().IndexOf(block));
    }
}
