using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    /// <summary>
    /// Вспомогательный класс для выражения
    /// </summary>
    public class OneExpression
    {
        #region
        public string Operation { get; }
        public string Argument1 { get; }
        public string Argument2 { get; }

        public OneExpression(string operation, string argument1, string argument2)
        {
            Operation = operation;
            Argument1 = argument1;
            Argument2 = argument2;
        }

        public OneExpression(Instruction instruction)
        {
            Operation = instruction.Operation;
            Argument1 = instruction.Argument1;
            Argument2 = instruction.Argument2;
        }

        public bool Equals(OneExpression expr1, OneExpression expr2)
            => expr1.Operation == expr2.Operation
                    && (expr1.Argument1 == expr2.Argument1 && expr1.Argument2 == expr2.Argument2
                    || expr1.Argument1 == expr2.Argument2 && expr1.Argument2 == expr2.Argument1);

        public bool Equals(OneExpression expr2)
            => Equals(this, expr2);

        public override string ToString()
            => Argument1 + " " + Operation + " " + Argument2;

        public bool ContainsVariable(string variable)
            => Argument1 == variable || Argument2 == variable;

        public int GetHashCode(OneExpression obj) 
            => throw new NotImplementedException();
        #endregion
    }
    /// <summary>
    /// Доступные выражения. Реализует итеративный алгоритм
    /// </summary>
    public class AvailableExpressions : GenericIterativeAlgorithm<List<OneExpression>>
    {
        #region
        private ControlFlowGraph CFG;
        public override Func<List<OneExpression>, List<OneExpression>, List<OneExpression>> CollectingOperator
            => (a, b) => a.Intersection(b);

        public override Func<List<OneExpression>, List<OneExpression>, bool> Compare
            => (a, b) => a.IsEqual(b);
        public override List<OneExpression> InitFirst { get => new List<OneExpression>(); protected set { } }
        public override List<OneExpression> Init { get => AvailableExpressionTransferFunc.GetU(CFG); protected set { } }
        public override Func<BasicBlock, List<OneExpression>, List<OneExpression>> TransferFunction { get; protected set; }
        public override Direction Direction => Direction.Forward; 
        public override InOutData<List<OneExpression>> Execute(ControlFlowGraph graph)
        {
            CFG = graph;
            TransferFunction = new AvailableExpressionTransferFunc(graph).Transfer;
            GetInitData(graph, out var blocks, out var data,
                out var getPreviousBlocks, out var getDataValue, out var combine);

            var outChanged = true;
            while (outChanged)
            {
                outChanged = false;
                foreach (var block in blocks)
                {
                    var sets = getPreviousBlocks(block).Select(x => getDataValue(x));
                    var inset = new List<OneExpression>();
                    var count = sets.Count();
                    if (count == 1)
                    {
                        inset = sets.First();
                    }
                    if (count >= 2)
                    {
                        inset = getPreviousBlocks(block).Aggregate(Init, (x, y) => CollectingOperator(x, getDataValue(y)));
                    }
                    var outset = TransferFunction(block, inset);
                    if (!Compare(outset, getDataValue(block)))
                    {
                        outChanged = true;
                    }
                    data[block] = combine(inset, outset);
                }
            }
            return data;
        }
        #endregion
    }
    /// <summary>
    /// Класс, определяющий передаточную функцию
    /// </summary>
    public class AvailableExpressionTransferFunc
    {
        private static List<OneExpression> u;
        private readonly Dictionary<BasicBlock, List<OneExpression>> e_gen;
        private readonly Dictionary<BasicBlock, List<OneExpression>> e_kill;
        private static readonly List<string> operationTypes = new List<string> { "OR", "AND", "LESS", "PLUS", "MINUS", "MULT", "DIV" };
        public AvailableExpressionTransferFunc(ControlFlowGraph graph)
        {
            u = new List<OneExpression>();
            e_gen = new Dictionary<BasicBlock, List<OneExpression>>();
            e_kill = new Dictionary<BasicBlock, List<OneExpression>>();
            Initialization(graph);
        }
        #region
        public List<OneExpression> Transfer(BasicBlock basicBlock, List<OneExpression> input) 
            => E_gen_join(basicBlock, In_minus_e_kill(basicBlock, input));
        public static List<OneExpression> GetU(ControlFlowGraph graph)
        {
            var blocks = graph.GetCurrentBasicBlocks();
            foreach (var block in blocks)
            {
                var instructions = block.GetInstructions();
                foreach (var instruction in instructions)
                {
                    if (operationTypes.Contains(instruction.Operation))
                    {
                        var newExpr = new OneExpression(instruction);
                        if (!u.ContainsExpression(newExpr))
                        {
                            u.Add(newExpr);
                        }
                    }
                }
            }
            return u;
        }
        private void Initialization(ControlFlowGraph graph)
        {
            GetU(graph);
            Get_e_gen(graph);
            Get_e_kill(graph);
        }
        private List<OneExpression> In_minus_e_kill(BasicBlock basicBlock, List<OneExpression> input)
        {
            var result = new List<OneExpression>(input);
            foreach (var expression in e_kill[basicBlock])
            {
                if (result.ContainsExpression(expression))
                {
                    RemoveExpression(result, expression);
                }
            }
            return result;
        }
        private List<OneExpression> E_gen_join(BasicBlock basicBlock, List<OneExpression> list)
        {
            var result = new List<OneExpression>(e_gen[basicBlock]);
            foreach (var expression in list)
            {
                if (!result.ContainsExpression(expression))
                {
                    result.Add(expression);
                }
            }
            return result;
        }
        private void Get_e_gen(ControlFlowGraph graph)
        {
            foreach (var block in graph.GetCurrentBasicBlocks())
            {
                var s = new List<OneExpression>();
                foreach (var instruction in block.GetInstructions())
                {
                    if (operationTypes.Contains(instruction.Operation))
                    {
                        var newExpr = new OneExpression(instruction);
                        if (!s.ContainsExpression(newExpr))
                        {
                            s.Add(newExpr);
                        }
                        s.Where(expression => expression.ContainsVariable(instruction.Result));
                    }
                    if (instruction.Operation == "assign")
                    {
                        s.Where(expression => expression.ContainsVariable(instruction.Result));
                    }
                }
                e_gen.Add(block, s);
            }
        }
        private void Get_e_kill(ControlFlowGraph graph)
        {
            foreach (var block in graph.GetCurrentBasicBlocks())
            {
                var K = new List<OneExpression>();
                foreach (var instruction in block.GetInstructions())
                {
                    if (operationTypes.Contains(instruction.Operation) || instruction.Operation == "assign")
                    {
                        foreach (var expression in u.Where(x => x.ContainsVariable(instruction.Result)).ToList())
                        {
                            if (!K.ContainsExpression(expression))
                            {
                                K.Add(expression);
                            }
                        }
                    }
                }
                foreach (var genExpression in e_gen[block])
                {
                    RemoveExpression(K, genExpression);
                }
                e_kill.Add(block, K);
            }
        }
        private void RemoveExpression(List<OneExpression> K, OneExpression expression)
        {
            for (var i = 0; i < K.Count; i++)
            {
                if (K[i].Equals(expression))
                {
                    K.RemoveAt(i);
                }
            }
        }
        #endregion
    }
    public static class ListExtensionForAvailableExpression
    {
        #region
        public static List<OneExpression> Intersection(this List<OneExpression> list1, List<OneExpression> list2)
        {
            var result = new List<OneExpression>();
            foreach (var oneExpression in list1)
            {
                if (list2.ContainsExpression(oneExpression))
                {
                    result.Add(oneExpression);
                }
            }
            return result;
        }
        public static bool IsEqual(this List<OneExpression> list1, List<OneExpression> list2)
        {
            if (list1 == null || list2 == null)
            {
                return false;
            }
            foreach (var expression in list1)
            {
                if (!list2.ContainsExpression(expression))
                {
                    return false;
                }
            }
            return list1.Count == list2.Count;
        }

        public static bool ContainsExpression(this List<OneExpression> list, OneExpression expr)
        {
            if (list == null)
            {
                return false;
            }
            foreach (var elem in list)
            {
                if (elem.Equals(expr))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
