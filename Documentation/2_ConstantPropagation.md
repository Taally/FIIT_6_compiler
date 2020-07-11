## Протяжка констант

### Постановка задачи
Создать оптимизирующий модуль программы, который выполняет протяжку констант

### Команда
И. Потапов

### Зависимые и предшествующие задачи
Предшествующие:

- Трехадресный код

### Теоретическая часть
В рамках этой задачи необходимо было реализовать оптимизацию протяжка констант. Суть данной оптимизации заключается в том, чтобы подставить величины известных констант в выражениях:

* До 

```csharp
a = 6;
с = 2;
b = (a-c)/c;
```

* После

```csharp
a = 6;
с = 2;
b = (6-2)/2;
```

### Практическая часть
Примеры реализации метода:

```csharp
public static class ThreeAddressCodeConstantPropagation
{
    public static (bool wasChanged, IReadOnlyList<Instruction> instructions) PropagateConstants(IReadOnlyCollection<Instruction> instructions)
    {
        var count = 0;
        var wasChanged = false;
        var result = new List<Instruction>();
        foreach (var instruction in instructions)
        {
            string currentArg1 = instruction.Argument1, currentArg2 = instruction.Argument2;
            int arg1;
            var currentOp = instruction.Operation;
            if (instruction.Operation == "assign"
                && instructions.Take(count).ToList().FindLast(x => x.Result == instruction.Argument1) is Instruction cmnd)
            {
                if (cmnd.Operation == "assign"
                    && int.TryParse(cmnd.Argument1, out arg1))
                {
                    currentArg1 = cmnd.Argument1;
                    wasChanged = true;
                }
                result.Add(new Instruction(instruction.Label, currentOp,  currentArg1, currentArg2, instruction.Result));
            }
            else if (instruction.Operation != "assign")
            {
                if (instructions.Take(count).ToList().FindLast(x => x.Result ==         instruction.Argument1) is Instruction cmnd1
                    && cmnd1.Operation == "assign"
                    && int.TryParse(cmnd1.Argument1, out arg1))
                {
                    currentArg1 = cmnd1.Argument1;
                    wasChanged = true;
                }
                if (instructions.Take(count).ToList().FindLast(x => x.Result ==         instruction.Argument2) is Instruction cmnd2
                    && cmnd2.Operation == "assign"
                    && int.TryParse(cmnd2.Argument1, out var arg2))
                {
                    currentArg2 = cmnd2.Argument1;
                    wasChanged = true;
                }
                result.Add(new Instruction(instruction.Label, currentOp,
                    currentArg1, currentArg2, instruction.Result));
            }
            else
            {
                result.Add(instruction);
            }
            ++count;
        }

        return (wasChanged, result);
    }
}
```

### Место в общем проекте (Интеграция)
Данная оптимизация применяется в классе `ThreeAddressCodeOptimizer` наряду со всеми остальными TAC - оптимизациями.

### Тесты

```csharp
[Test]
public void Test1()
{
    var TAC = GenTAC(@"
        var x, y;
        x = 14;
        y = 7 - x;
        x = x + x;
        ");
    var optimizations = new List<Optimization> { ThreeAddressCodeConstantPropagation.PropagateConstants };

    var expected = new List<string>()
    {
        "x = 14",
        "#t1 = 7 - 14",
        "y = #t1",
        "#t2 = 14 + 14",
        "x = #t2"
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
        .Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}

[Test]
public void Test2()
{
    var TAC = GenTAC(@"
        var x, y, b;
        y = 5;
        x = b;
        y = 7;
        x = y + y;
    ");
    var optimizations = new List<Optimization> { ThreeAddressCodeConstantPropagation.PropagateConstants };

    var expected = new List<string>()
    {
        "y = 5",
        "x = b",
        "y = 7",
        "#t1 = 7 + 7",
        "x = #t1"
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
        .Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```
