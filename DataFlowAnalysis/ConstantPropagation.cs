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
            {
                return false;
            }

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

        public LatticeValue Collecting(LatticeValue second) =>
            Type == LatticeTypeData.NAC || second.Type == LatticeTypeData.NAC
            ? new LatticeValue(LatticeTypeData.NAC)
            : Type == LatticeTypeData.UNDEF
            ? second
            : second.Type == LatticeTypeData.UNDEF
            ? this
            : ConstValue == second.ConstValue
            ? second
            : new LatticeValue(LatticeTypeData.NAC);

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

    public class ConstantPropagation : GenericIterativeAlgorithm<Dictionary<string, LatticeValue>>
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
            foreach (var instruction in basicBlock.GetInstructions())
            {
                if (instruction.Result.StartsWith("#"))
                {
                    OUT.Add(instruction.Result, new LatticeValue(LatticeTypeData.UNDEF));

                    string first, second, operation;

                    first = instruction.Argument1;
                    second = instruction.Argument2;
                    operation = instruction.Operation;

                    if (first == "True" || second == "True" || first == "False" || second == "False" || untreatedTypes.Contains(operation))
                    {
                        OUT[instruction.Result] = new LatticeValue(LatticeTypeData.NAC);
                    }
                    else if (int.TryParse(first, out var v2) && OUT[second].Type == LatticeTypeData.CONST)
                    {
                        int.TryParse(OUT[second].ConstValue, out var val2);
                        OUT[instruction.Result] = new LatticeValue(LatticeTypeData.CONST, FindOperations(val2, v2, operation).ToString());
                    }
                    else if (OUT[first].Type == LatticeTypeData.CONST && int.TryParse(second, out var v1))
                    {
                        int.TryParse(OUT[first].ConstValue, out var val1);
                        OUT[instruction.Result] = new LatticeValue(LatticeTypeData.CONST, FindOperations(val1, v1, operation).ToString());
                    }
                    else if (OUT[first].Type == LatticeTypeData.CONST && OUT[second].Type == LatticeTypeData.CONST)
                    {
                        int.TryParse(OUT[first].ConstValue, out var val1);
                        int.TryParse(OUT[second].ConstValue, out var val2);
                        OUT[instruction.Result] = new LatticeValue(LatticeTypeData.CONST, FindOperations(val1, val2, operation).ToString());
                    }
                    else
                    {
                        OUT[instruction.Result] =
                            OUT[first].Type == LatticeTypeData.UNDEF
                            ? new LatticeValue(LatticeTypeData.UNDEF)
                            : OUT[first].Type == LatticeTypeData.NAC || OUT[second].Type == LatticeTypeData.NAC
                            ? new LatticeValue(LatticeTypeData.NAC)
                            : new LatticeValue(LatticeTypeData.UNDEF);
                    }
                }

                if (instruction.Operation == "assign")
                {
                    if (int.TryParse(instruction.Argument1, out var s))
                    {
                        OUT[instruction.Result] = new LatticeValue(LatticeTypeData.CONST, s);
                    }
                    else
                    {
                        var operation = instruction.Operation;
                        var first = instruction.Argument1;

                        OUT[instruction.Result] =
                            untreatedTypes.Contains(operation)
                            ? new LatticeValue(LatticeTypeData.NAC)
                            : first == "True" || first == "False"
                            ? new LatticeValue(LatticeTypeData.NAC)
                            : OUT[first].Type == LatticeTypeData.CONST
                            ? new LatticeValue(LatticeTypeData.CONST, OUT[first].ConstValue)
                            : OUT[first].Type == LatticeTypeData.NAC
                            ? new LatticeValue(LatticeTypeData.NAC)
                            : new LatticeValue(LatticeTypeData.UNDEF);
                    }
                }

                if (instruction.Operation == "input")
                {
                    OUT[instruction.Result] = new LatticeValue(LatticeTypeData.NAC);
                }
            }

            var temp_keys = OUT.Keys.Where(x => x.StartsWith("#")).ToList();
            foreach (var k in temp_keys)
            {
                OUT.Remove(k);
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
                    if (CheckStr(instr.Result) && !variables.Contains(instr.Result))
                    {
                        variables.Add(instr.Result);
                    }

                    if (CheckStr(instr.Argument1) && instr.Argument1 != "True"
                        && instr.Argument1 != "False" && !int.TryParse(instr.Argument1, out var temp1) && !variables.Contains(instr.Argument1))
                    {
                        variables.Add(instr.Argument1);
                    }

                    if (CheckStr(instr.Argument2) && instr.Argument2 != "True" && instr.Argument2 != "False"
                        && !int.TryParse(instr.Argument2, out var temp2) && !variables.Contains(instr.Argument2))
                    {
                        variables.Add(instr.Argument2);
                    }
                }
            }
            var temp = new Dictionary<string, LatticeValue>();
            foreach (var elem in variables)
            {
                temp.Add(elem, new LatticeValue(LatticeTypeData.UNDEF));
            }

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
                    var parents = g.GetParentsBasicBlocks(g.VertexOf(block)).Select(x => x.block);
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
            {
                result[elem.Key] = first[elem.Key].Collecting(elem.Value);
            }

            return result;
        }

        public override
            Func<Dictionary<string, LatticeValue>, Dictionary<string, LatticeValue>, Dictionary<string, LatticeValue>>
            CollectingOperator => (a, b) => Collect(a, b);

        public override Func<BasicBlock, Dictionary<string, LatticeValue>, Dictionary<string, LatticeValue>>
            TransferFunction
        { get; protected set; }

        public override Dictionary<string, LatticeValue> Init { get; protected set; }

        public override InOutData<Dictionary<string, LatticeValue>> Execute(ControlFlowGraph graph, bool useRenumbering = true)
        {
            var blocks = graph.GetCurrentBasicBlocks();
            var variables = new HashSet<string>();
            foreach (var block in blocks)
            {
                foreach (var instr in block.GetInstructions())
                {
                    if (CheckStr(instr.Result) && !variables.Contains(instr.Result))
                    {
                        variables.Add(instr.Result);
                    }

                    if (CheckStr(instr.Argument1) && instr.Argument1 != "True"
                        && instr.Argument1 != "False" && !int.TryParse(instr.Argument1, out var temp1) && !variables.Contains(instr.Argument1))
                    {
                        variables.Add(instr.Argument1);
                    }

                    if (CheckStr(instr.Argument2) && instr.Argument2 != "True" && instr.Argument2 != "False"
                        && !int.TryParse(instr.Argument2, out var temp2) && !variables.Contains(instr.Argument2))
                    {
                        variables.Add(instr.Argument2);
                    }
                }
            }
            var temp = new Dictionary<string, LatticeValue>();
            foreach (var elem in variables)
            {
                temp.Add(elem, new LatticeValue(LatticeTypeData.UNDEF));
            }

            Init = temp;
            TransferFunction = Transfer;
            return base.Execute(graph);
        }

        private bool CheckStr(string str) => str != "" && !str.StartsWith("#") && !str.StartsWith("L");
    }
}
