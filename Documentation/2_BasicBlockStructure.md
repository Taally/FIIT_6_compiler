## Создание структуры базового блока

### Постановка задачи
Реализовать структуру базового блока.

### Команда
А. Пацеев, И. Ушаков

### Зависимые и предшествующие задачи
Предшествующие задачи:

* Трёхадресный код

Зависимые задачи:

* Разбиение на базовые блоки (от лидера до лидера)
* Def-Use информация и удаление мёртвого кода
* Свёртка констант
* Учет алгебраических тождеств
* Протяжка констант
* Протяжка копий
* Живые и мёртвые переменные и удаление мёртвого кода
* Построение CFG. Обход потомков и обход предков для каждого базового блока

### Теоретическая часть
В рамках этой задачи необходимо было реализовать структуру базового блока. Необходимым условием является наличие конструктора, а также списка инструкций для заданного базового блока.

### Практическая часть
```csharp
public class BasicBlock
{
    private readonly List<Instruction> _instructions;

    public BasicBlock() => _instructions = new List<Instruction>();

    public BasicBlock(List<Instruction> instructions) => _instructions = instructions;
    public BasicBlock(IEnumerable<Instruction> instructions) => _instructions = instructions.ToList();

    public IReadOnlyList<Instruction> GetInstructions() => _instructions;

    public void InsertInstruction(int index, Instruction instruction) => _instructions.Insert(index, instruction);

    public void AddInstruction(Instruction instruction) => _instructions.Add(instruction);

    public void InsertRangeOfInstructions(int index, IEnumerable<Instruction> instruction) => _instructions.InsertRange(index, instruction);

    public void AddRangeOfInstructions(IEnumerable<Instruction> instruction) => _instructions.AddRange(instruction);

    public void RemoveInstructionByIndex(int index) => _instructions.RemoveAt(index);

    public void ClearInstructions() => _instructions.Clear();
}
```

### Место в общем проекте (Интеграция)
Данная структура была задействована во всех задачах, в которых использовались базовые блоки, например в задаче разбиения на базовые блоки (от лидера до лидера).
