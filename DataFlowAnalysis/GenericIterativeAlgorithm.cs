using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class InOutData<T> : Dictionary<BasicBlock, (T In, T Out)>
        where T : IEnumerable
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("++++");
            foreach (var kv in this)
            {
                sb.AppendLine(kv.Key + ":\n" + kv.Value);
            }
            sb.AppendLine("++++");
            return sb.ToString();
        }
        
        public InOutData() { }
        
        public InOutData(Dictionary<BasicBlock, (T, T)> dictionary)
        {
            foreach (var b in dictionary)
                this[b.Key] = b.Value;
        }
    }

    public enum Direction { Forward, Backward }

    public class AlgorithmInfo<T> where T : IEnumerable
    {
        // оператор сбора
        public Func<T, T, T> CollectingOperator { get; set; }

        // сравнение двух последовательностей (условие продолжения цикла)
        public Func<T, T, bool> Compare { get; set; }

        // начальное значение для всех блоков, кроме первого
        public Func<T> Init { get; set; }

        // начальное значение для первого блока
        // (при движении с конца - для последнего)
        public Func<T> InitFirst { get; set; }

        // передаточная функция
        public Func<BasicBlock, T, T> TransferFunction { get; set; }

        // направление
        public Direction Direction { get; set; }
    }

    public static class GenericIterativeAlgorithm<T> where T : IEnumerable
    {
        public static InOutData<T> Analyze(
            ControlFlowGraph graph, 
            AlgorithmInfo<T> info)
        {
            var start = info.Direction == Direction.Backward 
                ? graph.GetCurrentBasicBlocks().Last() 
                : graph.GetCurrentBasicBlocks().First();
            var blocks = graph.GetCurrentBasicBlocks().Except(new[] { start });

            var data = new InOutData<T>
            {
                [start] = (info.InitFirst(), info.InitFirst())
            };
            foreach (var block in blocks)
                data[block] = (info.Init(), info.Init());

            Func<BasicBlock, IEnumerable<BasicBlock>> getPreviousBlocks = info.Direction == Direction.Backward
                ? (Func<BasicBlock, IEnumerable<BasicBlock>>)(x => graph.GetChildrenBasicBlocks(graph.VertexOf(x)).Select(z => z.Item2))
                : x => graph.GetParentsBasicBlocks(graph.VertexOf(x)).Select(z => z.Item2);
            Func<BasicBlock, T> getDataValue = info.Direction == Direction.Backward
                ? (Func<BasicBlock, T>)(x => data[x].In)
                : x => data[x].Out;
            Func<T, T, (T, T)> combine = info.Direction == Direction.Backward
                ? (Func<T, T, (T, T)>)((x, y) => (y, x))
                : (x, y) => (x, y);

            var outChanged = true;
            while (outChanged)
            {
                outChanged = false;
                foreach (var block in blocks)
                {
                    var inset = getPreviousBlocks(block).Aggregate(info.Init(), (x, y) => info.CollectingOperator(x, getDataValue(y)));
                    var outset = info.TransferFunction(block, inset);

                    if (!info.Compare(outset, getDataValue(block)))
                    {
                        outChanged = true;
                    }
                    data[block] = combine(inset, outset);
                }
            }
            return data;
        }
    }
}
