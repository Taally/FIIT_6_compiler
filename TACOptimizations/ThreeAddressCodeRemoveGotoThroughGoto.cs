using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{
    static class ThreeAddressCodeRemoveGotoThroughGoto
    {
        // устранение переходов через переходы
        /*public static Tuple<bool, List<Instruction>> RemoveGotoThroughGoto(List<Instruction> instructions)
        {
            if (instructions is null)
                throw new ArgumentNullException("instructions is null");

            bool isChange = false;
            var newInstructions = new List<Instruction>();

            for (int i = 0; i < instructions.Count; ++i)
            {
                var com0 = instructions[i];
                if (instructions[i].Operation == "ifgoto" && 4 <= (instructions.Count - i))
                {
                    var com1 = instructions[i + 1];
                    var com2 = instructions[i + 2];

                    // 
                    if (com1.Operation == "goto" && com1.Label == "" && com2.Operation != "noop" && com0.Argument2 == com2.Label)
                    {
                        // search end label with "noop" operation
                        var endLabelIndex = FindLabelIndex(instructions, i + 2, com1.Argument1);
                        if (0 < endLabelIndex || endLabelIndex < instructions.Count || instructions[endLabelIndex].Operation == "noop")
                        {
                            // 
                            string tmpName = ThreeAddressCodeTmp.GenTmpName();
                            newInstructions.Add(new Instruction(com0.Label, "EQUAL", "False", com0.Argument1, tmpName)); // операция отрицания через EQUAL
                            newInstructions.Add(new Instruction("", "ifgoto", tmpName, com1.Argument1, ""));

                            // copy instructions for true branch
                            newInstructions.Add(new Instruction("", com2.Operation, com2.Argument1, com2.Argument2, com2.Result));
                            for (int k = i + 3; k < endLabelIndex; ++k)
                            {
                                var bla = instructions[k];
                                newInstructions.Add(new Instruction(bla.Label, bla.Operation, bla.Argument1, bla.Argument2, bla.Result));
                            }

                            var endLabel = instructions[endLabelIndex];
                            newInstructions.Add(new Instruction(endLabel.Label, endLabel.Operation, endLabel.Argument1, endLabel.Argument2, endLabel.Result));
                            i = endLabelIndex;
                            isChange = true;
                            continue;
                        }
                    }
                }

                newInstructions.Add(new Instruction(com0.Label, com0.Operation, com0.Argument1, com0.Argument2, com0.Result));
            }

            return Tuple.Create(isChange, newInstructions);
        }*/


        // старый код. работает только с одной инструкцией в true ветке
        public static Tuple<bool, List<Instruction>> RemoveGotoThroughGoto(List<Instruction> instructions)
        {
            if (instructions is null)
                throw new ArgumentNullException("instructions is null");

            bool isChange = false;
            var newInstructions = new List<Instruction>();

            for (int i = 0; i < instructions.Count; ++i)
            {
                if (instructions[i].Operation == "ifgoto" && 4 <= (instructions.Count - i))
                {
                    var com0 = instructions[i];
                    var com1 = instructions[i + 1];
                    var com2 = instructions[i + 2];
                    var com3 = instructions[i + 3];

                    // только одна операция
                    if (com1.Operation == "goto" && com1.Label == "" && com2.Operation != "noop" && com3.Operation == "noop" && com0.Argument2 == com2.Label && com1.Argument1 == com3.Label)
                    {
                        string tmpName = ThreeAddressCodeTmp.GenTmpName();
                        newInstructions.Add(new Instruction(com0.Label, "EQUAL", "False", com0.Argument1, tmpName)); // операция отрицания через EQUAL
                        newInstructions.Add(new Instruction("", "ifgoto", tmpName, com3.Label, ""));
                        newInstructions.Add(new Instruction("", com2.Operation, com2.Argument1, com2.Argument2, com2.Result));
                        newInstructions.Add(new Instruction(com3.Label, com3.Operation, com3.Argument1, com3.Argument2, com3.Result));

                        i += 3;
                        continue;
                    }
                }
                newInstructions.Add(new Instruction(instructions[i].Label, instructions[i].Operation, instructions[i].Argument1, instructions[i].Argument2, instructions[i].Result));
            }
            return Tuple.Create(isChange, newInstructions);
        }

        // 
        private static int FindLabelIndex(IList<Instruction> instructions, int offset, string label)
        {
            if (offset < 0 || instructions.Count < offset)
                throw new ArgumentException($"incorrect offset: {offset}");
            if (instructions is null)
                throw new ArgumentNullException("instructions is null");
            if (label is null)
                throw new ArgumentNullException("label is null");

            for (int i = offset; i < instructions.Count; ++i)
                if (instructions[i].Label == label)
                    return i;
            return -1;
        }
    }
}
