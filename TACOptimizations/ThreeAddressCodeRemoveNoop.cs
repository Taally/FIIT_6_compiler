using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public static class ThreeAddressCodeRemoveNoop
    {
        public static (bool wasChanged, IReadOnlyList<Instruction> instructions) RemoveEmptyNodes(IReadOnlyList<Instruction> commands)
        {
            var commandsTmp = new List<Instruction>(commands);
            if (commands.Count == 0)
            {
                return (false, commandsTmp);
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
            for (var i = 0; i < commandsTmp.Count - 1; i++)
            {
                var currentCommand = commandsTmp[i];
                if (currentCommand.Operation == "noop" && currentCommand.Label == "")
                {
                    wasChanged = true;
                }
                // we have label here
                else if (currentCommand.Operation == "noop")
                {
                    // if next label is empty, we concat current label with next op
                    if (commandsTmp[i + 1].Label == "")
                    {
                        var nextCommand = commandsTmp[i + 1];
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
                        if (i == commandsTmp.Count - 1)
                        {
                            toAddLast = false;
                        }
                    }
                    // if next label is not empty, instead of noop + next,
                    // rename GOTO current_label to GOTO next_label
                    else
                    {
                        var nextCommand = commandsTmp[i + 1];
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

                        for (var j = i + 1; j < commandsTmp.Count; j++)
                        {
                            commandsTmp[j] = commandsTmp[j].Operation == "goto"
                                          && commandsTmp[j].Argument1 == currentLabel
                                ? new Instruction(
                                    commandsTmp[j].Label,
                                    commandsTmp[j].Operation,
                                    nextLabel,
                                    commandsTmp[j].Argument2,
                                    commandsTmp[j].Result
                                )
                                : (commandsTmp[j].Operation == "ifgoto" && commandsTmp[j].Argument2 == currentLabel)
                                    ? new Instruction(
                                        commandsTmp[j].Label,
                                        commandsTmp[j].Operation,
                                        commandsTmp[j].Argument1,
                                        nextLabel,
                                        commandsTmp[j].Result)
                                    : commandsTmp[j];
                        }
                    }
                }
                else
                {
                    result.Add(commandsTmp[i]);
                }
            }

            if (toAddLast)
            {
                var lastCommand = commandsTmp[commandsTmp.Count - 1];
                var toSkip = lastCommand.Operation == "noop" && lastCommand.Label == "";
                if (toSkip)
                {
                    wasChanged = true;
                }
                else
                {
                    result.Add(commandsTmp[commandsTmp.Count - 1]);
                }
            }
            return (wasChanged, result);
        }
    }
}
