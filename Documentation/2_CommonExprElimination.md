### Оптимизация общих подвыражений
#### Постановка задачи
Реализовать оптимизацию по трёхадресному коду вида:

| До оптимизации | Общие подвыражения | Результат оптимизации |
|-|-|-|
| a = b + c  <br>b = a – d <br>c = b + c  <br>d = a - d   | a = *b + c* <br>***b =*** `a – d`<br>c = *b + c*<br>d = `a – d` | a = b + c  <br>`b` = a – d  <br>c = b + c  <br>d = `b` |

#### Команда
Д. Лутченко, М. Письменский

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево



#### Теоретическая часть
a = `b + c`  
`b =` a – d  
c = `b + c`  
d = a - d  

1. Было ли использовано в правой части
`b + c` раньше в этом блоке?
2. Если да, то ***в промежутке между этими
определениями менялось ли b или c?***  
`Да`  => не подлежит оптимизации

a = b + c  
b = `a - d`  
c = b + c  
d = `a - d`

1. Было ли использовано в правой части
`a - d` раньше в этом блоке?
2. Если да, то ***в промежутке между этими
определениями менялось ли a или d?***  
`Нет` => можно оптимизировать

a = b + c  
`b` = a – d  
c = b + c  
d = `b`

#### Практическая часть


Метод поддерживает проверку коммутативности, что позволяет выполнять оптимизации вида
```csharp
public static bool IsCommutative(Instruction instr)
{
    switch (instr.Operation)
    {
        case "OR":
        case "AND":
        case "EQUAL":
        case "NOTEQUAL":
        case "PLUS":
        case "MULT":
            return true;
    }
    return false;
}
```
Что позволяетв выполять оптимизации вида:  
| До оптимизации | Общие подвыражения | Результат оптимизации |
|-|-|-|
| a = b + c  <br>c = c + b   | a = `b + c`<br>c = `c + b` | `a` = b + c<br>c = `a` |

Ориентированнй граф связей подвыражений представлен следующим образом:
```csharp
var exprToResults = new StringToStrings();
var argToExprs = new StringToStrings();
var resultToExpr = new Dictionary<string, string>();
```
Где:  
- `exprToResults` связи выражений к результатам (один ко многим)
- `argToExprs` связи операнд к выражениям (один ко многим)
- `resultToExpr` связь результата с выражением (один к одному)

Для построения общего ключа для выражений с коммутативной операцией применяется сортировка операнд:
```
string uniqueExpr(Instruction instr) =>
	string.Format(IsCommutative(instr) && string.Compare(instr.Argument1, instr.Argument2) > 0 ?
		"{2}{1}{0}" : "{0}{1}{2}", instr.Argument1, instr.Operation, instr.Argument2);
```
Основной алгоритм представляет из себя цикл по входным инструкциям,  
на каждой итерации которого, происходят следующие действия для каждой инструкции:
- создание ключа по выражению
- если для выражения есть связь с результатом
	- то - выполняем оптимизацию
	- иначе - добавляем связи операнд к выражению
