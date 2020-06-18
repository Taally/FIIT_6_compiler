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
* 1 До
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
* 2 До
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
* 3 До
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

#### Практическая часть
Реализовали структуру:
```csharp
public struct GtotScaner
        {
            public string Label { get; }
            public string LabelFrom { get; }
            public string Operation { get; }
            public int InstructionNum { get; }
            public GtotScaner(string label, string labelFrom, string operation, int instructionNum)
            {
                Label = label;
                LabelFrom = labelFrom;
                Operation = operation;
                InstructionNum = instructionNum;
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
            var wasChanged = false; // флаг, проведенной оптимизации
            List<GtotScaner> list = new List<GtotScaner>();  // Список всех переходов и их меток
            List<Instruction> tmpcommands = new List<Instruction>();  // Трехадресный код
```

Заполнение  списка переходов:
```csharp
            for (var i = 0; i < commands.Count; i++)
            {
                if (commands[i].Operation == "goto") // Если операция вида goto
                {
                    // Заполнение списка переходов в виде (метка инструкции, метка перехода, операция, номер команды)
                    list.Add(new GtotScaner(commands[i].Label, commands[i].Argument1, commands[i].Operation, i)); 
                }

                if (commands[i].Operation == "ifgoto") // Если операция вида if(усл) goto
                {
                    // Заполнение списка переходов в виде (метка инструкции, метка перехода, операция, номер команды)
                    list.Add(new GtotScaner(commands[i].Label, commands[i].Argument2, commands[i].Operation, i));
                }
            }
```

Поиск по списку переходов и применение оптимизации:
```csharp
            var addNewLabels = new Dictionary<int, string>();   // Словарь вида (номер строки, необходимая метка)
            var shiftNewLabels = 0;

            for (var i = 0; i < commands.Count; i++)
            {
                if (commands[i].Operation == "goto")   // Если операция вида goto
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                    
                        if (list[j].Label == commands[i].Argument1       // Если на метку есть переход
                        && list[j].LabelFrom != commands[i].Argument1    // Метка не на себя
                        && list[j].Operation == "goto")                  //  Тип операции goto
                        {
                            wasChanged = true;
                            tmpCommands.Add(new Instruction(commands[i].Label, "goto", list[j].LabelFrom, "", ""));
                            i++;
                            break;
                        }
                        else if (list[j].Label == commands[i].Argument1 // Если на метку есть переход
                        && list[j].LabelFrom != commands[i].Argument1   // Метка не на себя
                        && list[j].Operation == "ifgoto"                //  Тип операции ifgoto
                        && CountGoTo(list, list[j].Label) <= 1)         // условие работы для задания типа 3
                        {
                            shiftNewLabels++;
                            wasChanged = true;
                            tmpCommands.Add(new Instruction("",
                                commands[list[j].InstructionNum].Operation,
                                commands[list[j].InstructionNum].Argument1,
                                commands[list[j].InstructionNum].Argument2,
                                commands[list[j].InstructionNum].Result));
                            // Если на следующей операции нет метки, вставим необходимую метку
                            if (commands[list[j].InstructionNum + 1].Label == "")
                            {
                                var tmpName = ThreeAddressCodeTmp.GenTmpLabel();
                                tmpCommands.Add(new Instruction("", "goto", tmpName, "", ""));
                                addNewLabels.Add(list[j].InstructionNum + shiftNewLabels, tmpName);
                                i += 1;
                                break;
                            }
                        }
                    }
                }
                // Если это операция ifgoto и не подлежит удалению
                else if (commands[i].Operation == "ifgoto" && !addNewLabels.ContainsKey(i)) 
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[j].Label == commands[i].Argument2)
                        {
                            if (list[j].Label == commands[i].Argument2 && list[j].LabelFrom != commands[i].Argument2)
                            {
                                wasChanged = true;
                                tmpCommands.Add(new Instruction(commands[i].Label, "ifgoto", commands[i].Argument1, list[j].LabelFrom, ""));
                                i++;
                                break;
                            }
                        }
                    }
                }
                tmpCommands.Add(new Instruction(commands[i].Label, commands[i].Operation, commands[i].Argument1, commands[i].Argument2, commands[i].Result));
            }

            foreach (var x in addNewLabels.Keys) // Удаление if вида 3
            {
                tmpCommands[x] = new Instruction(addNewLabels[x], "noop", "", "", "");
            }
```

Вспомогательная функция для реализации части 3
```csharp
    public static int CountGoTo(List<GtotScaner> a, string label)
        {
            var tmpCount = 0;
            foreach (var x in a)
            {
                if (x.LabelFrom == label && (x.Operation == "goto" || x.Operation == "ifgoto"))
                {
                    tmpCount++;
                }
            }
            return tmpCount;
        }
```

Результатом работы программы является пара значений, была ли применена оптимизация и список инструкций с примененной оптимизацией
```csharp
    return Tuple.Create(wasChanged, tmpcommands);
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
