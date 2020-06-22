### Удаление алгебраических тождеств

#### Постановка задачи
Создать оптимизирующий модуль программы для устранения вычислений относительно следующих алгебраических тождеств:
- a = b - b => a = 0
- a = 0 / b => a = 0
- a = b * 0 => a = 0
- a = b * 1 => a = b
- a = b + 0 => a = b
- a = b - 0 => a = b
- a = b / 1 => a = b
- a = b / b => a = 1

#### Команда
С. Рыженков, А.Евсеенко

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* Трехадресный код
* Разбиение кода на базовые блоки

#### Теоретическая часть
Данная задача относится к локальной оптимизации внутри каждого блока. Задача заключается в поиске алгебраических выражений для устранения вычислений из базового блока. 
Примеры работы до применения оптимизации и после: 
1) ```a = b * 1;``` => ```a = b;```
2) ```a = b + 0;``` => ```a = b;```
3) ```a = b - 0;``` => ```a = b;```
4) ```a = b / 1;``` => ```a = b;```
5) ```a = b / b;``` => ```a = 1;```
6) ```a = b - b;``` => ```a = 0;```
7) ```a = 0 / b;``` => ```a = 0;```
8) ```a = b * 0;``` => ```a = 0;```

Для случаев 1, 2, 8 оптимизация работает и в случае инверсии мест аргументов.
#### Практическая часть
Статический метод ```RemoveAlgebraicIdentities(List<Instruction> commands)``` принимает список инструкций. 
Список ```var result = new List<Instruction>();``` служит для накапливания инструкций (упрощенных и исходных). Т.к. данная задача относится только к алгебраическим тождествам, то в данном методе также присутствует переменная: ```variablesAreNotBool```, значение которой равно - ```!bool.TryParse(commands[i].Argument1, out _) && !bool.TryParse(commands[i].Argument2, out _)``` - true в случае если оба аргумента - числа.
Переменная ```var wasChanged = false;``` нужна для фиксации изменения инструкции.
- Разность равных аргументов: ``` a = b - b```
```csharp
if (variablesAreNotBool && commands[i].Argument1 == commands[i].Argument2 && commands[i].Operation == "MINUS")
{
    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
    wasChanged = true;
    continue;
}
```
- Значение делимого равно нулю: ``` a = 0 / b ```
```csharp
if (commands[i].Operation == "DIV" && variablesAreNotBool && arg1IsNumber && arg1 == 0 && (arg2IsNumber && arg2 != 0 || !arg2IsNumber))
{
    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
    wasChanged = true;
    continue;
}
```
- Умножение на ноль: ```a = 0 * b```
```csharp
if (commands[i].Operation == "MULT" && variablesAreNotBool && (arg1IsNumber && arg1 == 0 
|| arg2IsNumber && arg2 == 0))
{
    result.Add(new Instruction(commands[i].Label, "assign", "0", "", commands[i].Result));
    wasChanged = true;
    continue;
}
```
- Умножение на единицу: ```a = 1 * b```
```csharp
var arg1IsNumber = double.TryParse(commands[i].Argument1, out var arg1);
if (commands[i].Operation == "MULT" && variablesAreNotBool && arg1IsNumber && arg1 == 1)
{
    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument2, "", commands[i].Result));
    wasChanged = true;
    continue;
}
var arg2IsNumber = double.TryParse(commands[i].Argument2, out var arg2);
if (commands[i].Operation == "MULT" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
{
    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
    wasChanged = true;
    continue;
}
```
- Суммирование и вычитание с нулем: ```a = b + 0``` и ```a = b - 0```
```csharp
if ((commands[i].Operation == "PLUS" || commands[i].Operation == "MINUS") && variablesAreNotBool && arg1IsNumber && arg1 == 0)
{
    var sign = commands[i].Operation == "PLUS" ? "" : "-";
    result.Add(new Instruction(commands[i].Label, "assign", sign + commands[i].Argument2, "", commands[i].Result));
    wasChanged = true;
    continue;
}
if ((commands[i].Operation == "PLUS" || commands[i].Operation == "MINUS") && variablesAreNotBool && arg2IsNumber && arg2 == 0)
{
    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
    wasChanged = true;
    continue;
}
```
- Деление на единицу: ```a = b / 1```
```csharp
if (commands[i].Operation == "DIV" && variablesAreNotBool && arg2IsNumber && arg2 == 1)
{
    result.Add(new Instruction(commands[i].Label, "assign", commands[i].Argument1, "", commands[i].Result));
    wasChanged = true;
    continue;
}
```
- Деление равных аргументов: ```a = b / b```
```csharp
if (commands[i].Operation == "DIV" && variablesAreNotBool && arg1 == arg2)
{
    result.Add(new Instruction(commands[i].Label, "assign", "1", "", commands[i].Result));
    wasChanged = true;
    continue;
}
```
Результат работы - кортеж, где первое значение - логическая переменная, отвечающая за фиксацию применения оптимизации, а вторая - список инструкций:
```csharp
return (wasChanged, result);
```
#### Место в общем проекте (Интеграция)
Используется после создания трехадресного кода: 
```csharp
//ThreeAddressCodeOptimizer.cs
private static List<Optimization> BasicBlockOptimizations => new List<Optimization>()
{
    //...
    ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities,
    //...
};
private static List<Optimization> AllCodeOptimizations => new List<Optimization>
{
    //...
};
//Main.cs
var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
parser.root.Visit(threeAddrCodeVisitor);
var threeAddressCode = threeAddrCodeVisitor.Instructions;
var optResult = ThreeAddressCodeOptimizer.OptimizeAll(threeAddressCode);
```

#### Тесты
```csharp
[Test]
public void ComplexTest()
{
    var TAC = GenTAC(@"var a, b;
b = a - a;
b = a * 0;
b = 0 * a;
b = 1 * a;
b = a * 1;
b = a / 1;
b = a + 0;
b = 0 + a;
b = a - 0;
b = 0 - a;
b = b / b;
");
    var expected = new List<string>() 
    {
        "#t1 = 0", "b = #t1", "#t2 = 0", "b = #t2", "#t3 = 0", "b = #t3", "#t4 = a", "b = #t4",
        "#t5 = a", "b = #t5", "#t6 = a", "b = #t6", "#t7 = a", "b = #t7", "#t8 = a", "b = #t8",
        "#t9 = a", "b = #t9", "#t10 = -a", "b = #t10", "#t11 = 1", "b = #t11"
    };
    var optimizations = new List<Func<List<Instruction>, 
    (bool, List<Instruction>)>>
    {
        ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
        .Select(instruction => instruction.ToString());
    CollectionAssert.AreEqual(expected, actual);
}
```



