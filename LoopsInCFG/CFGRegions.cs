using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class Region
    {
        public IReadOnlyCollection<Region> includedRegions;
        public IReadOnlyCollection<(Region, Region)> edges;
        public BasicBlock Initial;

        public Region(IReadOnlyCollection<Region> _regs = null, IReadOnlyCollection<(Region, Region)> _edges = null, BasicBlock _initial = null)
        {
            includedRegions = _regs;
            edges = _edges;
            Initial = _initial;
        }
    }

    public class CFGregions
    {
        private readonly List<Region> _regions = new List<Region>();
        public IReadOnlyList<Region> Regions => _regions;
        private readonly Dictionary<BasicBlock, int> Block_to_region = new Dictionary<BasicBlock, int>();

        private readonly List<List<BasicBlock>> cycles;
        private readonly HashSet<BasicBlock> blocks;
        private readonly Dictionary<BasicBlock, List<BasicBlock>> children = new Dictionary<BasicBlock, List<BasicBlock>>();
        private uint curID = 0;

        public CFGregions(ControlFlowGraph cfg)
        {
            cycles = NaturalLoop.GetAllNaturalLoops(cfg).OrderBy(x => x.Count).Where(x => x.Count > 0).Select(x => new List<BasicBlock>(x)).ToList();
            blocks = cfg.GetCurrentBasicBlocks().ToHashSet();
            var i = 0;
            foreach (var elem in blocks)
            {
                var index = cfg.VertexOf(elem);
                if (i++ != index)
                {
                    throw new Exception();
                }

                children.Add(elem, cfg.GetChildrenBasicBlocks(index).Select(x => x.block).ToList());
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
            for (var i = 0; i < cycles.Count; ++i) // regular for because CollapseCycle can modify
            {
                CollapseCycle(cycles[i]);
            }
            var temp_edges = new List<(Region, Region)>();
            foreach (var entry in children)
            {
                foreach (var second in entry.Value)
                {
                    temp_edges.Add((_regions[Block_to_region[entry.Key]], _regions[Block_to_region[second]]));
                }
            }
            _regions.Add(new Region(blocks.Select(x => _regions[Block_to_region[x]]).ToList(), temp_edges));
        }

        private void CollapseCycle(IReadOnlyCollection<BasicBlock> cycle)
        {
            var new_block = new BasicBlock();
            new_block.AddInstruction(new Instruction("", "", curID.ToString(), "", ""));
            children.Add(new_block, new List<BasicBlock>());
            var cycle_edges = new List<(BasicBlock, BasicBlock)>();

            foreach (var cur_vertex in blocks)
            {
                if (!cycle.Contains(cur_vertex))
                {
                    var temp = children[cur_vertex].ToList();
                    foreach (var child in temp)
                    {
                        if (child == cycle.First())
                        {
                            children[cur_vertex].Remove(child);
                            children[cur_vertex].Add(new_block);
                        }
                    }
                }
                else
                {
                    var temp = children[cur_vertex].ToList();
                    foreach (var child in temp)
                    {
                        if (child == cycle.First())
                        {
                            children[new_block].Add(new_block);
                            children[cur_vertex].Remove(child);
                        }
                        else if (!cycle.Contains(child))
                        {
                            children[new_block].Add(child);
                            children[cur_vertex].Remove(child);
                        }
                        else
                        {
                            cycle_edges.Add((cur_vertex, child));
                            children[cur_vertex].Remove(child);
                        }
                    }
                }
            }
            blocks.Add(new_block);
            foreach (var bblock in cycle)
            {
                blocks.Remove(bblock);
                children.Remove(bblock);
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

            children[new_block].Remove(new_block);
        }
    }
}
