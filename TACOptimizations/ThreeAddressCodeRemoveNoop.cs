using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public static class ThreeAddressCodeRemoveNoop
    {
        public static (bool wasChanged, List<Instruction> instructions) RemoveEmptyNodes(List<Instruction> commands)
        {
            if (commands.Count == 0)
            {
                return (false, commands);
            }

            var result = new List<Instruction>();
            var wasChanged = false;
            var toAddLast = true;

            // three cases:
            // a) Command = noop without label, in this case just remove it
            // b) Command = noop with label, next command - op without label.
            //    Then just combine current label and next op.
            // c) Command = noop with label, in this case,
            //    rename all GOTO current_label to GOTO next_label
            for (var i = 0; i < commands.Count - 1; i++)
            {
                var currentCommand = commands[i];
                if (currentCommand.Operation == "noop" && currentCommand.Label == "")
                {
                    wasChanged = true;
                }
                // we have label here
                else if (currentCommand.Operation == "noop")
                {
                    // if next label is empty, we concat current label with next op
                    if (commands[i + 1].Label == "")
                    {
                        var nextCommand = commands[i + 1];
                        wasChanged = true;
                        result.Add(
                            new Instruction(
                                currentCommand.Label,
                                nextCommand.Operation,
                                nextCommand.Argument1,
                                nextCommand.Argument2,
                                nextCommand.Result
                            )
                        );
                        i += 1;
                        if (i == commands.Count - 1)
                        {
                            toAddLast = false;
                        }
                    }
                    // if next label is not empty, instead of noop + next,
                    // rename GOTO current_label to GOTO next_label
                    else
                    {
                        var nextCommand = commands[i + 1];
                        wasChanged = true;
                        var currentLabel = currentCommand.Label;
                        var nextLabel = nextCommand.Label;

                        result = result
                            .Select(com =>
                                com.Operation == "goto" && com.Argument1 == currentLabel
                                    ? new Instruction(com.Label, com.Operation, nextLabel, com.Argument2, com.Result)
                                    : com.Operation == "ifgoto" && com.Argument2 == currentLabel
                                        ? new Instruction(com.Label, com.Operation, com.Argument1, nextLabel, com.Result)
                                        : com
                            ).ToList();

                        for (var j = i + 1; j < commands.Count; j++)
                        {
                            commands[j] = commands[j].Operation == "goto"
                                          && commands[j].Argument1 == currentLabel
                                ? new Instruction(
                                    commands[j].Label,
                                    commands[j].Operation,
                                    nextLabel,
                                    commands[j].Argument2,
                                    commands[j].Result
                                )
                                : (commands[j].Operation == "ifgoto" && commands[j].Argument2 == currentLabel)
                                    ? new Instruction(
                                        commands[j].Label,
                                        commands[j].Operation,
                                        commands[j].Argument1,
                                        nextLabel,
                                        commands[j].Result)
                                    : commands[j];
                        }
                    }
                }
                else
                {
                    result.Add(commands[i]);
                }
            }

            if (toAddLast)
            {
                var lastCommand = commands[commands.Count - 1];
                var toSkip = lastCommand.Operation == "noop" && lastCommand.Label == "";
                if (toSkip)
                {
                    wasChanged = true;
                }
                else
                {
                    result.Add(commands[commands.Count - 1]);
                }
            }
            return (wasChanged, result);
        }
    }
}
