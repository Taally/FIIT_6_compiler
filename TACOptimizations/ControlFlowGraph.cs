using System.Collections.Generic;

namespace SimpleLang
{
    public class ControlFlowGraph
    {
        private List<BasicBlock> _basicBlocks;

        private List<List<int>> _children;

        private List<List<int>> _parent;
        
        public ControlFlowGraph()
        {
            _basicBlocks = new List<BasicBlock>();
        }
        
        public ControlFlowGraph(List<BasicBlock> basicBlocks)
        {
            _basicBlocks = basicBlocks;
        }

        public List<BasicBlock> GetCurrentBasicBlocks() => _basicBlocks;

        public List<BasicBlock> GetChildrenBasicBlock(int index)
        {
            var result = new List<BasicBlock>();
            for (int j = 0; j < _children[index].Count; j++)
                if (_children[index][j] == 1)
                    result.Add(_basicBlocks[j]);
            return result;
        }

        public List<BasicBlock> GetParentBasicBlock(int index)
        {
            var result = new List<BasicBlock>();
            for (int j = 0; j < _children[index].Count; j++)
                if (_parent[index][j] == 1)
                    result.Add(_basicBlocks[j]);
            return result;
        }

    }
}