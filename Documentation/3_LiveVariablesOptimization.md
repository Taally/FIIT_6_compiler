## Удаление мёртвого кода, используя InOut из активных переменных

### Постановка задачи
Создать оптимизирующий модуль программы, который удаляет мёртвый код, используя InOut из активных переменных

### Команда
И. Потапов

### Зависимые и предшествующие задачи
Предшествующие:

- Построение графа потока управления
- Анализ активных переменных

### Теоретическая часть
В рамках этой задачи необходимо было реализовать оптимизацию удаление мёртвого кода, причём данная оптимизация применяется ко всему коду. Суть данной оптимизации заключается в том, чтобы удалить мёртвый код, используя информацию об Активных переменных:

### Практическая часть
Примеры реализации метода:

```csharp
public class LiveVariablesOptimization
{
    public static void DeleteDeadCode(ControlFlowGraph cfg)
    {
        var info = new LiveVariables().Execute(cfg);
        foreach (var block in cfg.GetCurrentBasicBlocks())
        {
            var blockInfo = info[block].Out;
            (var wasChanged, var newInstructions) = DeleteDeadCodeWithDeadVars.DeleteDeadCode(block.GetInstructions(), blockInfo);
            if (wasChanged)
            {
                block.ClearInstructions();
                block.AddRangeOfInstructions(newInstructions);
            }
        }
    }
}
```

### Место в общем проекте (Интеграция)
Оптимизация живых переменных использует главным образом граф потока управления и вычисленную информацию о живых переменных в соответствующих классах.

### Тесты

```csharp
[TestCase(@"
var a,b,c;
input (b);
a = b + 1;
c = 6;
if a < b
    c = b - a;
else
    c = b + a;
print (c);
",
    ExpectedResult = new[]
    {
        "input b",
        "#t1 = b + 1",
        "a = #t1",
        "noop",
        "#t2 = a < b",
        "if #t2 goto L1",
        "#t3 = b + a",
        "c = #t3",
        "goto L2",
        "L1: #t4 = b - a",
        "c = #t4",
        "L2: noop",
        "print c",
    },
    TestName = "Simple")]

public IEnumerable<string> TestLiveVariablesOptimization(string sourceCode)
{
    var cfg = GenCFG(sourceCode);
    LiveVariablesOptimization.DeleteDeadCode(cfg);
    return cfg.GetInstructions().Select(x => x.ToString());
}
```