- обновлям связи результата и выражения
- если результат имеет связь с выражениями как операнд - удаляем все зависимые связи
```csharp
for (var i = 0; i < instructions.Count; ++i)
{
    var expr = uniqueExpr(instructions[i]);
    if (instructions[i].Operation != "assign" &&
    	exprToResults.TryGetValue(expr, out var results) &&
	results.Count != 0)
    {
        changed = true;
        newInstructions.Add(new Instruction(instructions[i].Label, 
						"assign", 
						results.First(), "", 
						instructions[i].Result));
    }
    else
    {
        newInstructions.Add(instructions[i].Copy());
        addLink(argToExprs, instructions[i].Argument1, expr);
        addLink(argToExprs, instructions[i].Argument2, expr);
    }

    if (resultToExpr.TryGetValue(instructions[i].Result, out var oldExpr))
    {
        if (exprToResults.ContainsKey(oldExpr))
        {
            exprToResults[oldExpr].Remove(instructions[i].Result);
        }
    }

    resultToExpr[instructions[i].Result] = expr;
    addLink(exprToResults, expr, instructions[i].Result);

    if (argToExprs.ContainsKey(instructions[i].Result))
    {
        foreach (var delExpr in argToExprs[instructions[i].Result])
        {
            if (exprToResults.ContainsKey(delExpr))
            {
                foreach (var res in exprToResults[delExpr])
                {
                    resultToExpr.Remove(res);
                }
            }
            exprToResults.Remove(delExpr);
        }
    }
}
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
  ThreeAddressCodeCommonExprElimination.CommonExprElimination,
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

#### Примеры работы
| До оптимизации | Общие подвыражения | Результат оптимизации |
|-|-|-|
| a = b + c  <br>c = c + b<br>с = c + b | a = `b + c`<br>***c =*** `c + b`<br>с = `c + b` | `a` = b + c  <br>c = `a`<br>с = c + b |
| a = x + y  <br>b = y + x<br>a = q + z<br>a = x + y | a = `x + y` <br>b = `y + x`<br>***a =*** q + z<br>a = `x + y` | `a` = x + y  <br>`b` =`a`<br>a = q + z<br>a = `b` |
| a = -x<br>b = -x | a = `-x`<br>b = `-x` | `a` = -x<br>b = `a`|
| a = x<br>b = x | a = x<br>b = x | a = x<br>b = x |

#### Тесты
###### Проверка несрабатывания оптимизации:
```csharp
[Test]
public void Test1()
{
    var TAC = GenTAC(@"
var a, b, c, d, e, f, g, h, k;
a = b + c;
c = a + g;
k = b + c;
");
    
    var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
    Assert.IsFalse(ok);
    var expected = new List<string>()
    {
        "#t1 = b + c",
        "a = #t1",
        "#t2 = a + g",
        "c = #t2",
        "#t3 = b + c",
        "k = #t3"
    };
    var actual = instructions.Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```
###### Проверка срабатывания оптимизации:
```csharp
[Test]
public void Test2()
{
    var TAC = GenTAC(@"
var a, b, c, d, e, f, g, h, k;
a = b + c;
k = b + c;
");

    var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
    Assert.IsTrue(ok);
    var expected = new List<string>()
    {
        "#t1 = b + c",
        "a = #t1",
        "#t2 = #t1",
        "k = #t2"
    };
    var actual = instructions.Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```
###### Корректность проверки коммутативности:
```csharp
[Test]
public void CommutativeOpTest()
{
    var TAC = GenTAC(@"
var a, b, c, k;
a = b + c;
k = c + b;
");

    var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
    Assert.IsTrue(ok);
    var expected = new List<string>()
    {
        "#t1 = b + c",
        "a = #t1",
        "#t2 = #t1",
        "k = #t2"
    };
    var actual = instructions.Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```
###### Корректность проверки некоммутативности:
```csharp
[Test]
public void NotCommutativeOpTest()
{
    var TAC = GenTAC(@"
var a, b, c, k;
a = b - c;
k = c - b;
");

    var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
    Assert.IsFalse(ok);
    var expected = new List<string>()
    {
        "#t1 = b - c",
        "a = #t1",
        "#t2 = c - b",
        "k = #t2"
    };
    var actual = instructions.Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```
###### Проверка сброса связи:
```csharp
[Test]
public void Test5()
{
    var TAC = GenTAC(@"
var a, b, c, k;
a = b * c;
b = b * c;
k = b * c;
");

    var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
    Assert.IsTrue(ok);
    var expected = new List<string>()
    {
        "#t1 = b * c",
        "a = #t1",
        "#t2 = #t1",
        "b = #t2",
        "#t3 = b * c",
        "k = #t3"
    };
    var actual = instructions.Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```
###### С унарными операциями:
```csharp
[Test]
public void UnarOp()
{
    var TAC = GenTAC(@"
var a, b, c, k;
a = -b;
k = -b;
");

    var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
    Assert.IsTrue(ok);
    var expected = new List<string>()
    {
        "#t1 = -b",
        "a = #t1",
        "#t2 = #t1",
        "k = #t2"
    };
    var actual = instructions.Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```
###### Без унарных операций:
```csharp
[Test]
public void NotUnarOp()
{
    var TAC = GenTAC(@"
var a, b, c, k;
a = b;
k = b;
");

    var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
    Assert.IsFalse(ok);
    var expected = new List<string>()
    {
        "a = b",
        "k = b"
    };
    var actual = instructions.Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}
```  
###### Константы:
```csharp
[Test]
public void Constants()
{
    var TAC = GenTAC(@"
var a, b, c, k;
a = 5;
k = 5;
");

    var (ok, instructions) = ThreeAddressCodeCommonExprElimination.CommonExprElimination(TAC);
    Assert.IsFalse(ok);
    var expected = new List<string>()
    {
        "a = 5",
        "k = 5"
    };
    var actual = instructions.Select(instruction => instruction.ToString());

    CollectionAssert.AreEqual(expected, actual);
}

```
