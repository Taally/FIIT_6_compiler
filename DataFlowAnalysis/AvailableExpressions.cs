using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{
    public class AvailableExpressionsTransferFunc : ITransFunc<List<Instruction>>
    {
        Dictionary<BasicBlock, List<Instruction>> e_gen;
        Dictionary<BasicBlock, List<Instruction>> e_kill;

        public AvailableExpressionsTransferFunc(ControlFlowGraph graph)
        {
            var availableExpr = new AvailableExpressions(graph);
            this.e_gen = GetInstrFromSet(availableExpr.e_gen);
            this.e_kill = GetInstrFromSet(availableExpr.e_kill);
        }

        //e_gen U (in - e_kill)

        public List<Instruction> Transfer(BasicBlock basicBlock, List<Instruction> input)
            => e_gen[basicBlock]
                .Union(input.Except(e_kill[basicBlock]))
                .ToList();

        private Dictionary<BasicBlock, List<Instruction>> GetInstrFromSet
            (Dictionary<BasicBlock, List<Expression>> setOfExpressions)
        {
            var setOfInstructions = new Dictionary<BasicBlock, List<Instruction>>();
            foreach(var expression in setOfExpressions)
            {
                var listOfInstructions = new List<Instruction>();
                foreach(var expr in expression.Value)
                    listOfInstructions.Add(expr.GetInstruction());
                setOfInstructions[expression.Key] = new List<Instruction>(listOfInstructions);
            }
            return setOfInstructions;
        }
    }

    public class Expression
    {
        private readonly Instruction instruction;
        public string Operation { get; }
        public string Argument1 { get; }
        public string Argument2 { get; }

        public Expression(Instruction instruction)
        {
            Operation = instruction.Operation;
            Argument1 = instruction.Argument1;
            Argument2 = instruction.Argument2;
            this.instruction = instruction;
        }
        public Instruction GetInstruction()
        {
            return instruction;
        }
        public bool Equals(Expression expr2)
        {
            return Operation == expr2.Operation && Argument1 == expr2.Argument1 && Argument2 == expr2.Argument2;
        }

        public bool ContainsVariable(string variable)
        {
            return Argument1 == variable || Argument2 == variable;
        }
        public override string ToString()
        {
            return instruction.ToString();
        }
    }

    public class AvailableExpressions 
    {
        static List<Expression> u;
        public Dictionary<BasicBlock, List<Expression>> e_gen;
        public Dictionary<BasicBlock, List<Expression>> e_kill;
        public Dictionary<BasicBlock, List<Expression>> dictionaryIn;
        public Dictionary<BasicBlock, List<Expression>> dictionaryOut;

        List<string> operationTypes = new List<string> { "OR", "AND", "LESS", "PLUS", "MINUS", "MULT", "DIV" };
        public AvailableExpressions(ControlFlowGraph graph)
        {
            u = new List<Expression>();
            GetU(graph);
            e_gen = new Dictionary<BasicBlock, List<Expression>>();
            Get_e_gen(graph);
            e_kill = new Dictionary<BasicBlock, List<Expression>>();
            Get_e_kill(graph);
            dictionaryIn = new Dictionary<BasicBlock, List<Expression>>();
            dictionaryOut = new Dictionary<BasicBlock, List<Expression>>();
            AssignInitialValues(graph);
            RunIterrations(graph);
        }

        private void RunIterrations(ControlFlowGraph graph)
        {
            var blocks = graph.GetCurrentBasicBlocks();
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var block in blocks)
                {
                    var parents = GetParents(graph, block);
                    dictionaryIn[block] = GetIntersection(parents);
                    dictionaryOut[block] = E_gen_join(block, In_minus_e_kill(block));
                }
            }
        }

        private void GetU(ControlFlowGraph graph)
        {            
            var blocks = graph.GetCurrentBasicBlocks();
            foreach(var block in blocks)
            {
                var instructions = block.GetInstructions();
                foreach (var instruction in instructions)
                {
                    if (operationTypes.Contains(instruction.Operation))
                    {
                        var newExpr = new Expression(instruction);
                        if (!ContainsExpression(u, newExpr))
                            u.Add(newExpr);
                    }
                }
            }
        }

        private void Get_e_gen(ControlFlowGraph graph)
        {            
            var blocks = graph.GetCurrentBasicBlocks();
            foreach (var block in blocks)
            {
                var s = new List<Expression>();
                var instructions = block.GetInstructions();
                foreach (var instruction in instructions)
                {
                    if (operationTypes.Contains(instruction.Operation))
                    {
                        var newExpr = new Expression(instruction);                        
                        if (!ContainsExpression(s, newExpr))
                            s.Add(newExpr);
                        RemoveExpressions(s, instruction.Result);
                    }
                    if (instruction.Operation == "assign")
                        RemoveExpressions(s, instruction.Result);
                }
                e_gen.Add(block, s);                
            }
        }

        private void Get_e_kill(ControlFlowGraph graph)
        {
            var blocks = graph.GetCurrentBasicBlocks();
            foreach (var block in blocks)
            {
                var K = new List<Expression>();
                var instructions = block.GetInstructions();
                foreach(var instruction in instructions)
                {                    
                    if (operationTypes.Contains(instruction.Operation) || instruction.Operation == "assign")
                    {
                        var killed = GetListOfKilledExpressions(instruction.Result);
                        foreach (var expression in killed)
                            if (!ContainsExpression(K, expression))
                                K.Add(expression);
                    }
                }
                foreach (var genExpression in e_gen[block])
                    RemoveExpression(K, genExpression);
                e_kill.Add(block, K);
            }            
        }

        private List<Expression> GetListOfKilledExpressions(string variable)
        {
            var result = new List<Expression>();
            foreach (var expr in u)
                if (expr.ContainsVariable(variable))
                    result.Add(expr);
            return result;
        }

        private bool RemoveExpression(List<Expression> K, Expression expression)
        {
            for (int i = 0; i < K.Count; i++)
                if (K[i].Equals(expression))
                {
                    K.RemoveAt(i);
                    return true;
                }
            return false;
        }
        private void RemoveExpressions(List<Expression> list, string variable)
        {
            for (int i = 0; i < list.Count(); i++)
                if (list[i].ContainsVariable(variable))
                    list.RemoveAt(i);
        }


        private bool ContainsExpression(List<Expression> list, Expression expr)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Equals(expr))
                    return true;
            return false;
        }

        
        private void AssignInitialValues(ControlFlowGraph graph)
        {
            var blocks = graph.GetCurrentBasicBlocks();
            foreach (var block in blocks)
            {
                dictionaryOut[block] = u;
                dictionaryIn[block] = null;
            }
        }

        private List<Expression> GetIntersection(List<BasicBlock> blocks)
        {
            var listOfExpressions = new List<Expression>();
            var result = new List<Expression>();
            foreach (var block in blocks)
            {
                var expressions = dictionaryOut[block];
                foreach (var expression in expressions)
                {
                    if (!ContainsExpression(listOfExpressions, expression))
                        listOfExpressions.Add(expression);
                }                      
            }    
            foreach (var expression in listOfExpressions)
            {
                var contains = true;
                foreach(var block in blocks)
                    if(!ContainsExpression(dictionaryOut[block], expression))
                    {
                        contains = false; 
                        break;
                    }
                if (contains)
                    result.Add(expression);
            }
            return result;
        }

        private List<Expression> In_minus_e_kill(BasicBlock block)
        {
            var result = new List<Expression>(dictionaryIn[block]);            
            foreach (var expression in e_kill[block])
                if (ContainsExpression(result, expression))
                    RemoveExpression(result, expression);
            return result;
        }

        private List<Expression> E_gen_join(BasicBlock block, List<Expression> list)
        {
            var result = new List<Expression>(e_gen[block]);
            foreach (var expression in list)
                if (!ContainsExpression(result, expression))
                    result.Add(expression);
            return result;
        }

        private List<BasicBlock> GetParents(ControlFlowGraph graph, BasicBlock block)
        {
            var blocksWithID = graph.GetParentsBasicBlocks(block);
            var result = new List<BasicBlock>();
            foreach (var blockWithID in blocksWithID)
                result.Add(blockWithID.Item2);
            return result;
        }
        
        public class Operation : ICompareOperations<List<Instruction>>
        {
            public Operation(List<Instruction> instructions) => Upper = new List<Instruction>(instructions);
            public List<Instruction> Lower => new List<Instruction>();
            public List<Instruction> Upper { get; }
            public bool Compare(List<Instruction> a, List<Instruction> b) => a.SequenceEqual(b);
            public (List<Instruction>, List<Instruction>) Init() => (Lower, Lower);
            public List<Instruction> Operator(List<Instruction> a, List<Instruction> b) => a.Union(b).ToList();
        }
    }
}
