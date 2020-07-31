using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class Region
    {
        public IReadOnlyCollection<Region> IncludedRegions;
        public IReadOnlyCollection<(Region, Region)> Edges;
        public BasicBlock Initial;

        public Region(IReadOnlyCollection<Region> regions = null, IReadOnlyCollection<(Region, Region)> edges = null, BasicBlock initial = null)
        {
            IncludedRegions = regions;
            Edges = edges;
            Initial = initial;
        }

        public void Print(int indent = 0)
        {
            if (IncludedRegions == null)
            {
                foreach (var item in Initial.GetInstructions())
                {
                    Console.WriteLine(new string(' ', indent) + item.ToString());
                }
                Console.WriteLine();
            }
            else
            {
                foreach (var item in IncludedRegions)
                {
                    item.Print(indent + 4);
                }
            }
        }
    }

    public class CFGRegions
    {
        private readonly List<Region> regions = new List<Region>();
        public IReadOnlyList<Region> Regions => regions;
        private readonly Dictionary<BasicBlock, int> BlockToRegion = new Dictionary<BasicBlock, int>();

        private readonly List<List<BasicBlock>> cycles;
        private readonly HashSet<BasicBlock> blocks;
        private readonly Dictionary<BasicBlock, List<BasicBlock>> children = new Dictionary<BasicBlock, List<BasicBlock>>();
        private uint curID = 0;

        public CFGRegions(ControlFlowGraph cfg)
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
                regions.Add(new Region(initial: item));
                BlockToRegion.Add(item, regions.Count - 1);
                curID++;
            }
            for (var i = 0; i < cycles.Count; ++i) // regular for because CollapseCycle can modify
            {
                CollapseCycle(cycles[i]);
            }
            var tempEdges = new List<(Region, Region)>();
            foreach (var entry in children)
            {
                foreach (var second in entry.Value)
                {
                    tempEdges.Add((regions[BlockToRegion[entry.Key]], regions[BlockToRegion[second]]));
                }
            }
            regions.Add(new Region(blocks.Select(x => regions[BlockToRegion[x]]).ToList(), tempEdges));
        }

        private void CollapseCycle(IReadOnlyCollection<BasicBlock> cycle)
        {
            var bodyBlock = new BasicBlock();
            bodyBlock.AddInstruction(new Instruction("", "", curID.ToString(), "", ""));
            children.Add(bodyBlock, new List<BasicBlock>());
            var cycleEdges = new List<(BasicBlock, BasicBlock)>();

            foreach (var curVertex in blocks)
            {
                if (!cycle.Contains(curVertex))
                {
                    var temp = children[curVertex].ToList();
                    foreach (var child in temp)
                    {
                        if (child == cycle.First())
                        {
                            children[curVertex].Remove(child);
                            children[curVertex].Add(bodyBlock);
                        }
                    }
                }
                else
                {
                    var temp = children[curVertex].ToList();
                    foreach (var child in temp)
                    {
                        if (child == cycle.First())
                        {
                            children[bodyBlock].Add(bodyBlock);
                            children[curVertex].Remove(child);
                        }
                        else if (!cycle.Contains(child))
                        {
                            children[bodyBlock].Add(child);
                            children[curVertex].Remove(child);
                        }
                        else
                        {
                            cycleEdges.Add((curVertex, child));
                            children[curVertex].Remove(child);
                        }
                    }
                }
            }
            blocks.Add(bodyBlock);
            foreach (var bblock in cycle)
            {
                blocks.Remove(bblock);
                children.Remove(bblock);
            }
            var innerRegions = cycle.Select(x => regions[BlockToRegion[x]]).ToList();
            var innerEdged = cycleEdges.Select(x => (regions[BlockToRegion[x.Item1]], regions[BlockToRegion[x.Item2]])).ToList();

            regions.Add(new Region(innerRegions, innerEdged));
            BlockToRegion.Add(bodyBlock, regions.Count - 1);
            curID++;

            // add new node
            var cycleBlock = new BasicBlock();
            cycleBlock.AddInstruction(new Instruction("", "", curID.ToString(), "", ""));
            blocks.Add(cycleBlock);
            children.Add(cycleBlock, new List<BasicBlock>());

            // clear old node
            children[bodyBlock].Remove(bodyBlock);
            foreach (var child in children[bodyBlock])
            {
                children[cycleBlock].Add(child);
            }

            foreach (var childList in children)
            {
                if (childList.Value.Remove(bodyBlock))
                {
                    childList.Value.Add(cycleBlock);
                }
            }

            regions.Add(new Region(new List<Region>() { regions[BlockToRegion[bodyBlock]] },
                                    new List<(Region, Region)>() { (regions[BlockToRegion[bodyBlock]], regions[BlockToRegion[bodyBlock]]) }));
            BlockToRegion.Add(cycleBlock, regions.Count - 1);
            children.Remove(bodyBlock);
            blocks.Remove(bodyBlock);
            curID++;

            for (var i = 0; i < cycles.Count; i++)
            {
                var c = cycles[i].Count;
                cycles[i] = cycles[i].Except(cycle).ToList();
                if (cycles[i].Count != c)
                {
                    cycles[i].Add(cycleBlock);
                }
            }
        }
    }
}
