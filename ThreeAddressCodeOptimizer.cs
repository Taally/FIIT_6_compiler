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

                //RemoveGotoThroughGoto(instructions);

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

        // устранение переходов через переходы
        public static void RemoveGotoThroughGoto(List<Instruction> commands)
        {
            for (int i = 0; i < commands.Count; ++i)
            {
                if (commands[i].Operation == "ifgoto" && 4 <= (commands.Count - i))
                {
                    var com0 = commands[i];
                    var com1 = commands[i + 1];
                    var com2 = commands[i + 2];
                    var com3 = commands[i + 3];

                    // только одна операция
                    if (com1.Operation == "goto" && com2.Operation != "noop" && com3.Operation == "noop" && com0.Argument2 == com2.Label && com1.Argument1 == com3.Label)
                    {
                        string tmpName = ThreeAddressCodeTmp.GenTmpName();
                        commands[i] = new Instruction("", "EQUAL", "False", com0.Argument1, tmpName); // операция отрицания через EQUAL
                        commands[i+1] = new Instruction("", "ifgoto", tmpName, com3.Label, "");
                        commands[i+2] = new Instruction("", com2.Operation, com2.Argument1, com2.Argument2, com2.Result);
                        commands[i+3] = new Instruction(com3.Label, com3.Operation, com3.Argument1, com3.Argument2, com3.Result);

                        i += 3;
                    }
                }
            }
        }
    }
}
