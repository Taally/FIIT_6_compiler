using System.Collections.Generic;
using System.Linq;
using System;

namespace SimpleLang
{
    public class ControlFlowGraph
    {
        private List<BasicBlock> _basicBlocks;
        private List<List<(int, BasicBlock)>> _children;
        private List<List<(int, BasicBlock)>> _parents;
        private Dictionary<BasicBlock,int> _blockToVertex;

        public ControlFlowGraph()
        {
            _basicBlocks = new List<BasicBlock>();
            _blockToVertex = new Dictionary<BasicBlock, int>();
        }

        public ControlFlowGraph(List<BasicBlock> basicBlocks)
        {
            _basicBlocks = new List<BasicBlock>(basicBlocks.Count+2);
            _basicBlocks.Add(new BasicBlock(new List<Instruction> { new Instruction("#in", "noop", "", "", "") }));
            _basicBlocks.AddRange(basicBlocks);
            _basicBlocks.Add(new BasicBlock(new List<Instruction> { new Instruction("#out", "noop", "", "", "") }));


            _blockToVertex = _basicBlocks.Select((b, i) => new { b, i }).ToDictionary(v => v.b, v => v.i);

            _children = new List<List<(int, BasicBlock)>>(_basicBlocks.Count);
            _parents = new List<List<(int, BasicBlock)>>(_basicBlocks.Count);

            for (int i = 0; i < _basicBlocks.Count; ++i)
            {
                _children.Add(new List<(int, BasicBlock)>());
                _parents.Add(new List<(int, BasicBlock)>());
            }

            for (int i = 0; i < _basicBlocks.Count; ++i)
            {
                var instructions = _basicBlocks[i].GetInstructions();
                var instr = instructions.Last();
                switch (instr.Operation)
                {
                    case "goto":
                        var gotoOutLabel = instr.Argument1;
                        var gotoOutBlock = _basicBlocks.FindIndex(block =>
                                string.Equals(block.GetInstructions().First().Label, gotoOutLabel));

                        if (gotoOutBlock == -1)
                            throw new Exception($"label {gotoOutLabel} not found");

                        _children[i].Add((gotoOutBlock, _basicBlocks[gotoOutBlock]));
                        _parents[gotoOutBlock].Add((i, _basicBlocks[i]));
                        break;

                    case "ifgoto":
                        var ifgotoOutLabel = instr.Argument2;
                        var ifgotoOutBlock = _basicBlocks.FindIndex(block =>
                                string.Equals(block.GetInstructions().First().Label, ifgotoOutLabel));

                        if (ifgotoOutBlock == -1)
                            throw new Exception($"label {ifgotoOutLabel} not found");

                        _children[i].Add((ifgotoOutBlock, _basicBlocks[ifgotoOutBlock]));
                        _parents[ifgotoOutBlock].Add((i, _basicBlocks[i]));

                        _children[i].Add((i + 1, _basicBlocks[i + 1]));
                        _parents[i + 1].Add((i, _basicBlocks[i]));
                        break;

                    default:
                        if (i < _basicBlocks.Count - 1)
                        {
                            _children[i].Add((i + 1, _basicBlocks[i + 1]));
                            _parents[i + 1].Add((i, _basicBlocks[i]));
                        }
                        break;
                }
            }
        }

        public int VertexOf(BasicBlock block) => _blockToVertex[block];
        public List<BasicBlock> GetCurrentBasicBlocks() => _basicBlocks.ToList();

        public List<(int, BasicBlock)> GetChildrenBasicBlocks(int vertex) => _children[vertex];

        public List<(int, BasicBlock)> GetParentsBasicBlocks(int vertex) => _parents[vertex];
        
        public IEnumerable<Instruction> GetAssigns() =>
            _basicBlocks.Select(b => b.GetInstructions().Where(instr =>
                instr.Operation == "assign" || instr.Operation == "input" ||
                (instr.Operation == "PLUS" && !instr.Result.StartsWith("#"))
            )).SelectMany(i => i);

        public int GetAmountOfAssigns() => GetAssigns().Count();
    }
}