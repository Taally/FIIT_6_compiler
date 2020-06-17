### Устранение переходов к переходам

#### Постановка задачи
Создать оптимизирующий модуль программы устраняющий переходы к переходам.

#### Команда
К. Галицкий, А. Черкашин

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* Трехадресный код

#### Теоретическая часть
В рамках этой задачи необходимо было реализовать оптимизацию устранения переходов к переходам. Если оператор goto ведет на метку, содержащую в goto переход на следующую метку, необходимо протянуть финальную метку до начального goto.
Были поставлены  следующие задачи:
* До
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
* До
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
* До
  ```csharp
  goto L1;
  ...
  L1: if (/*усл*/) goto L2;
  L3:
  ```
  После
  ```csharp
  ...
  L1: if (/*усл*/) goto L2;
  goto L3;
  ...
  L3:
  ```

#### Практическая часть
Реализовали структуру:
```csharp
public struct GtotScaner
        {
            public int index;
            public string label;
            public string labelfrom;

            public GtotScaner(int index, string label, string labelfrom)
            {
                this.index = index;
                this.label = label;
                this.labelfrom = labelfrom;
            }
        }
```
Номер команды в трёхадресном коде, для обеспечения поиска сложности О(1)
```csharp
public int index;
```
Метка в трехадресном коде на которой стоит goto или ifgoto
```csharp
public string label;
```
Метка на которую существует goto стоящий в трехадресном коде предыдущей метки вида
L1: goto L2;
где L1 - label
    L2 - labelfrom
```csharp
public string labelfrom;
```

Примеры реализации метода:

```csharp
        public static Tuple<bool, List<Instruction>> ReplaceGotoToGoto(List<Instruction> commands)
        {
            bool changed = false; // флаг, проведенной оптимизации
            List<GtotScaner> list = new List<GtotScaner>();  // Список всех переходов и их меток
            List<Instruction> tmpcommands = new List<Instruction>();  // Трехадресный код
```

Заполнение  списка переходов:
```csharp
            for (int i = 0; i < commands.Count; i++)
            {
                tmpcommands.Add(commands[i]);
                if (commands[i].Operation == "goto")  // Добавление в список если команда вида goto
                {
                    list.Add(new GtotScaner(i, commands[i].Label, commands[i].Argument1));  // Добавление номера (строки, метки, метки перехода)
                }

                if (commands[i].Operation == "ifgoto")  // Добавление в список если команда вида if()goto
                {
                    list.Add(new GtotScaner(i, commands[i].Label, commands[i].Argument2));  // Добавление номера (строки, метки, метки перехода)
                }
            }
```

Поиск по списку переходов и применение оптимизации:
```csharp
            for (int i = 0; i < tmpcommands.Count; i++)
            {

                if (tmpcommands[i].Operation == "goto")  // Если операция goto
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].label == tmpcommands[i].Argument1)  //  Если левая метка совпадает с меткой команды
                        {
                            if (tmpcommands[i].Argument1.ToString() == list[j].labelfrom.ToString())  // Если правая метка совпадает
                            {
                                changed |= false;  //  Изменений проведено не было
                            }
                            else
                            {
                                changed |= true; //  Изменения были проведены
                                tmpcommands[i] = new Instruction(tmpcommands[i].Label, "goto", list[j].labelfrom.ToString(), "", "");  // Меняем инструкцию, изменяя в ней правую часть на необходимую нам метку
                            }

                        }
                    }
                }

                if (tmpcommands[i].Operation == "ifgoto")  // Если операция if()goto
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].label == tmpcommands[i].Argument2) //  Если левая метка совпадает с меткой команды
                        {

                            if (tmpcommands[i].Argument2.ToString() == list[j].labelfrom.ToString()) // Если правая метка совпадает
                            {
                                changed |= false; //  Изменений проведено не было
                            }
                            else
                            {
                                tmpcommands[i] = new Instruction(tmpcommands[i].Label, "ifgoto", tmpcommands[i].Argument1, list[j].labelfrom.ToString(), ""); // Меняем инструкцию, изменяя в ней правую часть на необходимую нам метку
                                changed |= true; //  Изменения были проведены
                            }

                        }
                    }
                }
            }
```
Результатом работы программы является пара значений, была ли применена оптимизация и список инструкций с примененной оптимизацией
```csharp
    return Tuple.Create(changed, tmpcommands);
```

#### Место в общем проекте (Интеграция)
Используется после создания трехадресного кода:
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

#### Тесты
В тестах проверяется, что применение оптимизации устранения переходов к переходам к заданному трехадресному коду, возвращает ожидаемый результат:
```csharp
[Test]
public void Test1()
{
    var TAC = GenTAC(@"
    var a, b;
    1: goto 2;
    2: goto 5;
    3: goto 6;
    4: a = 1;
    5: goto 6;
    6: a = b;
    ");
    var optimizations = new List<Optimization> { ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto };

    var expected = new List<string>()
    {
        "1: goto 6",
        "2: goto 6",
        "3: goto 6",
        "4: a = 1",
        "5: goto 6",
        "6: a = b",
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
        .Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}

[Test]
public void TestGotoIfElseTACGen1()
{
    var TAC = GenTAC(@"
    var a,b;
    b = 5;
    if(a > b)
	    goto 6;
    6: a = 4;
    ");
    var optimizations = new List<Optimization> { ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto };

    var expected = new List<string>()
    {
        "b = 5",
        "#t1 = a > b",
        "if #t1 goto 6",
        "goto L2",
        "L1: goto 6",
        "L2: noop",
        "6: a = 4",
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
        .Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}

[Test]
public void Test3()
{
    var TAC = GenTAC(@"
    var a;
    goto 1;
    1: goto 2;
    2: goto 3;
    3: goto 4;
    4: a = 4;
    ");
    var optimizations = new List<Optimization> { ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto };

    var expected = new List<string>()
    {
        "goto 4",
        "1: goto 4",
        "2: goto 4",
        "3: goto 4",
        "4: a = 4",
    };
    var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
        .Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```
