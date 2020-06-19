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

#### Практическая часть
Реализовали метод для удаления переходов к переходам и разделили его на 3 случая:
```csharp
wasChanged = false;
var tmpCommands = new List<Instruction>();
tmpCommands.AddRange(commands.ToArray()); // Перепишем набор наших инструкций в темповый массив

foreach (var instr in commands)
{
	if (instr.Operation == "goto") // Простые goto (для случая 1)
	{
		tmpCommands = StretchTransitions(instr.Argument1, tmpCommands);
	}

	if (instr.Operation == "ifgoto" && instr.Label == "") // Инструкции вида if(усл) goto (для случая 2)
	{
		tmpCommands = StretchIFWithoutLabel(instr.Argument2, tmpCommands);
	}

	if (instr.Operation == "ifgoto" && instr.Label != "") // Инструкции вида l1: if(усл) goto (для случая 3)
	{
		tmpCommands = StretchIFWithLabel(instr, tmpCommands);
	}
}
return (wasChanged, tmpCommands);
```
Реализовали три вспомогательные функции для каждого случая задачи.
Вспомогательная функция для случая 1:
```csharp
/// <summary>
/// Протягивает метки для goto
/// </summary>
/// <param name="Label">Метка которую мы ищем</param>
/// <param name="instructions">Набор наших инструкций</param>
/// <returns>
/// Вернет измененные инструкции с протянутыми goto
/// </returns>
private static List<Instruction> StretchTransitions(string Label, List<Instruction> instructions)
{
	for (int i = 0; i < instructions.Count; i++)
	{
		// Если метка инструкции равна метке которую мы ищем, и на ней стоит оперецаия вида "goto" и метка слева не равна метке справа
		if (instructions[i].Label == Label 
                && instructions[i].Operation == "goto" 
                && instructions[i].Argument1 != Label)
		{
			string tmp = instructions[i].Argument1;
			for (int j = 0; j < instructions.Count; j++)
			{
				//Для всех "goto" с искомой меткой, протягиваем нужный нам Label
				if (instructions[j].Operation == "goto" && instructions[j].Argument1 == Label)
				{
					wasChanged = true;
					instructions[j] = new Instruction(instructions[j].Label, "goto", tmp, "", "");
				}
			}
		}
	}
	return instructions;
}
```
Вспомогательная функция для случая 2:
```csharp
/// <summary>
/// Протягивает метки для if(усл) goto
/// </summary>
/// <param name="Label">Метка которую мы ищем</param>
/// <param name="instructions">Набор наших инструкций</param>
/// <returns>
/// Вернет измененные инструкции с протянутыми goto из if
/// </returns>
private static List<Instruction> StretchIFWithoutLabel(string Label, List<Instruction> instructions)
{
	for (int i = 0; i < instructions.Count; i++)
	{
		// Если метка инструкции равна метке которую мы ищем, и на ней стоит оперецаия вида "goto" и метка слева не равна метке справа
		if (instructions[i].Label == Label 
                && instructions[i].Operation == "goto" 
                && instructions[i].Argument2 != Label)
		{
			string tmp = instructions[i].Argument1;
			for (int j = 0; j < instructions.Count; j++)
			{
				//Для всех "ifgoto" с искомой меткой, протягиваем нужный нам Label
				if (instructions[j].Operation == "ifgoto" && instructions[j].Argument2 == Label)
				{
					wasChanged = true;
					instructions[j] = new Instruction("", "ifgoto", instructions[j].Argument1, tmp, "");
				}
			}
		}
	}
	return instructions;
}
```

Вспомогательная функция для случая 3:
```csharp
private static List<Instruction> StretchIFWithLabel(Instruction findInstruction, List<Instruction> instructions)
{
	int findIndexIf = instructions.IndexOf(findInstruction); //Поиск индекса "ifgoto" на которую существует метка
	
	//проверка на наличие индекса. Проверка на наличие только одного перехода по условию для случая 3
	if (findIndexIf == -1
		|| instructions.Where(x => instructions[findIndexIf].Label == x.Argument1 
                && x.Operation == "goto" 
                && x.ToString() != instructions[findIndexIf].ToString()).Count() > 1)
	{
		return instructions;
	}
	//поиск индекса перехода на требуемый "ifgoto"
	int findIndexGoto = instructions.IndexOf(instructions.Where(x => instructions[findIndexIf].Label == x.Argument1 
                                                                                && x.Operation == "goto").ElementAt(0));

	wasChanged = true;
	
	//Если следущая команда после "ifgoto" не содержит метку
	if (instructions[findIndexIf + 1].Label == "")
	{
		instructions[findIndexGoto] = new Instruction("",
                instructions[findIndexIf].Operation,
                instructions[findIndexIf].Argument1,
                instructions[findIndexIf].Argument2,
                instructions[findIndexIf].Result);
		var tmp = ThreeAddressCodeTmp.GenTmpLabel();
		instructions[findIndexIf] = new Instruction(tmp, "noop", "", "", "");
		instructions.Insert(findIndexGoto + 1, new Instruction("", "goto", tmp, "", ""));
	}
	else //Если следущая команда после "ifgoto" содержит метку
	{
		instructions[findIndexGoto] = new Instruction("",
                instructions[findIndexIf].Operation,
                instructions[findIndexIf].Argument1,
                instructions[findIndexIf].Argument2,
                instructions[findIndexIf].Result);
		var tmp = instructions[findIndexIf + 1].Label;
		instructions[findIndexIf] = new Instruction("", "noop", "", "", "");
		instructions.Insert(findIndexGoto + 1, new Instruction("", "goto", tmp, "", ""));
	}
	return instructions;
}
```

Результатом работы программы является пара значений, была ли применена оптимизация и список инструкций с примененной оптимизацией
```csharp
    return (wasChanged, tmpcommands);
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
public void MultiGoToTest()
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
public void GoToLabelTest()
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
