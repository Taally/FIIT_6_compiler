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

        public override string ToString()
            => Argument1 + " " + Operation + " " + Argument2;

        public bool ContainsVariable(string variable)
            => Argument1 == variable || Argument2 == variable;

        public override bool Equals(object obj) =>
            obj != null
            && obj is OneExpression expr
            && Operation == expr.Operation
            && Argument1 == expr.Argument1
            && Argument2 == expr.Argument2;

        public override int GetHashCode() =>
            Operation.GetHashCode()
            ^ Argument1.GetHashCode()
            ^ Argument2.GetHashCode();
        #endregion
    }

    /// <summary>
    /// Доступные выражения. Реализует итеративный алгоритм
    /// </summary>
    public class AvailableExpressions : GenericIterativeAlgorithm<List<OneExpression>>
    {
        #region
        public override Func<List<OneExpression>, List<OneExpression>, List<OneExpression>> CollectingOperator
            => (a, b) => a.Intersect(b, new ExpressionEqualityComparer()).ToList();

        public override Func<List<OneExpression>, List<OneExpression>, bool> Compare
            => (a, b) => !a.Except(b, new ExpressionEqualityComparer()).Any() && a.Count == b.Count;

        public override List<OneExpression> InitFirst { get => new List<OneExpression>(); protected set { } }

        public override List<OneExpression> Init
        {
            get => AvailableExpressionTransferFunc.UniversalSet;
            protected set { }
        }

        public override Func<BasicBlock, List<OneExpression>, List<OneExpression>> TransferFunction { get; protected set; }

        public override Direction Direction => Direction.Forward;

        public override InOutData<List<OneExpression>> Execute(ControlFlowGraph graph, bool useRenumbering = true)
        {
            TransferFunction = new AvailableExpressionTransferFunc(graph).Transfer;
            var inOutData = base.Execute(graph);
            var outBlock = graph.GetCurrentBasicBlocks().Last();
            if (graph.GetParentsBasicBlocks(graph.VertexOf(outBlock)).Count == 0)
            {
                inOutData[outBlock] = (new List<OneExpression>(), new List<OneExpression>());
            }
            return inOutData;
        }
        #endregion
    }

    /// <summary>
    /// Класс, определяющий передаточную функцию
    /// </summary>
    public class AvailableExpressionTransferFunc
    {
        public static List<OneExpression> UniversalSet;
        private readonly Dictionary<BasicBlock, List<OneExpression>> e_gen;
        private readonly Dictionary<BasicBlock, List<OneExpression>> e_kill;
        private static readonly List<string> operationTypes = new List<string> { "OR", "AND", "LESS", "PLUS", "MINUS", "MULT", "DIV" };

        public AvailableExpressionTransferFunc(ControlFlowGraph graph)
        {
            e_gen = new Dictionary<BasicBlock, List<OneExpression>>();
            e_kill = new Dictionary<BasicBlock, List<OneExpression>>();
            Initialization(graph);
        }

        #region
        public List<OneExpression> Transfer(BasicBlock basicBlock, List<OneExpression> input)
            => E_gen_join(basicBlock, In_minus_e_kill(basicBlock, input));

        private List<OneExpression> GetU(ControlFlowGraph graph)
        {
            foreach (var block in graph.GetCurrentBasicBlocks())
            {
                foreach (var instruction in block.GetInstructions())
                {
                    if (operationTypes.Contains(instruction.Operation))
                    {
                        var newExpr = new OneExpression(instruction);
                        if (!UniversalSet.Contains(newExpr, new ExpressionEqualityComparer()))
                        {
                            UniversalSet.Add(newExpr);
                        }
                    }
                }
            }
            return UniversalSet;
        }

        private void Initialization(ControlFlowGraph graph)
        {
            UniversalSet = new List<OneExpression>();
            GetU(graph);
            Get_e_gen(graph);
            Get_e_kill(graph);
        }

        private List<OneExpression> In_minus_e_kill(BasicBlock basicBlock, List<OneExpression> input)
        {
            var result = new List<OneExpression>(input);
            foreach (var expression in e_kill[basicBlock])
            {
                if (result.Contains(expression))
                {
                    result.Remove(expression);
                }
            }
            return result;
        }

        private List<OneExpression> E_gen_join(BasicBlock basicBlock, List<OneExpression> list)
        {
            var result = new List<OneExpression>(e_gen[basicBlock]);
            foreach (var expression in list)
            {
                if (!result.Contains(expression))
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
                        if (!s.Contains(newExpr))
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
                        foreach (var expression in UniversalSet.Where(x => x.ContainsVariable(instruction.Result)).ToList())
                        {
                            if (!K.Contains(expression))
                            {
                                K.Add(expression);
                            }
                        }
                    }
                }
                foreach (var genExpression in e_gen[block])
                {
                    K.Remove(genExpression);
                }
                e_kill.Add(block, K);
            }
        }
        #endregion
    }

    internal class ExpressionEqualityComparer : IEqualityComparer<OneExpression>
    {
        public bool Equals(OneExpression expr1, OneExpression expr2)
            => expr1.Operation == expr2.Operation
            && (expr1.Argument1 == expr2.Argument1 && expr1.Argument2 == expr2.Argument2
            || expr1.Argument1 == expr2.Argument2 && expr1.Argument2 == expr2.Argument1);

        public int GetHashCode(OneExpression obj) => obj.Operation.GetHashCode() + obj.Argument1.GetHashCode() + obj.Argument2.GetHashCode();
    }
}
