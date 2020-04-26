using System.Collections.Generic;

namespace SimpleLang
{
    class ThreeAdressCodeGotoToGoto
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

        public static void ReplaceGotoToGoto(List<Instruction> commands)
        {
            List<GtotScaner> list = new List<GtotScaner>();
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Operation == "goto")
                {
                    list.Add(new GtotScaner(i, commands[i].Label, commands[i].Argument1));
                }
            }

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Operation == "goto")
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].label == commands[i].Argument1)
                        {
                            commands[i] = new Instruction("", "goto", list[j].labelfrom.ToString(), "", "");
                        }
                    }
                }
            }
        }
    }
}
