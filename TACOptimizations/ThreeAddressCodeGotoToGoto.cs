using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodeGotoToGoto
    {
        private struct GtotScaner
        {
            public string Label { get; }
            public string LabelFrom { get; }

            public GtotScaner(string label, string labelFrom)
            {
                Label = label;
                LabelFrom = labelFrom;
            }
        }

        public static (bool wasChanged, List<Instruction> instructions) ReplaceGotoToGoto(List<Instruction> commands)
        {
            var wasChanged = false;
            var list = new List<GtotScaner>();
            var tmpCommands = new List<Instruction>();
            foreach (var command in commands)
            {
                tmpCommands.Add(command);
                if (command.Operation == "goto")
                {
                    list.Add(new GtotScaner(command.Label, command.Argument1));
                }

                if (command.Operation == "ifgoto")
                {
                    list.Add(new GtotScaner(command.Label, command.Argument2));
                }
            }

            for (var i = 0; i < tmpCommands.Count; i++)
            {
                if (tmpCommands[i].Operation == "goto")
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[j].Label == tmpCommands[i].Argument1 && list[j].LabelFrom != tmpCommands[i].Argument1)
                        {
                            wasChanged = true;
                            tmpCommands[i] = new Instruction(tmpCommands[i].Label, "goto", list[j].LabelFrom, "", "");
                        }
                    }
                }

                if (tmpCommands[i].Operation == "ifgoto")
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[j].Label == tmpCommands[i].Argument2)
                        {
                            if (list[j].Label == tmpCommands[i].Argument2 && list[j].LabelFrom != tmpCommands[i].Argument2)
                            {
                                wasChanged = true;
                                tmpCommands[i] = new Instruction(tmpCommands[i].Label, "ifgoto", tmpCommands[i].Argument1, list[j].LabelFrom, "");
                            }
                        }
                    }
                }
            }
            return (wasChanged, tmpCommands);
        }
    }
}
