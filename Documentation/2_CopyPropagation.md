## Протяжка копий

### Постановка задачи
Создать оптимизирующий модуль программы, который выполняет протяжку копий

### Команда
И. Потапов

### Зависимые и предшествующие задачи
Предшествующие:

- Трёхадресный код

### Теоретическая часть
В рамках этой задачи необходимо было реализовать оптимизацию протяжка копий. Суть данной оптимизации заключается в том, чтобы заменить в выражениях переменные их значениями:

* До 

```csharp
a = 3;
с = a+a;
b = c;
```

* После

```csharp
a = 3;
с = a+a;
b = a+a;
```

### Практическая часть
Примеры реализации метода:

```csharp
public static class ThreeAddressCodeCopyPropagation
{
    public static (bool wasChanged, IReadOnlyList<Instruction> instructions) PropagateCopies(IReadOnlyList<Instruction> instructions)
    {
        var count = 0;
        var wasChanged = false;
        var result = new List<Instruction>();
        foreach (var instruction in instructions)
        {
            string currentArg1 = instruction.Argument1, currentArg2 = instruction.Argument2;
            var currentOp = instruction.Operation;
            if (!int.TryParse(instruction.Argument1, out var arg))
            {
                var index1 = instructions.Take(count).ToList().FindLastIndex(x => x.Result == instruction.Argument1);
                if (index1 != -1
                    && instructions[index1].Operation == "assign"
                    && !int.TryParse(instructions[index1].Argument1, out arg)
                    && instructions.Skip(index1).Take(count - index1).ToList().FindLastIndex(x => x.Result == instructions[index1].Argument1) == -1)
                {
                    currentArg1 = instructions[index1].Argument1;
                    wasChanged = true;
                }

            }
            if (!int.TryParse(instruction.Argument2, out arg))
            {
                var index2 = instructions.Take(count).ToList().FindLastIndex(x => x.Result == instruction.Argument2);
                if (index2 != -1
                    && instructions[index2].Operation == "assign"
                    && !int.TryParse(instructions[index2].Argument1, out arg)
                    && instructions.Skip(index2).Take(count - index2).ToList().FindLastIndex(x => x.Result == instructions[index2].Argument1) == -1)
                {
                    currentArg2 = instructions[index2].Argument1;
                    wasChanged = true;
                }
            }
            result.Add(new Instruction(instruction.Label, instruction.Operation, currentArg1, currentArg2, instruction.Result));
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
[TestCase(@"
var a, b, c, d, e, x, y, k;
a = b;
c = b - a;
d = c + 1;
e = d * a;
a = x - y;
k = c + a;
",
    ExpectedResult = new string[]
    {
        "a = b",
        "#t1 = b - b",
        "c = #t1",
        "#t2 = #t1 + 1",
        "d = #t2",
        "#t3 = #t2 * b",
        "e = #t3",
        "#t4 = x - y",
        "a = #t4",
        "#t5 = #t1 + #t4",
        "k = #t5"
    },
    TestName = "Test1")]

[TestCase(@"
var a, b, c, d, e, x, y, k;
b = x;
x = 5;
c = b + 5;
d = c;
e = d;
",
    ExpectedResult = new string[]
    {
        "b = x",
        "x = 5",
        "#t1 = b + 5",
        "c = #t1",
        "d = #t1",
        "e = #t1"
    },
    TestName = "Test2")]

public IEnumerable<string> TestPropagateConstants(string sourceCode) =>
    TestTACOptimization(sourceCode, ThreeAddressCodeCopyPropagation.PropagateCopies);
```
