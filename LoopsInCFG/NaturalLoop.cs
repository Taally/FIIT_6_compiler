using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class NaturalLoop
    {
        /// <summary>
        /// Принимает Граф потока данных и по нему ищет все естественные циклы
        /// </summary>
        /// <param name="cfg">Граф потока управления</param>
        /// <returns>
        /// Вернет все натуральные циклы
        /// </returns>
        public static List<List<BasicBlock>> GetAllNaturalLoops(ControlFlowGraph cfg)
        {
            var natLoops = new List<List<BasicBlock>>();
            var allEdges = new BackEdges(cfg);
            var ForwardEdges = cfg.GetCurrentBasicBlocks();

            foreach (var (From, To) in allEdges.BackEdgesFromGraph)
            {
                if (cfg.VertexOf(To) > 0)
                {
                    var tmp = new List<BasicBlock>();
                    for (var i = cfg.VertexOf(To); i < cfg.VertexOf(From) + 1; i++)
                    {
                        if (!tmp.Contains(ForwardEdges[i]))
                        {
                            tmp.Add(ForwardEdges[i]);
                        }
                    }

                    natLoops.Add(tmp);
                }
            }

            return natLoops.Where(loop => IsNaturalLoop(loop, cfg)).ToList();

        }

        /// <summary>
        /// Првоерка цикла на естественность
        /// </summary>
        /// <param name="loop">Проверяемый цикл</param>
        /// <param name="cfg">Граф потока управления</param>
        /// <returns>
        /// Вернет флаг, естественнен ли он
        /// </returns>
        private static bool IsNaturalLoop(List<BasicBlock> loop, ControlFlowGraph cfg)
        {
            for (var i = 1; i < loop.Count; i++)
            {
                var parents = cfg.GetParentsBasicBlocks(cfg.VertexOf(loop[i]));
                if (parents.Count > 1)
                {
                    foreach (var parent in parents.Select(x => x.block))
                    {
                        if (!loop.Contains(parent))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
