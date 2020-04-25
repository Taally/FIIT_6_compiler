using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang
{
    static class ThreeAddressCodeRemoveGoto{
        // устранение переходов через переходы
        public static Tuple<bool, List<Instruction>> RemoveGotoThroughGoto(List<Instruction> commands){
            var result = new List<Instruction>();
            bool Checked = false;
            for (int i = 0; i < commands.Count; ++i)
            {
                if (commands[i].Operation == "ifgoto" && 4 <= (commands.Count - i))
                {
                    var com0 = commands[i];
                    var com1 = commands[i + 1];
                    var com2 = commands[i + 2];
                    var com3 = commands[i + 3];

                    // только одна операция
                    if (com1.Operation == "goto" && com2.Operation != "noop" && com3.Operation == "noop" && com0.Argument2 == com2.Label && com1.Argument1 == com3.Label)
                    {
                        string tmpName = ThreeAddressCodeTmp.GenTmpName();
                        result.Add(new Instruction(com0.Label, "EQUAL", "False", com0.Argument1, tmpName));
                        //commands[i] = new Instruction(com0.Label, "EQUAL", "False", com0.Argument1, tmpName); // операция отрицания через EQUAL

                        result.Add(new Instruction("", "ifgoto", tmpName, com3.Label, ""));
                        //commands[i + 1] = new Instruction("", "ifgoto", tmpName, com3.Label, "");

                        result.Add(new Instruction("", com2.Operation, com2.Argument1, com2.Argument2, com2.Result));
                        //commands[i + 2] = new Instruction("", com2.Operation, com2.Argument1, com2.Argument2, com2.Result);

                        result.Add(new Instruction(com3.Label, com3.Operation, com3.Argument1, com3.Argument2, com3.Result));
                        //commands[i + 3] = new Instruction(com3.Label, com3.Operation, com3.Argument1, com3.Argument2, com3.Result);

                        Checked = true;
                        i += 3;
                        continue;
                    }
                }
                result.Add(commands[i]);
            }
            return Tuple.Create(Checked, result);
        }
    }
}