## Удаление мертвого кода, используя InOut из активных переменных

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
public class LiveVariableAnalysisOptimization
{
    public static (bool wasChanged, IReadOnlyList<Instruction> instructions) LiveVariableDeleteDeadCode(IReadOnlyList<Instruction> instructions)
    {
        var wasChanged = false;
        var newInstructions = new List<Instruction>();
        var divResult = BasicBlockLeader.DivideLeaderToLeader(instructions);
        var cfg = new ControlFlowGraph(divResult);
        var activeVariable = new LiveVariableAnalysis();
        var resActiveVariable = activeVariable.Execute(cfg);
        foreach (var x in divResult)
        {
            var instructionsTemp = x.GetInstructions();
            if (resActiveVariable.ContainsKey(x))
            {
                var InOutTemp = resActiveVariable[x];
                foreach (var i in instructionsTemp)
                {
                    if (!InOutTemp.Out.Contains(i.Result) && i.Operation == "assign" && i.Argument1 != i.Result)
                    {
                        wasChanged = true;
                        if (i.Label != "")
                        {
                            newInstructions.Add(new Instruction(i.Label, "noop", "", "", ""));
                        }
                    }
                    else
                    {
                        newInstructions.Add(i);
                    }
                }
            }
        }
        return (wasChanged, newInstructions);
    }
}
```

### Место в общем проекте (Интеграция)
Данная оптимизация применяется в классе `ThreeAddressCodeOptimizer` наряду со всеми остальными TAC - оптимизациями.

### Пример работы

```csharp
[Test]
public void Test1()
{
    var TAC = GenTAC(@"
        var a,b,c;
        input (b);
        a = b + 1;
        c = 6;
        if a < c
	        c = b - a;
        else
	        c = b + a;
        print (c);");

            var optimizations = new List<Optimization> { LiveVariableAnalysisOptimization.LiveVariableDeleteDeadCode };

            var expected = new List<string>()
            {
                "input b",
                "#t1 = b + 1",
                "a = #t1",
                "#t2 = a < c",
                "if #t2 goto L1",
                "#t3 = b + a",
                "c = #t3",
                "goto L2",
                "L1: #t4 = b - a",
                "c = #t4",
                "L2: noop",
                "print c"
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
```
