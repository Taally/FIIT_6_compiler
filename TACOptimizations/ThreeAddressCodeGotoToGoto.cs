using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public static class ThreeAddressCodeGotoToGoto
    {
        private static bool wasChanged = false;

        /// <summary>
        /// Устранит переходы к переходам
        /// </summary>
        /// <param name="commands">Программа в трехадресном коде</param>
        /// <returns>
        /// Вернет программу с устраненными переходами к переходам
        /// </returns>
        public static (bool wasChanged, List<Instruction> instructions) ReplaceGotoToGoto(List<Instruction> commands)
        {
            wasChanged = false;
            var tmpCommands = new List<Instruction>();
            tmpCommands.AddRange(commands.ToArray()); // Перепишем набор наших инструкций во временный массив

            foreach (var instr in commands)
            {
                if (instr.Operation == "goto") // Простые goto (случай из задания 1)
                {
                    tmpCommands = PropagateTransitions(instr.Argument1, tmpCommands);
                }

                if (instr.Operation == "ifgoto" && instr.Label == "") // Инструкции вида if(усл) goto (случай из задания 2)
                {
                    tmpCommands = PropagateIfWithoutLabel(instr.Argument2, tmpCommands);
                }

                if (instr.Operation == "ifgoto" && instr.Label != "") // Инструкции вида l1: if(усл) goto (случай из задания 2)
                {
                    tmpCommands = PropagateIfWithLabel(instr, tmpCommands);
                }
            }

            return (wasChanged, tmpCommands);
        }

        /// <summary>
        /// Протягивает метки для goto
        /// </summary>
        /// <param name="label">Метка, которую мы ищем</param>
        /// <param name="instructions">Набор наших инструкций</param>
        /// <returns>
        /// Вернет измененные инструкции с протянутыми goto
        /// </returns>
        private static List<Instruction> PropagateTransitions(string label, List<Instruction> instructions)
        {
            for (var i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].Label == label && instructions[i].Operation == "goto" && instructions[i].Argument1 != label)
                {
                    var tmp = instructions[i].Argument1;
                    for (var j = 0; j < instructions.Count; j++)
                    {
                        if (instructions[j].Operation == "goto" && instructions[j].Argument1 == label)
                        {
                            wasChanged = true;
                            instructions[j] = new Instruction(instructions[j].Label, "goto", tmp, "", "");
                        }
                    }
                }
            }

            return instructions;
        }

        /// <summary>
        /// Протягивает метки для if(усл) goto
        /// </summary>
        /// <param name="label">Метка, которую мы ищем</param>
        /// <param name="instructions">Набор наших инструкций</param>
        /// <returns>
        /// Вернет измененные инструкции с протянутыми goto из if
        /// </returns>
        private static List<Instruction> PropagateIfWithoutLabel(string label, List<Instruction> instructions)
        {
            for (var i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].Label == label && instructions[i].Operation == "goto" && instructions[i].Argument2 != label)
                {
                    var tmp = instructions[i].Argument1;
                    for (var j = 0; j < instructions.Count; j++)
                    {
                        if (instructions[j].Operation == "ifgoto" && instructions[j].Argument2 == label)
                        {
                            wasChanged = true;
                            instructions[j] = new Instruction("", "ifgoto", instructions[j].Argument1, tmp, "");
                        }
                    }
                }
            }

            return instructions;
        }

        /// <summary>
        /// Протянуть if с метками
        /// </summary>
        /// <param name="findInstruction">Инструкция, которую мы ищем</param>
        /// <param name="instructions">Набор наших инструкций</param>
        /// <returns>
        /// Вернет измененные инструкции, если меток if не более двух
        /// </returns>
        private static List<Instruction> PropagateIfWithLabel(Instruction findInstruction, List<Instruction> instructions)
        {
            var findIndexIf = instructions.IndexOf(findInstruction);

            if (findIndexIf == -1
                || instructions.Where(x => instructions[findIndexIf].Label == x.Argument1 && x.Operation == "goto" && x.ToString() != instructions[findIndexIf].ToString()).Count() > 1)
            {
                return instructions;
            }

            var findIndexGoto = instructions.IndexOf(instructions.Where(x => instructions[findIndexIf].Label == x.Argument1 && x.Operation == "goto").ElementAt(0));

            wasChanged = true;
            if (instructions[findIndexIf + 1].Label == "")
            {
                instructions[findIndexGoto] = new Instruction("", instructions[findIndexIf].Operation, instructions[findIndexIf].Argument1, instructions[findIndexIf].Argument2, instructions[findIndexIf].Result);
                var tmp = ThreeAddressCodeTmp.GenTmpLabel();
                instructions[findIndexIf] = new Instruction(tmp, "noop", "", "", "");
                instructions.Insert(findIndexGoto + 1, new Instruction("", "goto", tmp, "", ""));
            }
            else
            {
                instructions[findIndexGoto] = new Instruction("", instructions[findIndexIf].Operation, instructions[findIndexIf].Argument1, instructions[findIndexIf].Argument2, instructions[findIndexIf].Result);
                var tmp = instructions[findIndexIf + 1].Label;
                instructions[findIndexIf] = new Instruction("", "noop", "", "", "");
                instructions.Insert(findIndexGoto + 1, new Instruction("", "goto", tmp, "", ""));
            }
            return instructions;
        }
    }
}
