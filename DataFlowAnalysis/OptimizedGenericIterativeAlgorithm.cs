using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{

    public class InOutData<T> : Dictionary<BasicBlock, (T In, T Out)>
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

    // Upper = Полное множество, все возможные определения 
    // Lower = Пустое множество\ кроме обратной ходки

    public interface ICompareOperations<T>
    {
        // пересечение или объединение 
        T Operator(T a, T b);

        bool Compare(T a, T b);

        T Lower { get; }
        T Upper { get; }

        (T, T) Init();
    }

    public interface ITransFunc<T>
    {
        T Transfer(BasicBlock basicBlock, T input);
    }

    public class OptimizedGenericIterativeAlgorithm<T>
    {

        public InOutData<T> Analyze(ControlFlowGraph graph, ICompareOperations<T> ops, ITransFunc<T> f) 
        {
            var data = new InOutData<T>();
            data[graph.GetCurrentBasicBlocks().ElementAt(0)] = ops.Init();          
            

            foreach (var node in graph.GetCurrentBasicBlocks())
                data[node] = ops.Init();

            var outChanged = true;
            while (outChanged)
            {
                outChanged = false;
                foreach (var block in graph.GetCurrentBasicBlocks())
                {
                    var inset = ParentsBlock(graph, block).Aggregate(ops.Lower, (x, y) => ops.Operator(x, data[y].Out));
                    var outset = f.Transfer(block, inset);

                    if (!(ops.Compare(outset, data[block].Out) && ops.Compare(data[block].Out, outset)))
                    {
                        outChanged = true;
                    }
                    data[block] = (inset, outset);
               
                }
            }

            return data;
        }

        public List<BasicBlock> ParentsBlock(ControlFlowGraph graph, BasicBlock block)
        {
            List<BasicBlock> tmp = new List<BasicBlock>();
            foreach (var xx in ControlFlowGraphExtensions.GetParentsBasicBlocks(graph, block))
            {
                tmp.Add(xx.Item2);
            }

            return tmp;
        }
    }
}

