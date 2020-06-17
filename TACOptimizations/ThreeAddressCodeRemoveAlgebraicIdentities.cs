using System.Collections.Generic;

namespace SimpleLang
{
    // Instruction(string label, string operation, string argument1, string argument2, string result)
    public static class ThreeAddressCodeRemoveAlgebraicIdentities
    {
        public static (bool wasChanged, List<Instruction> instructions) RemoveAlgebraicIdentities(List<Instruction> commands)
        {
            var result = new List<Instruction>();
            var wasChanged = false;

            for (var i = 0; i < commands.Count; i++)
            {
                // a - a == 0
                var variablesAreNotBool = !bool.TryParse(commands[i].Argument1, out _) && !bool.TryParse(commands[i].Argument2, out _);
                if (variablesAreNotBool && commands[i].Argument1 == commands[i].Argument2 && commands[i].Operation == "MINUS")
                {
                    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }

                // Умножение на 1
                var arg1IsNumber = double.TryParse(commands[i].Argument1, out var arg1);
                if (commands[i].Operation == "MULT" && variablesAreNotBool && arg1IsNumber && arg1 == 1)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument2, "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }
                var arg2IsNumber = double.TryParse(commands[i].Argument2, out var arg2);
                if (commands[i].Operation == "MULT" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }

                // Суммирование и вычитание с 0                
                if ((commands[i].Operation == "PLUS" || commands[i].Operation == "MINUS") && variablesAreNotBool && arg1IsNumber && arg1 == 0)
                {
                    var sign = commands[i].Operation == "PLUS" ? "" : "-";
                    result.Add(new Instruction(commands[i].Label, "assign", sign + commands[i].Argument2, "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }
                if ((commands[i].Operation == "PLUS" || commands[i].Operation == "MINUS") && variablesAreNotBool && arg2IsNumber && arg2 == 0)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }

                // Умножение на 0
                if (commands[i].Operation == "MULT" && variablesAreNotBool && (arg1IsNumber && arg1 == 0 || arg2IsNumber && arg2 == 0))
                {
                    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }

                // 0 Делить на !0
                if (commands[i].Operation == "DIV" && variablesAreNotBool && arg1IsNumber && arg1 == 0 && (arg2IsNumber && arg2 != 0 || !arg2IsNumber))
                {
                    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }

                // Деление на 1
                if (commands[i].Operation == "DIV" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }

                // a / a = 1
                if (commands[i].Operation == "DIV" && variablesAreNotBool && arg1 == arg2)
                {
                    result.Add(new Instruction(commands[i].Label, "assign", "1", "", commands[i].Result));
                    wasChanged = true;
                    continue;
                }
                result.Add(commands[i]);
            }
            return (wasChanged, result);
        }
    }
}
