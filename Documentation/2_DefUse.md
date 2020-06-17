### Def-Use информация и удаление мертвого кода на ее основе 

#### Постановка задачи
Накопление Def-Use информации в пределах ББл и удаление мертвого кода на основе этой информации.

#### Команда
А. Татарова, Т. Шкуро

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* Трехадресный код
* Разбиение кода на базовые блоки

#### Теоретическая часть
В рамках этой задачи переменные делятся на два типа: **def** и **use**.
Def --- это определение переменной, т.е. этой переменной было присвоено значение в данном ББл.
Use --- это использование переменной, т.е. эта переменная использовалась в каком-либо выражении в данном ББл.
Например, в следующем выражении **a** будет являться def-переменной, а **b** и **c** - use-переменными:
```
a = b + c;
```
На основе трехадресного кода составляется список Def-Use: список **def**-переменных, где для каждой **def**-переменной есть список использований этой переменной, т.е. список **use**.
После формирования Def-Use информации по всему коду ББл производится удаление мертвого кода --- удаление определений тех переменных, список использования которых пуст. Для удаления мертвого кода список команд проходится снизу вверх, при удалении команды производится обновление информации Def-Use. 

#### Практическая часть
Первым шагом по полученным командам трехадресного кода составляется список Def-Use:
```csharp
public class Use
{
    public Def Parent { get; set; } // определение переменной
    public int OrderNum { get; set; } // номер команды в трехадресном коде
}

public class Def
{
    public List<Use> Uses { get; set; } // список использований переменной
    public int OrderNum { get; set; } // номер команды в трехадресном коде
    public string Id { get; set; } // идентификатор переменной
}
    
public static List<Def> DefList;

private static void FillLists(List<Instruction> commands)
{
    DefList = new List<Def>();
    for (int i = 0; i < commands.Count; ++i)
    {
        // если оператор является оператором присваивания, опертором 
        // ариметических или логических операций или оператором ввода,
        // добавляем в список DefList результат этой операции
        if (operations.Contains(commands[i].Operation))
            DefList.Add(new Def(i, commands[i].Result));
        // если в правой части оператора переменные,
        // и их определение есть в списке DefList,
        // добавляем их в соотвествующий список Uses
        AddUse(commands[i].Argument1, commands[i], i);
        AddUse(commands[i].Argument2, commands[i], i);
    }
}
```
Далее производится анализ полученной информации, начиная с последней команды трехадресного кода. Определение переменной можно удалить, если
1. список ее использований пуст
2. если эта переменная не является временной (появившейся в результате создания трехадресного кода), то это не должно быть ее последним определением в ББл (т.к. эта переменная может использоваться в следующих блоках)

```csharp
for (int i = commands.Count - 1; i >= 0; --i)
{
    // для текущей команды находим ее индекс в списке DefList 
    var c = commands[i];
    var curDefInd = DefList.FindIndex(x => x.OrderNum == i);
    // а также находим индекс ее последнего определения в ББл
    var lastDefInd = DefList.FindLastIndex(x => x.Id == c.Result);
    
    // если для текущей переменной существует определение в ББл,
    // проверяем, можно ли удалить команду
    if (curDefInd != -1 && DefList[curDefInd].Uses.Count == 0
            && (c.Result[0] != '#' ? curDefInd != lastDefInd : true))
    {
        // при удалении команды переменные в ее правой части 
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

#### Место в общем проекте (Интеграция)
Удаление мертвого кода является одной из оптимизаций, применяемых к трехадресному коду:
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

#### Тесты
В тестах проверяется, что для заданного трехадресного кода ББл оптимизация возвращает ожидаемый результат:
```csharp
[Test]
public void VarAssignSimple()
{
    var TAC = GenTAC(@"
    var a, b, x;
    x = a;
    x = b;
    ");
    var optimizations = new List<Optimization> { 
        ThreeAddressCodeDefUse.DeleteDeadCode 
    };
    var expected = new List<string>() 
    {
        "noop",
        "x = b"
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
        .Select(instruction => instruction.ToString());
    CollectionAssert.AreEqual(expected, actual);
}

[Test]
public void NoDeadCode()
{
    var TAC = GenTAC(@"
    var a, b, c;
    a = 2;
    b = a + 4;
    c = a * b;
    ");
    var optimizations = new List<Optimization> { 
        ThreeAddressCodeDefUse.DeleteDeadCode
    };
    var expected = new List<string>()
    {
        "a = 2",
        "#t1 = a + 4",
        "b = #t1",
        "#t2 = a * b",
        "c = #t2"
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
        .Select(instruction => instruction.ToString());
    CollectionAssert.AreEqual(expected, actual);
}

[Test]
public void DeadInput()
{
    var TAC = GenTAC(@"
    var a, b;
    input(a);
    input(a);
    b = a + 1;
    ");
    var optimizations = new List<Optimization> { 
        ThreeAddressCodeDefUse.DeleteDeadCode
    };
    var expected = new List<string>()
    {
        "noop",
        "input a",
        "#t1 = a + 1",
        "b = #t1"
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
        .Select(instruction => instruction.ToString());
    CollectionAssert.AreEqual(expected, actual);
}
```