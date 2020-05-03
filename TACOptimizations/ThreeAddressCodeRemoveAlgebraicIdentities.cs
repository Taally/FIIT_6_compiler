using System;
using System.Collections.Generic;

namespace SimpleLang
{
    //Instruction(string label, string operation, string argument1, string argument2, string result)
    static class ThreeAddressCodeRemoveAlgebraicIdentities
    {
        public static Tuple<bool, List<Instruction>> RemoveAlgebraicIdentities(List<Instruction> commands)
        {
            var result = new List<Instruction>();
            var changed = false;

            for (int i = 0; i < commands.Count; i++)
            {
                //a - a == 0
                bool variablesAreNotBool = !bool.TryParse(commands[i].Argument1, out bool b) && !bool.TryParse(commands[i].Argument2, out b);
                if (variablesAreNotBool && commands[i].Argument1 == commands[i].Argument2 && commands[i].Operation == "MINUS")
                {
                    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
                    changed = true;
                    continue;
                }

                //Умножение на 1
                bool arg1IsNumber = double.TryParse(commands[i].Argument1, out double arg1);
                if (commands[i].Operation == "MULT" && variablesAreNotBool && arg1IsNumber && arg1 == 1)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument2, "", commands[i].Result));
                    changed = true;
                    continue;
                }
                bool arg2IsNumber = double.TryParse(commands[i].Argument2, out double arg2);
                if (commands[i].Operation == "MULT" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
                    changed = true;
                    continue;
                }

                //Суммирование и вычитание с 0                
                if ((commands[i].Operation == "PLUS" || commands[i].Operation == "MINUS") && variablesAreNotBool && arg1IsNumber && arg1 == 0)
                {
                    var sign = commands[i].Operation == "PLUS" ? "" : "-";
                    result.Add(new Instruction(commands[i].Label, "assign", sign + commands[i].Argument2, "", commands[i].Result));
                    changed = true;
                    continue;
                }
                if ((commands[i].Operation == "PLUS" || commands[i].Operation == "MINUS") && variablesAreNotBool && arg2IsNumber && arg2 == 0)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
                    changed = true;
                    continue;
                }

                //Умножение на 0
                if (commands[i].Operation == "MULT" && variablesAreNotBool && (arg1IsNumber && arg1 == 0 || arg2IsNumber && arg2 == 0))
                {
                    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
                    changed = true;
                    continue;
                }

                //0 Делить на !0
                if (commands[i].Operation == "DIV" && variablesAreNotBool && arg1IsNumber && arg1 == 0 && (arg2IsNumber && arg2 != 0 || !arg2IsNumber))
                {
                    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
                    changed = true;
                    continue;
                }

                // Деление на 1
                if (commands[i].Operation == "DIV" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
                    changed = true;
                    continue;
                }

                //a / a = 1
                if (commands[i].Operation == "DIV" && variablesAreNotBool && arg1 == arg2)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", "1", "", commands[i].Result));
                    changed = true;
                    continue;
                }
                result.Add(commands[i]);
            }
            return new Tuple<bool, List<Instruction>>(changed, result);
        }
    }
}
