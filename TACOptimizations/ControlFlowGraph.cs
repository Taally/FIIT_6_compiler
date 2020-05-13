using System.Collections.Generic;
using System.Linq;
using System;

namespace SimpleLang
{
    public class ControlFlowGraph
    {
        private List<BasicBlock> _basicBlocks;
        private List<List<(int, BasicBlock)>> _childs;
        private List<List<(int, BasicBlock)>> _parents;

        public ControlFlowGraph()
        {
            _basicBlocks = new List<BasicBlock>();
        }

        public ControlFlowGraph(List<BasicBlock> basicBlocks)
        {
            _basicBlocks = basicBlocks;

            _childs = new List<List<(int, BasicBlock)>>(basicBlocks.Count);
            _parents = new List<List<(int, BasicBlock)>>(basicBlocks.Count);

            for (int i = 0; i < basicBlocks.Count; ++i)
            {
                _childs.Add(new List<(int, BasicBlock)>());
                _parents.Add(new List<(int, BasicBlock)>());
            }

            for (int i = 0; i < basicBlocks.Count; ++i)
            {
                var instructions = basicBlocks[i].GetInstructions();
                var instr = instructions.Last();
                switch (instr.Operation)
                {
                    case "goto":
                        var gotoOutLabel = instr.Argument1;
                        var gotoOutBlock = basicBlocks.FindIndex(block =>
                                string.Equals(block.GetInstructions().First().Label, gotoOutLabel));

                        if (gotoOutBlock == -1)
                              throw new Exception($"label {gotoOutLabel} not found");

                        _childs[i].Add((gotoOutBlock, basicBlocks[gotoOutBlock]));
                        _parents[gotoOutBlock].Add((i, basicBlocks[i]));
                        break;

                    case "ifgoto":
                        var ifgotoOutLabel = instr.Argument2;
                        var ifgotoOutBlock = basicBlocks.FindIndex(block =>
                                string.Equals(block.GetInstructions().First().Label, ifgotoOutLabel));

                        if (ifgotoOutBlock == -1)
                            throw new Exception($"label {ifgotoOutLabel} not found");

                        _childs[i].Add((ifgotoOutBlock, basicBlocks[ifgotoOutBlock]));
                        _parents[ifgotoOutBlock].Add((i, basicBlocks[i]));

                        _childs[i].Add((i + 1, basicBlocks[i + 1]));
                        _parents[i + 1].Add((i, basicBlocks[i]));
                        break;

                    default:
                        _childs[i].Add((i + 1, basicBlocks[i + 1]));
                        _parents[i + 1].Add((i, basicBlocks[i]));
                        break;
                }
            }
        }

        public List<BasicBlock> GetCurrentBasicBlocks() => _basicBlocks;

        public List<(int, BasicBlock)> GetChildsBasicBlocks(int index) => _childs[index];

        public List<(int, BasicBlock)> GetParentBasicBlocks(int index) => _parents[index];
    }
}