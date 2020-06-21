using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class Region
    {
        public List<Region> includedRegions;
        public List<(Region, Region)> edges;
        public BasicBlock Initial;

        public Region(List<Region> _regs = null, List<(Region, Region)> _edges = null, BasicBlock _initial = null)
        {
            includedRegions = _regs;
            edges = _edges;
            Initial = _initial;
        }
    }

    public class CFGregions
    {
        private List<Region> _regions = new List<Region>();
        public IReadOnlyList<Region> Regions => _regions;
        private Dictionary<BasicBlock, int> Block_to_region = new Dictionary<BasicBlock, int>();

        private List<List<BasicBlock>> cycles;
        private HashSet<BasicBlock> blocks;
        private Dictionary<BasicBlock, List<BasicBlock>> childrens = new Dictionary<BasicBlock, List<BasicBlock>>();
        private uint curID = 0;

        public CFGregions(ControlFlowGraph cfg)
        {
            cycles = NaturalLoop.GetAllNaturalLoops(cfg).OrderBy(x => x.Count).Where(x => x.Count > 0).ToList();
            blocks = cfg.GetCurrentBasicBlocks().ToHashSet();
            var i = 0;
            foreach (var elem in blocks)
            {
                var index = cfg.VertexOf(elem);
                if (i++ != index)
                    throw new Exception();
                childrens.Add(elem, cfg.GetChildrenBasicBlocks(index).Select(x => x.block).ToList());
            }
            FindRegions();
        }

        private void FindRegions()
        {
            foreach (var item in blocks)
            {
                _regions.Add(new Region(_initial: item));
                Block_to_region.Add(item, _regions.Count - 1);
                curID++;
            }
            for (var i = 0; i < cycles.Count; i++)
            {
                CollapseCycle(cycles[i]);
            }
            var temp_edges = new List<(Region, Region)>();
            foreach (var entry in childrens)
            {
                foreach (var second in entry.Value)
                {
                    temp_edges.Add((_regions[Block_to_region[entry.Key]], _regions[Block_to_region[second]]));
                }
            }
            _regions.Add(new Region(blocks.Select(x => _regions[Block_to_region[x]]).ToList(), temp_edges));
        }

        private void CollapseCycle(List<BasicBlock> cycle)
        {
            var new_block = new BasicBlock();
            new_block.AddInstruction(new Instruction("", "", curID.ToString(), "", ""));
            childrens.Add(new_block, new List<BasicBlock>());
            var cycle_edges = new List<(BasicBlock, BasicBlock)>();

            foreach (var cur_vertex in blocks)
            {
                if (!cycle.Contains(cur_vertex))
                {
                    var temp = childrens[cur_vertex].ToList();
                    foreach (var child in temp)
                    {
                        if (child == cycle[0])
                        {
                            childrens[cur_vertex].Remove(child);
                            childrens[cur_vertex].Add(new_block);
                        }
                    }
                }
                else
                {
                    var temp = childrens[cur_vertex].ToList();
                    foreach (var child in temp)
                    {
                        if (child == cycle[0])
                        {
                            childrens[new_block].Add(new_block);
                            childrens[cur_vertex].Remove(child);
                        }
                        else if (!cycle.Contains(child))
                        {
                            childrens[new_block].Add(child);
                            childrens[cur_vertex].Remove(child);
                        }
                        else
                        {
                            cycle_edges.Add((cur_vertex, child));
                            childrens[cur_vertex].Remove(child);
                        }
                    }
                }
            }
            blocks.Add(new_block);
            for (var i = 0; i < cycle.Count; i++)
            {
                blocks.Remove(cycle[i]);
                childrens.Remove(cycle[i]);
            }
            for (var i = 0; i < cycles.Count; i++)
            {
                var c = cycles[i].Count;
                cycles[i] = cycles[i].Except(cycle).ToList();
                if (cycles[i].Count != c)
                {
                    cycles[i].Add(new_block);
                }
            }
            var innerRegions = cycle.Select(x => _regions[Block_to_region[x]]).ToList();
            var innerEdged = cycle_edges.Select(x => (_regions[Block_to_region[x.Item1]], _regions[Block_to_region[x.Item2]])).ToList();

            _regions.Add(new Region(innerRegions, innerEdged));
            Block_to_region.Add(new_block, _regions.Count - 1);
            curID++;

            _regions.Add(new Region(new List<Region>() { _regions[Block_to_region[new_block]] },
                                    new List<(Region, Region)>() { (_regions[Block_to_region[new_block]], _regions[Block_to_region[new_block]]) }));

            childrens[new_block].Remove(new_block);
        }
    }
}
