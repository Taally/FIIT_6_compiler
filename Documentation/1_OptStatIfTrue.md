### AST-оптимизация замены if(true) на его true ветку

#### Постановка задачи
Реализовать оптимизацию по AST дереву вида if(true) st1 else st2 => st1

#### Команда
А. Татарова, Т. Шкуро, Д. Володин, Н. Моздоров

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево

#### Теоретическая часть
Реализовать оптимизацию по AST дереву вида if(true) st1 else st2 => st1
  * До
  ```csharp
  if(true)
    st1;
  else
    st2;
  ```
  * После
  ```csharp
  st1;
  ```

#### Практическая часть
Примеры реализации метода:

```csharp
    if (n is IfElseNode ifNode)  // Если это корень if
        if (ifNode.Expr is BoolValNode boolNode && boolNode.Val) // Если выражение == true
        {
            if (ifNode.TrueStat != null)
            {
                ifNode.TrueStat.Visit(this);
            }
            ReplaceStat(ifNode, ifNode.TrueStat);
        }
```

#### Место в общем проекте (Интеграция)
```csharp
public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
       {
             /* ... */
           new OptStatIftrue(),
             /* ... */
       };

       public static void Optimize(Parser parser)
       {
           int optInd = 0;
           do
           {
               parser.root.Visit(Optimizations[optInd]);
               if (Optimizations[optInd].Changed)
                   optInd = 0;
               else
                   ++optInd;
           } while (optInd < Optimizations.Count);
       }
```

#### Тесты
В тестах проверяется, что данная оптимизация на месте условного оператора ```if(true)``` оставляет только true-ветку
```[Test]
public void IfTrueComplexTest()
{
    var AST = BuildAST(@"
var a, b;
if true
if true {
a = b;
b = 1;
}
else
a = 1;

if a > b{
a = b;
if true{
b = b + 1;
b = b / 5;
}
}
");

    var expected = new[] {
        "var a, b;",
        "a = b;",
        "b = 1;",
        "if (a > b) {",
        "  a = b;",
        "  b = (b + 1);",
        "  b = (b / 5);",
        "}"
    };

    var result = ApplyOpt(AST, new OptStatIfTrue());
    CollectionAssert.AreEqual(expected, result);
}
```
