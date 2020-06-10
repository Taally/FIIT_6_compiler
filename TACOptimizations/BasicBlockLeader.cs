using System.Collections.Generic;

namespace SimpleLang
{
    public class BasicBlockLeader
    {
       static public List<BasicBlock> DivideLeaderToLeader(List<Instruction> instructions)
       {
            List<BasicBlock> basicBlockList = new List<BasicBlock>();
            List<Instruction> temp = new List<Instruction>();
            List<int> listOfLeaders = new List<int>();
            for (int i = 0; i < instructions.Count; i++) // формируем лист лидеров
            {
                if (i == 0) //Первая команда ТАК
                {
                    listOfLeaders.Add(i);
                }

                if (instructions[i].Label != null 
                    && IsLabelAlive(instructions, instructions[i].Label)) //Команда с меткой
                {
                    if (listOfLeaders.Contains(i))
                    {
                        continue;
                    }
                    else
                    {
                        listOfLeaders.Add(i);
                    }
                }

                if (instructions[i].Operation == "goto" 
                    || instructions[i].Operation == "ifgoto") //Следующая после Goto
                {
                    if (listOfLeaders.Contains(i + 1))
                    {
                        continue;
                    }
                    else
                    {
                        listOfLeaders.Add(i + 1);
                    }
                }
            }
            int j = 0;
            for (int i = 0; i < instructions.Count; i++) // заполняем BasicBlock
            {   //Заполняем временный список
                temp.Add(new Instruction(instructions[i].Label,
                                            instructions[i].Operation,
                                            instructions[i].Argument1,
                                            instructions[i].Argument2,
                                            instructions[i].Result));

                if (i + 1 >= instructions.Count 
                    || i == listOfLeaders[((j + 1) >= listOfLeaders.Count ? j : j + 1)] - 1) // Следующая команда в списке принадлежит другому лидеру или последняя
                {
                    basicBlockList.Add(new BasicBlock(temp));
                    temp = new List<Instruction>();
                    j++;
                }
            }
            return basicBlockList;
        }

        static public bool IsLabelAlive(List<Instruction> instructions, string checkLabel) //Есть ли переход на метку ? 
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].Operation == "goto")
                {
                    if (instructions[i].Argument1 == checkLabel) 
                        return true;
                }
                else if (instructions[i].Operation == "ifgoto")
                {
                    if (instructions[i].Argument2 == checkLabel) 
                        return true;
                }
            }
            return false;
        }
    }
}
