using System.Collections.Generic;

namespace SimpleLang
{
    // Instruction(string label, string operation, string argument1, string argument2, string result)
    public static class ThreeAddressCodeRemoveAlgebraicIdentities
    {
        public static (bool wasChanged, IReadOnlyList<Instruction> instructions) RemoveAlgebraicIdentities(IReadOnlyCollection<Instruction> commands)
        {
            var result = new List<Instruction>();
            var wasChanged = false;

            foreach (var command in commands)
            {
                // a - a == 0
                var variablesAreNotBool = !bool.TryParse(command.Argument1, out _) && !bool.TryParse(command.Argument2, out _);
                if (variablesAreNotBool && command.Argument1 == command.Argument2 && command.Operation == "MINUS")
                {
                    result.Add(new Instruction(command.Label, "assign", "0", "", command.Result));
                    wasChanged = true;
                    continue;
                }

                // Умножение на 1
                var arg1IsNumber = double.TryParse(command.Argument1, out var arg1);
                if (command.Operation == "MULT" && variablesAreNotBool && arg1IsNumber && arg1 == 1)
                {
                    result.Add(new Instruction(command.Label, "assign", command.Argument2, "", command.Result));
                    wasChanged = true;
                    continue;
                }
                var arg2IsNumber = double.TryParse(command.Argument2, out var arg2);
                if (command.Operation == "MULT" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
                {
                    result.Add(new Instruction(command.Label, "assign", command.Argument1, "", command.Result));
                    wasChanged = true;
                    continue;
                }

                // Суммирование и вычитание с 0                
                if ((command.Operation == "PLUS" || command.Operation == "MINUS") && variablesAreNotBool && arg1IsNumber && arg1 == 0)
                {
                    var sign = command.Operation == "PLUS" ? "" : "-";
                    result.Add(new Instruction(command.Label, "assign", sign + command.Argument2, "", command.Result));
                    wasChanged = true;
                    continue;
                }
                if ((command.Operation == "PLUS" || command.Operation == "MINUS") && variablesAreNotBool && arg2IsNumber && arg2 == 0)
                {
                    result.Add(new Instruction(command.Label, "assign", command.Argument1, "", command.Result));
                    wasChanged = true;
                    continue;
                }

                // Умножение на 0
                if (command.Operation == "MULT" && variablesAreNotBool && (arg1IsNumber && arg1 == 0 || arg2IsNumber && arg2 == 0))
                {
                    result.Add(new Instruction(command.Label, "assign", "0", "", command.Result));
                    wasChanged = true;
                    continue;
                }

                // 0 Делить на !0
                if (command.Operation == "DIV" && variablesAreNotBool && arg1IsNumber && arg1 == 0 && (arg2IsNumber && arg2 != 0 || !arg2IsNumber))
                {
                    result.Add(new Instruction(command.Label, "assign", "0", "", command.Result));
                    wasChanged = true;
                    continue;
                }

                // Деление на 1
                if (command.Operation == "DIV" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
                {
                    result.Add(new Instruction(command.Label, "assign", command.Argument1, "", command.Result));
                    wasChanged = true;
                    continue;
                }

                // a / a = 1
                if (command.Operation == "DIV" && variablesAreNotBool && arg1 == arg2)
                {
                    result.Add(new Instruction(command.Label, "assign", "1", "", command.Result));
                    wasChanged = true;
                    continue;
                }
                result.Add(command);
            }

            return (wasChanged, result);
        }
    }
}
