using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    static class ThreeAddressCodeOptimizer
    {
        static public bool Changed { get; set; }

        static public void Optimize(List<Instruction> instructions)
        {
            while (true)
            {
                FoldConstants(instructions);
                if (Changed)
                    continue;

                DeleteDeadCodeWithDeadVars(instructions);
                if (Changed)
                    continue;

                break;
            }
        }

        static public void FoldConstants(List<Instruction> instructions)
        {
            Changed = false;
            for (int i = 0; i < instructions.Count; ++i)
            {
                if (instructions[i].Argument2 != "")
                    if (int.TryParse(instructions[i].Argument1, out var intArg1) && int.TryParse(instructions[i].Argument2, out var intArg2))
                    {
                        var constant = CalculateConstant(instructions[i].Operation, intArg1, intArg2);
                        instructions[i] = new Instruction(instructions[i].Label, "assign", constant, "", instructions[i].Result);
                        Changed = true;
                        continue;
                    }
                    else if (bool.TryParse(instructions[i].Argument1, out var boolArg1) && bool.TryParse(instructions[i].Argument2, out var boolArg2))
                    {
                        var constant = CalculateConstant(instructions[i].Operation, boolArg1, boolArg2);
                        instructions[i] = new Instruction(instructions[i].Label, "assign", constant, "", instructions[i].Result);
                        Changed = true;
                        continue;
                    }
            }
        }

        private static string CalculateConstant(string operation, bool boolArg1, bool boolArg2)
        {
            switch (operation)
            {
                case "OR":
                    return (boolArg1 || boolArg2).ToString();
                case "AND":
                    return (boolArg1 && boolArg2).ToString();
                case "EQUAL":
                    return (boolArg1 == boolArg2).ToString();
                case "NOTEQUAL":
                    return (boolArg1 != boolArg2).ToString();
                default:
                    throw new InvalidOperationException();
            }
        }

        private static string CalculateConstant(string operation, int intArg1, int intArg2)
        {
            switch (operation)
            {
                case "EQUAL":
                    return (intArg1 == intArg2).ToString();
                case "NOTEQUAL":
                    return (intArg1 != intArg2).ToString();
                case "LESS":
                    return (intArg1 < intArg2).ToString();
                case "GREATER":
                    return (intArg1 > intArg2).ToString();
                case "EQGREATER":
                    return (intArg1 >= intArg2).ToString();
                case "EQLESS":
                    return (intArg1 <= intArg2).ToString();
                case "PLUS":
                    return (intArg1 + intArg2).ToString();
                case "MINUS":
                    return (intArg1 - intArg2).ToString();
                case "MULT":
                    return (intArg1 * intArg2).ToString();
                case "DIV":
                    return (intArg1 / intArg2).ToString();
                default:
                    throw new InvalidOperationException();
            }
        }

        static public void DeleteDeadCodeWithDeadVars(List<Instruction> instructions)
        {
            Changed = false;
            Dictionary<string, bool> varStatus = new Dictionary<string, bool>();

            var last = instructions.Last();
            varStatus.Add(last.Result, false);
            if (!int.TryParse(last.Argument1, out _) && last.Argument1 != "True" && last.Argument1 != "False")
                varStatus[last.Argument1] = true;
            if (!int.TryParse(last.Argument2, out _) && last.Argument2 != "True" && last.Argument2 != "False")
                varStatus[last.Argument2] = true;

            for (int i = instructions.Count - 2; i >= 0; --i)
            {
                var instruction = instructions[i];
                if (instruction.Operation == "noop")
                    continue;
                if (varStatus.ContainsKey(instruction.Result) && !varStatus[instruction.Result])
                {
                    instructions[i] = new Instruction(instruction.Label, "noop", null, null, null);
                    Changed = true;
                    continue;
                }

                varStatus[instruction.Result] = false;
                if (!int.TryParse(instruction.Argument1, out _) && instruction.Argument1 != "True" && instruction.Argument1 != "False")
                    varStatus[instruction.Argument1] = true;
                if (!int.TryParse(instruction.Argument2, out _) && instruction.Argument2 != "True" && instruction.Argument2 != "False")
                    varStatus[instruction.Argument2] = true;
            }
        }
    }
}
