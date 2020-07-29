## Устранение переходов через переходы

### Постановка задачи
Произвести оптимизацию в трёхадресном коде программы для устранения переходов через переходы.

### Команда
Карякин В.В., Карякин Д.В.

### Зависимые и предшествующие задачи
Предшествующие задачи:

- Генерация трёхадресного кода

### Теоретическая часть
В рамках данной задачи необходимо реализовать оптимизацию трёхадресного кода, которая устраняет безусловный оператор перехода. На изображении ниже показана работа данной оптимизации.

![Пример работы оптимизации](2_GotoThroughGoto/img1.png)

### Практическая часть
В алгоритме оптимизации происходит последовательный проход по трёхадресному коду программы. Если последовательность трёхадресных команд удовлетворяет условию, которое позволяет провести оптимизацию, то она проводится, иначе команды остаются в неизменном виде.

Код оптимизации:
```csharp
/* ThreeAddressCodeRemoveGotoThroughGoto.cs */
for (var i = 0; i < instructions.Count; ++i)
{
    if (instructions[i].Operation == "ifgoto" && 4 <= (instructions.Count - i))
    {
        var com0 = instructions[i];
        var com1 = instructions[i + 1];
        var com2 = instructions[i + 2];
        var com3 = instructions[i + 3];

        if (com1.Operation == "goto" && com1.Label == "" && com2.Operation != "noop" && com0.Argument2 == com2.Label && com1.Argument1 == com3.Label)
        {
            var tmpName = ThreeAddressCodeTmp.GenTmpName();
            newInstructions.Add(new Instruction(com0.Label, "NOT", com0.Argument1, "", tmpName));
            newInstructions.Add(new Instruction("", "ifgoto", tmpName, com3.Label, ""));

            var label = com2.Label.StartsWith("L") && uint.TryParse(com2.Label.Substring(1), out _) ? "" : com2.Label;
            newInstructions.Add(new Instruction(label, com2.Operation, com2.Argument1, com2.Argument2, com2.Result));
            newInstructions.Add(com3.Copy());

            wasChanged = true;
            i += 3;
            continue;
        }
    }
    newInstructions.Add(instructions[i].Copy());
            
```

### Место в общем проекте (Интеграция)
Устранение переходов через переходы применяется в списке оптимизаций к трёхадресному коду:
```csharp
/* ThreeAddressCodeOptimizer.cs */
private static List<Optimization> AllCodeOptimizations => new List<Optimization>
{ 
    /* ... */ 
    ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto,
    /* ... */
};
```

### Тесты
Метод ```GenTAC``` вызывается для обновления глобальных переменных которые использует оптимизация. Схема тестирования выглядит следующим образом: создаётся TAC; затем применяется оптимизация; после построчно сравниваются строки трёхадресного кода ожидаемого результата и полученного после оптимизации TAC. Ниже приведён один из тестов.

```csharp
[TestCase(@"
var a;
if (1 < 2)
    goto 3;
2: goto 4;
3: a = 0;
4: a = 1;
666: a = false;
",
    ExpectedResult = new string[]
    {
        "#t1 = 1 >= 2",
        "#t2 = !#t1",
        "if #t2 goto 3",
        "2: goto 4",
        "3: a = 0",
        "4: a = 1",
        "666: a = False"
    },
    TestName = "Optimization")]

public IEnumerable<string> TestGotoThroughGoto(string sourceCode) =>
    TestTACOptimization(sourceCode, allCodeOptimization: ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto);
```
