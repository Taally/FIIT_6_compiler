using SimpleLang.Visitors;
using System;
using System.Collections.Generic;

namespace SimpleLang.ThreeAddressCodeOptimizations
{
    static class ConstantFolding
    {
        public static List<Instruction> FoldConstants(List<Instruction> instructions)
        {
            var result = new List<Instruction>();
            foreach (var instruction in instructions)
            {
                if (instruction.Argument2 != "")
                    if (int.TryParse(instruction.Argument1, out var intArg1) && int.TryParse(instruction.Argument2, out var intArg2))
                    {
                        var constant = CalculateConstant(instruction.Operation, intArg1, intArg2);
                        result.Add(new Instruction(instruction.Label, "assign", constant, "", instruction.Result));
                        continue;
                    }
                    else if (bool.TryParse(instruction.Argument1, out var boolArg1) && bool.TryParse(instruction.Argument2, out var boolArg2))
                    {
                        var constant = CalculateConstant(instruction.Operation, boolArg1, boolArg2);
                        result.Add(new Instruction(instruction.Label, "assign", constant, "", instruction.Result));
                        continue;
                    }
                result.Add(instruction);
            }
            return result;
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
    }
}
