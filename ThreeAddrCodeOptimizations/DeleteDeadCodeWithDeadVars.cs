using SimpleLang.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang.ThreeAddressCodeOptimizations
{
    static class DeleteDeadCodeWithDeadVars
    {
        public static List<Instruction> Execute(List<Instruction> commands)
        {
            List<Instruction> result = new List<Instruction>();
            Dictionary<string, bool> varStatus = new Dictionary<string, bool>();

            var last = commands.Last();
            result.Add(last);
            varStatus.Add(last.Result, false);
            if (!int.TryParse(last.Argument1, out _) && last.Argument1 != "True" && last.Argument1 != "False")
                varStatus[last.Argument1] = true;
            if (!int.TryParse(last.Argument2, out _) && last.Argument2 != "True" && last.Argument2 != "False")
                varStatus[last.Argument2] = true;

            foreach (var command in commands.Reverse<Instruction>().Skip(1))
            {
                if (!varStatus.ContainsKey(command.Result) || !varStatus[command.Result])
                {
                    result.Add(new Instruction(command.Label, "noop", null, null, null));
                    continue;
                }

                result.Add(command);

                varStatus[command.Result] = false;
                if (!int.TryParse(command.Argument1, out _) && command.Argument1 != "True" && command.Argument1 != "False")
                    varStatus[command.Argument1] = true;
                if (!int.TryParse(command.Argument2, out _) && command.Argument2 != "True" && command.Argument2 != "False")
                    varStatus[command.Argument2] = true;
            }

            return result.Reverse<Instruction>().ToList();
        }
    }
}
