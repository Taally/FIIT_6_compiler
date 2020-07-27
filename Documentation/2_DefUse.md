## Def-Use информация и удаление мёртвого кода на её основе 

### Постановка задачи
Накопление Def-Use информации в пределах базового блока (ББл) и удаление мёртвого кода на основе этой информации.

### Команда
А. Татарова, Т. Шкуро

### Зависимые и предшествующие задачи
Предшествующие задачи:

- Трёхадресный код
- Разбиение кода на базовые блоки

### Теоретическая часть
В рамках этой задачи переменные делятся на два типа: **def** и **use**.
Def --- это определение переменной, т.е. этой переменной было присвоено значение в данном ББл.
Use --- это использование переменной, т.е. эта переменная использовалась в каком-либо выражении в данном ББл.
Например, в следующем выражении **a** будет являться def-переменной, а **b** и **c** - use-переменными:
```
a = b + c;
```
На основе трёхадресного кода составляется список Def-Use: список **def**-переменных, где для каждой **def**-переменной есть список использований этой переменной, т.е. список **use**.
После формирования Def-Use информации по всему коду ББл производится удаление мёртвого кода --- удаление определений тех переменных, список использования которых пуст. Для удаления мёртвого кода список команд проходится снизу вверх, при удалении команды производится обновление информации Def-Use. 

### Практическая часть
Первым шагом по полученным командам трёхадресного кода составляется список Def-Use:
```csharp
public class Use
{
    public Def Parent { get; set; } // определение переменной
    public int OrderNum { get; set; } // номер команды в трёхадресном коде
}

public class Def
{
    public List<Use> Uses { get; set; } // список использований переменной
    public int OrderNum { get; set; } // номер команды в трёхадресном коде
    public string Id { get; set; } // идентификатор переменной
}
    
public static List<Def> DefList;

private static void FillLists(List<Instruction> commands)
{
    DefList = new List<Def>();
    for (int i = 0; i < commands.Count; ++i)
    {
        // если оператор является оператором присваивания, оператором 
        // арифметических или логических операций или оператором ввода,
        // добавляем в список DefList результат этой операции
        if (operations.Contains(commands[i].Operation))
            DefList.Add(new Def(i, commands[i].Result));
        // если в правой части оператора переменные,
        // и их определение есть в списке DefList,
        // добавляем их в соответствующий список Uses
        AddUse(commands[i].Argument1, commands[i], i);
        AddUse(commands[i].Argument2, commands[i], i);
    }
}
```
Далее производится анализ полученной информации, начиная с последней команды трёхадресного кода. Определение переменной можно удалить, если
1. список её использований пуст
2. если эта переменная не является временной (появившейся в результате создания трёхадресного кода), то это не должно быть её последним определением в ББл (т.к. эта переменная может использоваться в следующих блоках)

```csharp
for (int i = commands.Count - 1; i >= 0; --i)
{
    // для текущей команды находим её индекс в списке DefList 
    var c = commands[i];
    var curDefInd = DefList.FindIndex(x => x.OrderNum == i);
    // а также находим индекс её последнего определения в ББл
    var lastDefInd = DefList.FindLastIndex(x => x.Id == c.Result);
    
    // если для текущей переменной существует определение в ББл,
    // проверяем, можно ли удалить команду
    if (curDefInd != -1 && DefList[curDefInd].Uses.Count == 0
            && (c.Result[0] != '#' ? curDefInd != lastDefInd : true))
    {
        // при удалении команды переменные в её правой части 
        // удаляются из соответствующих списков Uses
        DeleteUse(commands[i].Argument1, i);
        DeleteUse(commands[i].Argument2, i);
        // вместо удаленной команды добавляется пустой оператор noop
        result.Add(new Instruction(commands[i].Label, "noop", null, null, null));
    }
    // если удалять не нужно, добавляем команду в результирующий список команд
    else result.Add(commands[i]);
}
```

### Место в общем проекте (Интеграция)
Удаление мёртвого кода является одной из оптимизаций, применяемых к трёхадресному коду:
```csharp
/* ThreeAddressCodeOptimizer.cs */
private static List<Optimization> BasicBlockOptimizations => new List<Optimization>()
{
    ThreeAddressCodeDefUse.DeleteDeadCode,
    /* ... */
};
private static List<Optimization> AllCodeOptimizations => new List<Optimization>
{ /* ... */ };

public static List<Instruction> OptimizeAll(List<Instruction> instructions) =>
    Optimize(instructions, BasicBlockOptimizations, AllCodeOptimizations);
    
/* Main.cs */
var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
parser.root.Visit(threeAddrCodeVisitor);
var threeAddressCode = threeAddrCodeVisitor.Instructions;
var optResult = ThreeAddressCodeOptimizer.OptimizeAll(threeAddressCode);
```

### Тесты
В тестах проверяется, что для заданного трёхадресного кода ББл оптимизация возвращает ожидаемый результат:
```csharp
[TestCase(@"
var a, b, x;
x = a;
x = b;
",
    ExpectedResult = new string[]
    {
        "noop",
        "x = b"
    },
    TestName = "VarAssignSimple")]

[TestCase(@"
var a, b, c;
a = 2;
b = a + 4;
c = a * b;
",
    ExpectedResult = new string[]
    {
        "a = 2",
        "#t1 = a + 4",
        "b = #t1",
        "#t2 = a * b",
        "c = #t2"
    },
    TestName = "NoDeadCode")]

[TestCase(@"
var a, b;
input(a);
input(a);
b = a + 1;
",
    ExpectedResult = new string[]
    {
        "noop",
        "input a",
        "#t1 = a + 1",
        "b = #t1"
    },
    TestName = "DeadInput")]

public IEnumerable<string> TestDefUse(string sourceCode) =>
    TestTACOptimization(sourceCode, ThreeAddressCodeDefUse.DeleteDeadCode);
```
