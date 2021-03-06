## Разбиение на базовые блоки (от лидера до лидера)

### Постановка задачи
Реализовать разбиение на базовые блоки от лидера до лидера.

### Команда
К. Галицкий, А. Черкашин

### Зависимые и предшествующие задачи
Предшествующие задачи:

* Трёхадресный код
* Создание структуры базового блока и CFG – графа базовых блоков

Зависимые задачи:

* Def-Use информация и удаление мёртвого кода
* Свёртка констант
* Учет алгебраических тождеств
* Протяжка констант
* Протяжка копий
* Живые и мёртвые переменные и удаление мёртвого кода
* Построение CFG. Обход потомков и обход предков для каждого базового блока

### Теоретическая часть
В рамках этой задачи необходимо было реализовать разбиение трёхадресного кода на базовые блоки.
Базовый блок – это блок команд от лидера до лидера.
Команды лидеры:

* первая команда
* любая команда, на которую есть переход
* любая команда, непосредственно следующая за переходом

Пример разбиения трёхадресного кода на базовые блоки:

![Пример разбиения](2_BasicBlockLeader/pic1.jpg)

### Практическая часть
Реализовали задачу следующим способом: заполнили список лидеров трёхадресного кода, разбили код на блоки от лидера до лидера (исключая последнего), вернули список базовых блоков. 
Пример создания списка операций лидеров:
```csharp
for (var i = 0; i < instructions.Count; i++) // формируем список лидеров
{
    if (i == 0) // Первая команда трёхадресного кода
    {
        listOfLeaders.Add(i);
    }

    if (instructions[i].Label != null &&
        IsLabelAlive(instructions, instructions[i].Label) && // Команда содержит метку, на которую существует переход
        !listOfLeaders.Contains(i)) // проверка на наличие данного лидера в списке лидеров
    {
        listOfLeaders.Add(i);
    }

    if ((instructions[i].Operation == "goto" ||
        instructions[i].Operation == "ifgoto") && // Команда является следующей после операции перехода (goto или ifgoto)
        !listOfLeaders.Contains(i + 1)) // проверка на наличие данного лидера в списке лидеров
    {
        listOfLeaders.Add(i + 1);
    }
}
```

Результатом работы является список базовых блоков, состоящий из команд трёхадресного кода, разбитых от лидера до лидера:
```csharp
return basicBlockList;
```

### Место в общем проекте (Интеграция)
Используется после создания трёхадресного кода. Необходим для разбиения трёхадресного кода на базовые блоки.
```csharp
/* Main.cs */
var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
parser.root.Visit(threeAddrCodeVisitor);
var threeAddressCode = threeAddrCodeVisitor.Instructions;
var optResult = ThreeAddressCodeOptimizer.OptimizeAll(threeAddressCode);
var divResult = BasicBlockLeader.DivideLeaderToLeader(optResult);
```

### Тесты
В тестах проверяется, что для заданного трёхадресного кода разбиение на базовые блоки возвращает ожидаемый результат:
```csharp
[Test]
public void LabelAliveTest()
{
    var TAC = GenTAC(@"
var a, b, c;
goto 3;
a = 54;
3: b = 11;
");

    var expected = new List<BasicBlock>()
    {
        new BasicBlock(new List<Instruction>()
        {
            new Instruction("", "goto", "3", "", "")
        }),
        new BasicBlock(new List<Instruction>()
        {
            new Instruction("", "assign", "54", "", "a")
        }),
        new BasicBlock(new List<Instruction>()
        {
            new Instruction("3", "assign", "11", "", "b")
        }),
    };
    var actual = BasicBlockLeader.DivideLeaderToLeader(TAC);

    AssertSet(expected, actual);
}

[Test]
public void LabelNotAliveTest()
{
    var TAC = GenTAC(@"
var a, b, c;
goto 4;
a = 54;
3: b = 11;
");

    var expected = new List<BasicBlock>()
    {
        new BasicBlock(new List<Instruction>()
        {
            new Instruction("", "goto", "4", "", "")
        }),
        new BasicBlock(new List<Instruction>()
        {
            new Instruction("", "assign", "54", "", "a"),
            new Instruction("3", "assign", "11", "", "b")
        }),
    };
    var actual = BasicBlockLeader.DivideLeaderToLeader(TAC);

    AssertSet(expected, actual);
}

[Test]
public void OneBlockTest()
{
    var TAC = GenTAC(@"
var a, b, c;
a = 54;
b = 11;
");

    var expected = new List<BasicBlock>()
    {
        new BasicBlock(new List<Instruction>()
        {
            new Instruction("", "assign", "54", "", "a"),
            new Instruction("", "assign", "11", "", "b")
        }),
    };
    var actual = BasicBlockLeader.DivideLeaderToLeader(TAC);

    AssertSet(expected, actual);
}
```
