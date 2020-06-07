using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class InOutData<T> : Dictionary<BasicBlock, (IEnumerable<T> In, IEnumerable<T> Out)>
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
    }

    public enum Pass { Forward, Backward }

    public interface ICompareOperations<T>
    {
        // пересечение или объединение 
        IEnumerable<T> Operator(IEnumerable<T> a, IEnumerable<T> b);

        bool Compare(IEnumerable<T> a, IEnumerable<T> b);

        // Lower = Пустое множество\ кроме обратной ходки
        IEnumerable<T> Lower { get; }
        // Upper = Полное множество, все возможные определения
        IEnumerable<T> Upper { get; }

        (IEnumerable<T>, IEnumerable<T>) Init { get; }
        (IEnumerable<T>, IEnumerable<T>) EnterInit { get; }
    }

    public interface ITransFunc<T>
    {
        IEnumerable<T> Transfer(BasicBlock basicBlock, IEnumerable<T> input);
    }

    public class GenericIterativeAlgorithm<T>
    {
        readonly Func<ControlFlowGraph, BasicBlock, List<BasicBlock>> getNextBlocks;
        readonly Pass type;

        public GenericIterativeAlgorithm(Pass _type)
        {
            type = _type;
            if (_type == Pass.Forward)
                getNextBlocks = GetParents;
            else if (_type == Pass.Backward)
                getNextBlocks = GetChildren;
        }

        public GenericIterativeAlgorithm()
        {
            getNextBlocks = GetParents;
        }

        public InOutData<T> Analyze(ControlFlowGraph graph, ICompareOperations<T> ops, ITransFunc<T> f) 
        {
            if (type == Pass.Backward) return AnalyzeBackward(graph, ops, f);

            var data = new InOutData<T>
            {
                [graph.GetCurrentBasicBlocks().First()] = ops.EnterInit
            };
            foreach (var node in graph.GetCurrentBasicBlocks().Skip(1))
                data[node] = ops.Init;

            var outChanged = true;
            while (outChanged)
            {
                outChanged = false;
                foreach (var block in graph.GetCurrentBasicBlocks().Skip(1))
                {
                    var inset = getNextBlocks(graph, block).Aggregate(ops.Lower, (x, y) => ops.Operator(x, data[y].Out));
                    var outset = f.Transfer(block, inset);

                    // Изменить применение Compare?
                    if (!(ops.Compare(outset, data[block].Out) && ops.Compare(data[block].Out, outset)))
                    {
                        outChanged = true;
                    }
                    data[block] = (inset, outset);
                }
            }
            return data;
        }

        // Либо надо придумать, как по-другому инвертировать 
        // множества IN OUT для алгоритмов с обратным проходом
        // не впихивая if внутрь циклов
        public InOutData<T> AnalyzeBackward(ControlFlowGraph graph, ICompareOperations<T> ops, ITransFunc<T> f){
            var data = new InOutData<T>();

            foreach (var node in graph.GetCurrentBasicBlocks())
                data[node] = ops.Init;

            var inChanged = true;
            while (inChanged){
                inChanged = false;
                foreach (var block in graph.GetCurrentBasicBlocks()){
                    var outset = getNextBlocks(graph, block)
                        .Aggregate(ops.Lower, (x, y) => ops.Operator(x, data[y].In));
                    var inset = f.Transfer(block, outset);

                    if (!ops.Compare(inset, data[block].In))
                    {
                        inChanged = true;
                    }
                    data[block] = (inset, outset);
                }
            }
            return data;
        }

        List<BasicBlock> GetParents(ControlFlowGraph graph, BasicBlock block) =>
            graph.GetParentsBasicBlocks(block).Select(z => z.Item2).ToList();

        List<BasicBlock> GetChildren(ControlFlowGraph graph, BasicBlock block) =>
            graph.GetChildrenBasicBlocks(block).Select(z => z.Item2).ToList();
    }
}
