using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class ControlFlowGraph
    {
        private readonly List<BasicBlock> _basicBlocks;
        private readonly List<List<(int vertex, BasicBlock block)>> _children;
        private readonly List<List<(int vertex, BasicBlock block)>> _parents;
        private readonly Dictionary<BasicBlock, int> _blockToVertex;

        private List<int> _nlr;
        public IReadOnlyList<int> PreOrderNumeration => _nlr.AsReadOnly();
        private List<int> _lrn;
        public IReadOnlyList<int> PostOrderNumeration => _lrn.AsReadOnly();

        private List<(int, int)> _dfst;
        public IReadOnlyList<(int from, int to)> DepthFirstSpanningTree => _dfst.AsReadOnly();

        private List<int> _dfn;
        public IReadOnlyList<int> DepthFirstNumeration => _dfn.AsReadOnly();

        public ControlFlowGraph()
        {
            _basicBlocks = new List<BasicBlock>();
            _blockToVertex = new Dictionary<BasicBlock, int>();
            _dfst = new List<(int, int)>();
        }

        public ControlFlowGraph(List<BasicBlock> basicBlocks)
        {
            _basicBlocks = new List<BasicBlock>(basicBlocks.Count + 2)
            {
                new BasicBlock(new List<Instruction> { new Instruction("#in", "noop", "", "", "") })
            };
            _basicBlocks.AddRange(basicBlocks);
            _basicBlocks.Add(new BasicBlock(new List<Instruction> { new Instruction("#out", "noop", "", "", "") }));


            _blockToVertex = _basicBlocks.Select((b, i) => new { b, i }).ToDictionary(v => v.b, v => v.i);

            _children = new List<List<(int, BasicBlock)>>(_basicBlocks.Count);
            _parents = new List<List<(int, BasicBlock)>>(_basicBlocks.Count);

            for (var i = 0; i < _basicBlocks.Count; ++i)
            {
                _children.Add(new List<(int, BasicBlock)>());
                _parents.Add(new List<(int, BasicBlock)>());
            }

            for (var i = 0; i < _basicBlocks.Count; ++i)
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
                        {
                            throw new Exception($"label {gotoOutLabel} not found");
                        }

                        _children[i].Add((gotoOutBlock, _basicBlocks[gotoOutBlock]));
                        _parents[gotoOutBlock].Add((i, _basicBlocks[i]));
                        break;

                    case "ifgoto":
                        var ifgotoOutLabel = instr.Argument2;
                        var ifgotoOutBlock = _basicBlocks.FindIndex(block =>
                                string.Equals(block.GetInstructions().First().Label, ifgotoOutLabel));

                        if (ifgotoOutBlock == -1)
                        {
                            throw new Exception($"label {ifgotoOutLabel} not found");
                        }

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

            DFS();
        }

        private void DFS()
        {
            _dfst = new List<(int, int)>();

            var c = _basicBlocks.Count;
            _lrn = new List<int>(c);
            _nlr = new List<int>(c);
            _dfn = new List<int>(new int[c]);

            var used = new bool[c];

            void dfs(int vertex)
            {
                used[vertex] = true;
                _nlr.Add(vertex);
                foreach ((var v, _) in _children[vertex])
                {
                    if (!used[v])
                    {
                        _dfst.Add((vertex, v));
                        dfs(v);
                    }
                }
                _lrn.Add(vertex);
                _dfn[vertex] = --c;
            }

            dfs(0);
        }

        public int VertexOf(BasicBlock block) => _blockToVertex[block];
        public IReadOnlyList<BasicBlock> GetCurrentBasicBlocks() => _basicBlocks.AsReadOnly();

        public IReadOnlyList<(int vertex, BasicBlock block)> GetChildrenBasicBlocks(int vertex) => _children[vertex].AsReadOnly();

        public IReadOnlyList<(int vertex, BasicBlock block)> GetParentsBasicBlocks(int vertex) => _parents[vertex].AsReadOnly();

        public IEnumerable<Instruction> GetAssigns() =>
            _basicBlocks.Select(b => b.GetInstructions().Where(instr =>
                instr.Operation == "assign" || instr.Operation == "input" ||
                instr.Operation == "PLUS" && !instr.Result.StartsWith("#")
            )).SelectMany(i => i);

        public int GetAmountOfAssigns() => GetAssigns().Count();
    }
}
