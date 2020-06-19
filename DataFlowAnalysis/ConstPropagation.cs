using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public enum LatticeTypeData { UNDEF = 0, CONST = 1, NAC = 2 }

    public struct LatticeValue
    {
        public LatticeTypeData Type { get; private set; }
        public string ConstValue { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var temp = (LatticeValue)obj;
            return Type == temp.Type && ConstValue == temp.ConstValue;
        }

        public static bool operator ==(LatticeValue first, LatticeValue second) =>
            first.Equals(second);

        public static bool operator !=(LatticeValue first, LatticeValue second) =>
            !first.Equals(second);

        public LatticeValue(LatticeTypeData type, string str = null)
        {
            Type = type;
            ConstValue = str;
        }

        public LatticeValue(LatticeTypeData type, int val)
        {
            Type = type;
            ConstValue = val.ToString();
        }

        public override int GetHashCode()
            => (Type.ToString() + ConstValue).GetHashCode();

        public LatticeValue collecting(LatticeValue second)
        {
            if (Type == LatticeTypeData.NAC || second.Type == LatticeTypeData.NAC)
                return new LatticeValue(LatticeTypeData.NAC);
            if (Type == LatticeTypeData.UNDEF)
                return second;
            if (second.Type == LatticeTypeData.UNDEF)
                return this;
            if (ConstValue == second.ConstValue)
                return second;
            return new LatticeValue(LatticeTypeData.NAC);
        }

        public override string ToString()
            => Type.ToString() + " " + ConstValue;
    }

    public class INsOUTs
    {
        public Dictionary<BasicBlock, Dictionary<string, LatticeValue>> IN { get; set; }
        public Dictionary<BasicBlock, Dictionary<string, LatticeValue>> OUT { get; set; }

        public INsOUTs()
        {
            IN = new Dictionary<BasicBlock, Dictionary<string, LatticeValue>>();
            OUT = new Dictionary<BasicBlock, Dictionary<string, LatticeValue>>();
        }
    }

    public class ConstPropagation : GenericIterativeAlgorithm<Dictionary<string, LatticeValue>>
    {
        public int FindOperations(int v1, int v2, string op)
        {
            switch (op)
            {
                case "PLUS":
                    return v1 + v2;
                case "MULT":
                    return v1 * v2;
                case "DIV":
                    return v1 / v2;
                case "MINUS":
                    return v1 - v2;
            }
            return 0;
        }

        public HashSet<string> untreatedTypes = new HashSet<string>() {
            "OR",
            "AND",
            "EQUAL",
            "NOTEQUAL",
            "GREATER",
            "LESS",
            "EQGREATER",
            "EQLESS",
            "NOT",
            "UNMINUS"
        };

        public Dictionary<string, LatticeValue> Transfer(BasicBlock basicBlock, Dictionary<string, LatticeValue> IN)
        {
            var OUT = IN.ToDictionary(entry => entry.Key, entry => entry.Value);
            var instrs = basicBlock.GetInstructions();
            for (var i = 0; i < instrs.Count; i++)
            {
                if (instrs[i].Operation == "assign")
                {
                    if (int.TryParse(instrs[i].Argument1, out int s))
                    {
                        OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.CONST, s);
                    }
                    else
                    {
                        string first, second, operation;

                        if (instrs[i].Argument1.StartsWith("#"))
                        {
                            first = instrs[i - 1].Argument1;
                            second = instrs[i - 1].Argument2;
                            operation = instrs[i - 1].Operation;
                        }
                        else
                        {
                            first = instrs[i].Argument1;
                            second = instrs[i].Argument2;
                            operation = instrs[i].Operation;
                        }
                        
                        if (first != "" && second != "")
                        {
                            if (first == "True" || second == "True" || second == "False" || second == "False" || untreatedTypes.Contains(operation))
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.NAC);
                            else
                            if (instrs[i - 1].Argument1.StartsWith("#") || instrs[i - 1].Argument2.StartsWith("#"))
                            {
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.NAC);
                            }
                            else
                            if (Int32.TryParse(first, out int v2) && OUT[second].Type == LatticeTypeData.CONST)
                            {
                                Int32.TryParse(OUT[second].ConstValue, out int val2);
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.CONST, FindOperations(val2, v2, operation).ToString());
                            }
                            else
                            if (OUT[first].Type == LatticeTypeData.CONST && Int32.TryParse(second, out int v1))
                            {
                                Int32.TryParse(OUT[first].ConstValue, out int val1);
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.CONST, FindOperations(val1, v1, operation).ToString());
                            }
                            else
                            if (OUT[first].Type == LatticeTypeData.CONST && OUT[second].Type == LatticeTypeData.CONST)
                            {
                                Int32.TryParse(OUT[first].ConstValue, out int val1);
                                Int32.TryParse(OUT[second].ConstValue, out int val2);                                                            
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.CONST, FindOperations(val1, val2, operation).ToString());
                            }
                            else if (OUT[first].Type == LatticeTypeData.UNDEF)
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.UNDEF);
                            else if (OUT[first].Type == LatticeTypeData.NAC || OUT[second].Type == LatticeTypeData.NAC)
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.NAC);
                            else
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.UNDEF);
                        }
                        else if (first != "")
                        {
                            if (untreatedTypes.Contains(operation))
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.NAC);
                            else
                            if (first == "True" || first == "False")
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.NAC);
                            else
                            if (OUT[first].Type == LatticeTypeData.CONST)
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.CONST, OUT[first].ConstValue);
                            else if (OUT[first].Type == LatticeTypeData.NAC)
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.NAC);
                            else
                                OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.UNDEF);
                        }
                    }
                }

                if (instrs[i].Operation == "input")
                {
                    OUT[instrs[i].Result] = new LatticeValue(LatticeTypeData.NAC);
                }
            }
            return OUT;
        }

        public INsOUTs ExecuteNonGeneric(ControlFlowGraph g)
        {
            var blocks = g.GetCurrentBasicBlocks();
            var INs = new Dictionary<BasicBlock, Dictionary<string, LatticeValue>>();
            var OUTs = new Dictionary<BasicBlock, Dictionary<string, LatticeValue>>();

            var variables = new HashSet<string>();

            foreach (var block in blocks)
            {
                foreach (var instr in block.GetInstructions())
                {
                    if (instr.Result != "" && !instr.Result.StartsWith("#") && !instr.Result.StartsWith("L") && !variables.Contains(instr.Result))
                        variables.Add(instr.Result);
                    if (instr.Argument1 != "" && !instr.Argument1.StartsWith("#") && !instr.Argument1.StartsWith("L") && instr.Argument1 != "True"
                        && instr.Argument1 != "False" && !Int32.TryParse(instr.Argument1, out int temp1) && !variables.Contains(instr.Argument1))
                        variables.Add(instr.Argument1);
                    if (instr.Argument2 != "" && !instr.Argument2.StartsWith("#") && !instr.Argument2.StartsWith("L") && instr.Argument2 != "True" && instr.Argument2 != "False"
                        && !Int32.TryParse(instr.Argument2, out int temp2) && !variables.Contains(instr.Argument2))
                        variables.Add(instr.Argument2);
                }
            }
            var temp = new Dictionary<string, LatticeValue>();
            foreach (var elem in variables)
                temp.Add(elem, new LatticeValue(LatticeTypeData.UNDEF));

            foreach (var block in blocks)
            {
                INs.Add(block, temp.ToDictionary(entry => entry.Key, entry => entry.Value));
                OUTs.Add(block, temp.ToDictionary(entry => entry.Key, entry => entry.Value));
            }

            var Changed = true;
            while (Changed)
            {
                Changed = false;
                foreach (var block in blocks)
                {
                    var parents = g.GetParentsBasicBlocks(g.VertexOf(block)).Select(x => x.Item2);
                    INs[block] = parents.Select(x => OUTs[x])
                        .Aggregate(temp.ToDictionary(entry => entry.Key, entry => entry.Value), (x, y) => Collect(x, y));
                    var newOut = Transfer(block, INs[block]);
                    if (OUTs[block].Where(entry => newOut[entry.Key] != entry.Value).Any())
                    {
                        Changed = true;
                        OUTs[block] = newOut;
                    }
                }
            }

            return new INsOUTs { IN = INs, OUT = OUTs };
        }

        public override Direction Direction => Direction.Forward;

        public override Func<Dictionary<string, LatticeValue>, Dictionary<string, LatticeValue>, bool> Compare
            => (a, b) => !a.Where(entry => b[entry.Key] != entry.Value).Any();

        public static Dictionary<string, LatticeValue> Collect(Dictionary<string, LatticeValue> first, Dictionary<string, LatticeValue> second)
        {
            var result = new Dictionary<string, LatticeValue>(first.Count, first.Comparer);
            foreach (var elem in second)
                result[elem.Key] = first[elem.Key].collecting(elem.Value);
            return result;
        }

        public override
            Func<Dictionary<string, LatticeValue>, Dictionary<string, LatticeValue>, Dictionary<string, LatticeValue>>
            CollectingOperator => (a, b) => Collect(a, b);

        public override Func<BasicBlock, Dictionary<string, LatticeValue>, Dictionary<string, LatticeValue>>
            TransferFunction { get; protected set; }

        public override Dictionary<string, LatticeValue> Init { get; protected set; }

        public override InOutData<Dictionary<string, LatticeValue>> Execute(ControlFlowGraph graph)
        {
            var blocks = graph.GetCurrentBasicBlocks();
            var variables = new HashSet<string>();
            foreach (var block in blocks)
            {
                foreach (var instr in block.GetInstructions())
                {
                    if (instr.Result != "" && !instr.Result.StartsWith("#") && !instr.Result.StartsWith("L") && !variables.Contains(instr.Result))
                        variables.Add(instr.Result);
                    if (instr.Argument1 != "" && !instr.Argument1.StartsWith("#") && !instr.Argument1.StartsWith("L") && instr.Argument1 != "True" 
                        && instr.Argument1 != "False" && !Int32.TryParse(instr.Argument1, out int temp1) && !variables.Contains(instr.Argument1))
                        variables.Add(instr.Argument1);
                    if (instr.Argument2 != "" && !instr.Argument2.StartsWith("#") && !instr.Argument2.StartsWith("L") && instr.Argument2 != "True" && instr.Argument2 != "False"
                        && !Int32.TryParse(instr.Argument2, out int temp2) && !variables.Contains(instr.Argument2))
                        variables.Add(instr.Argument2);
                }
            }
            var temp = new Dictionary<string, LatticeValue>();
            foreach (var elem in variables)
                temp.Add(elem, new LatticeValue(LatticeTypeData.UNDEF));
            Init = temp;
            TransferFunction = Transfer;
            return base.Execute(graph);
        }
    }

}
