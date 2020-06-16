using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAddressCodeGotoToGoto
    {

        public struct GtotScaner
        {
            public int index;
            public string label;
            public string labelfrom;

            public GtotScaner(int index, string label, string labelfrom)
            {
                this.index = index;
                this.label = label;
                this.labelfrom = labelfrom;
            }
        }
        public static (bool wasChanged, List<Instruction> instructions) ReplaceGotoToGoto(List<Instruction> commands)
        {
            var wasChanged = false;
            var list = new List<GtotScaner>();
            var tmpcommands = new List<Instruction>();
            for (var i = 0; i < commands.Count; i++)
            {
                tmpcommands.Add(commands[i]);
                if (commands[i].Operation == "goto")
                {
                    list.Add(new GtotScaner(i, commands[i].Label, commands[i].Argument1));
                }

                if (commands[i].Operation == "ifgoto")
                {
                    list.Add(new GtotScaner(i, commands[i].Label, commands[i].Argument2));
                }
            }

            for (var i = 0; i < tmpcommands.Count; i++)
            {

                if (tmpcommands[i].Operation == "goto")
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[j].label == tmpcommands[i].Argument1)
                        {
                            if (tmpcommands[i].Argument1.ToString() == list[j].labelfrom.ToString())
                            {
                                wasChanged |= false;
                            }
                            else
                            {
                                wasChanged |= true;
                                tmpcommands[i] = new Instruction(tmpcommands[i].Label, "goto", list[j].labelfrom.ToString(), "", "");
                            }

                        }
                    }
                }

                if (tmpcommands[i].Operation == "ifgoto")
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[j].label == tmpcommands[i].Argument2)
                        {

                            if (tmpcommands[i].Argument2.ToString() == list[j].labelfrom.ToString())
                            {
                                wasChanged |= false;
                            }
                            else
                            {
                                tmpcommands[i] = new Instruction(tmpcommands[i].Label, "ifgoto", tmpcommands[i].Argument1, list[j].labelfrom.ToString(), "");
                                wasChanged |= true;
                            }

                        }
                    }
                }
            }
            return (wasChanged, tmpcommands);
        }
    }
}
