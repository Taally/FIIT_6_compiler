using System.Collections.Generic;

namespace SimpleLang
{
    public class BasicBlockLeader
    {
        /// <summary>
        /// Разбивает список инструкций на базовые блоки от лидера до лидера
        /// </summary>
        /// <param name="instructions">Список инструкций</param>
        /// <returns>
        /// Вернёт список базовых блоков
        /// </returns>
        public static List<BasicBlock> DivideLeaderToLeader(IReadOnlyList<Instruction> instructions)
        {
            var basicBlockList = new List<BasicBlock>();
            var temp = new List<Instruction>();
            var listOfLeaders = new List<int>();
            for (var i = 0; i < instructions.Count; i++) // формируем лист лидеров
            {
                if (i == 0) // Первая команда ТАК
                {
                    listOfLeaders.Add(i);
                }

                if (instructions[i].Label != null &&
                    IsLabelAlive(instructions, instructions[i].Label) && // Команда с меткой
                    !listOfLeaders.Contains(i))
                {
                    listOfLeaders.Add(i);
                }

                if ((instructions[i].Operation == "goto" ||
                    instructions[i].Operation == "ifgoto") && // Следующая после Goto
                    !listOfLeaders.Contains(i + 1))
                {
                    listOfLeaders.Add(i + 1);
                }
            }

            var j = 0;
            for (var i = 0; i < instructions.Count; i++) // заполняем BasicBlock
            {   // Заполняем временный список
                temp.Add(new Instruction(instructions[i].Label,
                                            instructions[i].Operation,
                                            instructions[i].Argument1,
                                            instructions[i].Argument2,
                                            instructions[i].Result));

                if (i + 1 >= instructions.Count
                    || i == listOfLeaders[(j + 1) >= listOfLeaders.Count ? j : j + 1] - 1) // Следующая команда в списке принадлежит другому лидеру или последняя
                {
                    basicBlockList.Add(new BasicBlock(temp));
                    temp = new List<Instruction>();
                    j++;
                }
            }
            return basicBlockList;
        }

        /// <summary>
        /// Проверка, есть ли переход на метку
        /// </summary>
        /// <param name="instructions">Список инструкций</param>
        /// <param name="checkLabel">Проверяемая метка</param>
        /// <returns>
        /// Возвращает true, если есть переход на эту метку
        /// </returns>
        public static bool IsLabelAlive(IReadOnlyCollection<Instruction> instructions, string checkLabel) // Есть ли переход на метку?
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Operation == "goto")
                {
                    if (instruction.Argument1 == checkLabel)
                    {
                        return true;
                    }
                }
                else if (instruction.Operation == "ifgoto" && instruction.Argument2 == checkLabel)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
