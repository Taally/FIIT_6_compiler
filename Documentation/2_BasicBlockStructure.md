### Создание структуры ББл

#### Постановка задачи
Реализовать структуру базового блока

#### Команда
А. Пацеев, И. Ушаков

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* Трехадресный код

Зависимые задачи:
* Разбиение на ББл (от лидера до лидера)
* Def-Use информация и удаление мертвого кода
* Свертка констант
* Учет алгебраических тождеств
* Протяжка констант
* Протяжка копий
* Живые и мертвые переменные и удаление мертвого кода
* Построение CFG. Обход потомков и обход предков для каждого базового блока

#### Теоретическая часть
В рамках этой задачи необходимо было реализовать структуру базового блока. Необходимым условием является наличие конструктора, а также списка инструкций для заданного базового блока.

#### Практическая часть
```csharp
public class BasicBlock
    {
        private readonly List<Instruction> _instructions;

        public BasicBlock() => _instructions = new List<Instruction>();

        public BasicBlock(List<Instruction> instructions) => _instructions = instructions;

        public List<Instruction> GetInstructions() => _instructions.ToList();

        public void InsertInstruction(int index, Instruction instruction) => _instructions.Insert(index, instruction);

        public void AddInstruction(Instruction instruction) => _instructions.Add(instruction);

        public void InsertRangeOfInstructions(int index, List<Instruction> instruction) => _instructions.InsertRange(index, instruction);

        public void AddRangeOfInstructions(List<Instruction> instruction) => _instructions.AddRange(instruction);

        public void RemoveInstructionByIndex(int index) => _instructions.RemoveAt(index);
    }
```

#### Место в общем проекте (Интеграция)
Данная структура была задействована во всех задачах, в которых использовались ББл, например в задаче разбиения на ББл (от лидера до лидера) 
