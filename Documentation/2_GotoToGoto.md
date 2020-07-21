## Устранение переходов к переходам

### Постановка задачи
Создать оптимизирующий модуль программы устраняющий переходы к переходам.

### Команда
К. Галицкий, А. Черкашин

### Зависимые и предшествующие задачи
Предшествующие задачи:

- Трехадресный код

### Теоретическая часть
В рамках этой задачи необходимо было реализовать оптимизацию устранения переходов к переходам. Если оператор goto ведет на метку, содержащую в goto переход на следующую метку, необходимо протянуть финальную метку до начального goto.
Были поставлены  следующие 3 случая задачи:

* 1 случай 
  
  До

  ```csharp
  goto L1;
  ...
  L1: goto L2;
  ```

  После
  
  ```csharp
  goto L2;
  ...
  L1: goto L2;
  ```

* 2 случай
  
  До

  ```csharp
  if (/*усл*/) goto L1;
  ...
  L1: goto L2;
  ```

  После

  ```csharp
  if (/*усл*/) goto L2;
  ...
  L1: goto L2;
  ```

* 3 случай
  Если есть ровно один переход к L1 и оператору с L1 предшествует безусловный переход
  
  До

  ```csharp
  goto L1;
  ...
  L1: if (/*усл*/) goto L2;
  L3:
  ```

  После

  ```csharp
  ...
  if (/*усл*/) goto L2;
  goto L3;
  ...
  L3:
  ```

### Практическая часть
Реализовали метод для удаления переходов к переходам и разделили его на 3 случая:

Простые goto (для случая 1)
```csharp
if (instr.Operation == "goto")
{
        tmpCommands = PropagateTransitions(instr.Argument1, tmpCommands);
}
```
Инструкции вида if(усл) goto (для случая 2)
```csharp
if (instr.Operation == "ifgoto" && instr.Label == "")
{
        tmpCommands = PropagateIfWithoutLabel(instr.Argument2, tmpCommands);
}
```
Инструкции вида l1: if(усл) goto (для случая 3)
```csharp
if (instr.Operation == "ifgoto" && instr.Label != "") // Инструкции вида l1: if(усл) goto (для случая 3)
{
        tmpCommands = PropagateIfWithLabel(instr, tmpCommands);
}
```
Реализовали три вспомогательные функции для каждого случая задачи.

- Вспомогательная функция реализованная для случая 1:

Если метка инструкции равна метке которую мы ищем, и на ней стоит опереция вида "goto" и метка слева не равна метке справа тогда необходимо для всех "goto" с искомой меткой протянуть необходимую метку.

- Вспомогательная функция реализованная для случая 2: 

Если метка инструкции равна метке которую мы ищем, и на ней стоит оперецаия вида "goto" и метка слева не равна метке справа, тогда для всех "ifgoto" с искомой меткой, протягиваем необходимую метку.

- Вспомогательная функция реализованная для случая 3:

Реализовали проверку на наличие только одного перехода по условию для случая 3. Находим "ifgoto" на которую ссылается оператор безусловного перехода, ставим на место оператора безусловного перехода оператор "ifgoto" без метки на него. На следующей строке вставляем оператор безусловного перехода на метку где прежде стоял "ifgoto". В случае, если следующая команда после оператора "ifgoto" содержала метку, то оператор "goto" будет ссылаться на нее, иначе генирируем временную метку, которую поместим на прежнее место оператора "ifgoto".

Результатом работы программы является пара значений, была ли применена оптимизация и список инструкций с примененной оптимизацией

```csharp
return (wasChanged, tmpcommands);
```

### Место в общем проекте (Интеграция)
Используется после создания трехадресного кода внутри общего оптимизатора под названием `ThreeAddressCodeOptimizer`.
```csharp
/* ThreeAddressCodeOptimizer.cs */
private static List<Optimization> BasicBlockOptimizations => new List<Optimization>()
{
    /* ... */
};
private static List<Optimization> AllCodeOptimizations => new List<Optimization>
{
  ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
 /* ... */
};

public static List<Instruction> OptimizeAll(List<Instruction> instructions) =>
    Optimize(instructions, BasicBlockOptimizations, AllCodeOptimizations);

/* Main.cs */
var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
parser.root.Visit(threeAddrCodeVisitor);
var threeAddressCode = threeAddrCodeVisitor.Instructions;
var optResult = ThreeAddressCodeOptimizer.OptimizeAll(threeAddressCode);
```

### Тесты
В тестах проверяется, что применение оптимизации устранения переходов к переходам к заданному трехадресному коду, возвращает ожидаемый результат:
```csharp
[TestCase(@"
var a, b;
1: goto 2;
2: goto 5;
3: goto 6;
4: a = 1;
5: goto 6;
6: a = b;
",
    true,
    ExpectedResult = new string[]
    {
        "1: goto 6",
        "2: goto 6",
        "5: goto 6",
        "6: a = b",
    },
    TestName = "MultiGoTo")]

[TestCase(@"
var a, b;
b = 5;
if(a > b)
    goto 6;
6: a = 4;
",
    ExpectedResult = new string[]
    {
        "b = 5",
        "#t1 = a > b",
        "if #t1 goto 6",
        "goto L2",
        "L1: goto 6",
        "L2: noop",
        "6: a = 4",
    },
    TestName = "GotoIfElseTACGen1")]

public IEnumerable<string> TestGotoToGoto(
    string sourceCode,
    bool unreachableCodeElimination = false) =>
    TestTACOptimization(
        sourceCode,
        allCodeOptimization: ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
        unreachableCodeElimination: unreachableCodeElimination);
```
