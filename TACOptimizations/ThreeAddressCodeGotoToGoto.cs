using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public static class ThreeAddressCodeGotoToGoto
    {
        public struct GtotScaner
        {
            public string Label { get; }
            public string LabelFrom { get; }
            public string Operation { get; }
            public int InstructionNum { get; }
            public GtotScaner(string label, string labelFrom, string operation, int instructionNum)
            {
                Label = label;
                LabelFrom = labelFrom;
                Operation = operation;
                InstructionNum = instructionNum;
            }
        }

        public static (bool wasChanged, List<Instruction> instructions) ReplaceGotoToGoto(List<Instruction> commands)
        {
            var wasChanged = false;
            var list = new List<GtotScaner>();
            var tmpCommands = new List<Instruction>();
            for (var i = 0; i < commands.Count; i++)
            {
                if (commands[i].Operation == "goto")
                {
                    list.Add(new GtotScaner(commands[i].Label, commands[i].Argument1, commands[i].Operation, i));
                }

                if (commands[i].Operation == "ifgoto")
                {
                    list.Add(new GtotScaner(commands[i].Label, commands[i].Argument2, commands[i].Operation, i));
                }
            }

            var addNewLabels = new Dictionary<int, string>();
            var k = 0;

            for (var i = 0; i < commands.Count; i++)
            {
                if (commands[i].Operation == "goto")
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[j].Label == commands[i].Argument1 && list[j].LabelFrom != commands[i].Argument1 && list[j].Operation == "goto")
                        {
                            wasChanged = true;
                            tmpCommands.Add(new Instruction(commands[i].Label, "goto", list[j].LabelFrom, "", ""));
                            i++;
                            break;
                        }
                        else if (list[j].Label == commands[i].Argument1 && list[j].LabelFrom != commands[i].Argument1 && list[j].Operation == "ifgoto" && CountGoTo(list, list[j].Label) <= 1)
                        {
                            k++;
                            wasChanged = true;
                            tmpCommands.Add(new Instruction("",
                                commands[list[j].InstructionNum].Operation,
                                commands[list[j].InstructionNum].Argument1,
                                commands[list[j].InstructionNum].Argument2,
                                commands[list[j].InstructionNum].Result));
                            if (commands[list[j].InstructionNum + 1] != null && commands[list[j].InstructionNum + 1].Label != "")
                            {
                                tmpCommands.Add(new Instruction("", "goto", commands[list[j].InstructionNum + 1].Label, "", ""));
                                i += 1;
                                addNewLabels.Add(list[j].InstructionNum, commands[list[j].InstructionNum + 1].Label);
                                break;
                            }
                            else if (commands[list[j].InstructionNum + 1] == null || commands[list[j].InstructionNum + 1].Label == "")
                            {
                                var tmpName = ThreeAddressCodeTmp.GenTmpLabel();
                                tmpCommands.Add(new Instruction("", "goto", tmpName, "", ""));
                                addNewLabels.Add(list[j].InstructionNum + k, tmpName);
                                i += 1;
                                break;
                            }
                        }
                    }
                }
                else if (commands[i].Operation == "ifgoto" && !addNewLabels.ContainsKey(i))
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[j].Label == commands[i].Argument2)
                        {
                            if (list[j].Label == commands[i].Argument2 && list[j].LabelFrom != commands[i].Argument2)
                            {
                                wasChanged = true;
                                tmpCommands.Add(new Instruction(commands[i].Label, "ifgoto", commands[i].Argument1, list[j].LabelFrom, ""));
                                i++;
                                break;
                            }
                        }
                    }
                }
                tmpCommands.Add(new Instruction(commands[i].Label, commands[i].Operation, commands[i].Argument1, commands[i].Argument2, commands[i].Result));
            }

            foreach (var x in addNewLabels.Keys)
            {
                tmpCommands[x] = new Instruction(addNewLabels[x], "noop", "", "", "");
            }

            return (wasChanged, tmpCommands);
        }

        public static int CountGoTo(List<GtotScaner> a, string label)
        {
            var tmpCount = 0;
            foreach (var x in a)
            {
                if (x.LabelFrom == label && (x.Operation == "goto" || x.Operation == "ifgoto"))
                {
                    tmpCount++;
                }
            }
            return tmpCount;
        }
    }
}
