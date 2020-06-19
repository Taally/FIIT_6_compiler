using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleLang
{
    public class ControlFlowGraph
    {
        private List<BasicBlock> _basicBlocks;
        private List<List<(int vertex, BasicBlock block)>> _children;
        private List<List<(int vertex, BasicBlock block)>> _parents;
        private Dictionary<BasicBlock, int> _blockToVertex;

        private List<int> _nlr;
        public IReadOnlyList<int> PreOrderNumeration => _nlr;
        private List<int> _lrn;
        public IReadOnlyList<int> PostOrderNumeration => _lrn;

        private List<(int, int)> _dfst;
        public IReadOnlyList<(int from, int to)> DepthFirstSpanningTree => _dfst;

        private List<int> _dfn;
        public IReadOnlyList<int> DepthFirstNumeration => _dfn;

        private List<(int from, int to, EdgeType type)> _classifiedEdges;
        public IReadOnlyList<(int from, int to, EdgeType type)> ClassifiedEdges => _classifiedEdges;

        public ControlFlowGraph()
        {
            _basicBlocks = new List<BasicBlock>();
            _blockToVertex = new Dictionary<BasicBlock, int>();
            _dfst = new List<(int, int)>();
        }

        public ControlFlowGraph(List<BasicBlock> basicBlocks)
        {
            ConstructedCFG(basicBlocks);
            DFS();
            ConstructedCFG(UnreachableCodeElimination());
            DFS();
        }

        private void ConstructedCFG(List<BasicBlock> basicBlocks)
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
        }

        private List<BasicBlock> UnreachableCodeElimination()
        {
            var tmpBasicBlock = new List<BasicBlock>(_basicBlocks);

            for (int i = 1; i < _dfn.Count-1; i++)
            {
                if (_dfn[i] == 0)
                {
                    tmpBasicBlock[i] = null;
                }
            }
            tmpBasicBlock.RemoveAll(x => x == null);
            return tmpBasicBlock.Skip(1).Take(tmpBasicBlock.Count - 2).ToList();
        }

        public enum EdgeType
        {
            Advancing,
            Retreating,
            Cross
        }

        private enum VertexStatus
        {
            Init,
            InProgress,
            Done
        }

        private void DFS()
        {
            _dfst = new List<(int, int)>();

            var c = _basicBlocks.Count;
            _lrn = new List<int>(c);
            _nlr = new List<int>(c);
            _dfn = new List<int>(new int[c]);

            _classifiedEdges = new List<(int, int, EdgeType)>();
            var vertexStatuses = Enumerable.Repeat(VertexStatus.Init, c).ToArray();
            var pre = new int[c];
            var counter = 0;

            void dfs(int vertex)
            {
                vertexStatuses[vertex] = VertexStatus.InProgress;
                pre[vertex] = counter++;
                _nlr.Add(vertex);
                foreach (var (v, _) in _children[vertex])
                {
                    switch (vertexStatuses[v])
                    {
                        case VertexStatus.Init:
                            _classifiedEdges.Add((vertex, v, EdgeType.Advancing));
                            _dfst.Add((vertex, v));
                            dfs(v);
                            break;
                        case VertexStatus.InProgress:
                            _classifiedEdges.Add((vertex, v, EdgeType.Retreating));
                            break;
                        case VertexStatus.Done:
                            var edgeType = (pre[v] < pre[vertex]) ? EdgeType.Cross : EdgeType.Advancing;
                            _classifiedEdges.Add((vertex, v, edgeType));
                            break;
                    }
                }
                vertexStatuses[vertex] = VertexStatus.Done;
                _lrn.Add(vertex);
                _dfn[vertex] = --c;
            }

            dfs(0);
        }

        public int VertexOf(BasicBlock block) => _blockToVertex[block];
        public IReadOnlyList<BasicBlock> GetCurrentBasicBlocks() => _basicBlocks;

        public IReadOnlyList<(int vertex, BasicBlock block)> GetChildrenBasicBlocks(int vertex) => _children[vertex];

        public IReadOnlyList<(int vertex, BasicBlock block)> GetParentsBasicBlocks(int vertex) => _parents[vertex];

        public IEnumerable<Instruction> GetAssigns() =>
            _basicBlocks.Select(b => b.GetInstructions().Where(instr =>
                instr.Operation == "assign" || instr.Operation == "input" ||
                instr.Operation == "PLUS" && !instr.Result.StartsWith("#")
            )).SelectMany(i => i);

        public int GetAmountOfAssigns() => GetAssigns().Count();
    }
}
