using System;
using System.Collections.Generic;

namespace SimpleLang
{
    public static class ThreeAdressCodeGotoToGoto
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
        public static Tuple<bool, List<Instruction>> ReplaceGotoToGoto(List<Instruction> commands)
        {
            bool changed = false;
            List<GtotScaner> list = new List<GtotScaner>();
            List<Instruction> tmpcommands = new List<Instruction>();
            for (int i = 0; i < commands.Count; i++)
            {
                tmpcommands.Add(commands[i]);
                if (commands[i].Operation == "goto")
                {
                    list.Add(new GtotScaner(i, commands[i].Label, commands[i].Argument1));
                }
            }

            for (int i = 0; i < tmpcommands.Count; i++)
            {
                if (tmpcommands[i].Operation == "goto")
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].label == tmpcommands[i].Argument1)
                        {
                            changed = true;
                            tmpcommands[i] = new Instruction("", "goto", list[j].labelfrom.ToString(), "", "");
                        }
                    }
                }
            }
            return Tuple.Create(changed, tmpcommands);

        }
    }
}
