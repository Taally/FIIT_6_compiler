using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class NaturalСycle
    {
        /// <summary>
        /// Принимает Граф потока данных и по нему ищет все естественные циклы
        /// </summary>
        /// <param name="cfg">CFG</param>
        /// <returns>
        /// Вернет все натуральыне циклы
        /// </returns>
        public static List<List<BasicBlock>> GetAllNaturalCycles(ControlFlowGraph cfg)
        {
            List<List<BasicBlock>> natCycles = new List<List<BasicBlock>>();
            var allEdges = new BackEdges(cfg);
            var StraightEdges = allEdges.GetEnumEdges();

            foreach (var x in allEdges.BackEdgesFromGraph)
            {
                if (cfg.VertexOf(x.Item2) > 0)
                {
                    var tmp = new List<BasicBlock>();
                    for (int i = cfg.VertexOf(x.Item2); i < cfg.VertexOf(x.Item1) + 1; i++)
                    {
                        if (!tmp.Contains(StraightEdges[i].From))
                            tmp.Add(StraightEdges[i].From);
                    }

                    natCycles.Add(tmp);
                }
            }

            return natCycles.Where(cycle => IsNaturalCycle(cycle, cfg)).ToList();

        }

        /// <summary>
        /// Првоерка цикла на естественность
        /// </summary>
        /// <param name="cycle">Проверяемый цикл</param>
        /// <param name="cfg">CFG</param>
        /// <returns>
        /// Вернет флаг естественнен ли он
        /// </returns>
        private static bool IsNaturalCycle(List<BasicBlock> cycle, ControlFlowGraph cfg)
        {
            for (int i = 1; i < cycle.Count; i++)
            {
                var parents = cfg.GetParentsBasicBlocks(cfg.VertexOf(cycle[i]));
                if (parents.Count > 1)
                {
                    foreach (var parent in parents.Select(x => x.Item2))
                        if (!cycle.Contains(parent))
                            return false;
                }
            }
            
            return true;
        }
    }
}
