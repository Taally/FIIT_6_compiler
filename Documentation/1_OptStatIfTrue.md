## AST-оптимизация замены if (true) на его true ветку

### Постановка задачи

Реализовать оптимизацию по AST дереву вида if (true) st1 else st2 => st1

### Команда
А. Татарова, Т. Шкуро, Д. Володин, Н. Моздоров

### Зависимые и предшествующие задачи

Предшествующие:

- Построение AST-дерева
- Базовые визиторы
- ChangeVisitor

### Теоретическая часть

Реализовать оптимизацию по AST дереву вида if (true) st1 else st2 => st1

  * До

  ```csharp
  if (true)
    st1;
  else
    st2;
  ```

  * После

  ```csharp
  st1;
  ```

### Практическая часть

Примеры реализации метода:

```csharp
public class OptStatIfTrue : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        // if (true) st1; else st2
        if (n is IfElseNode ifNode && ifNode.Expr is BoolValNode boolNode && boolNode.Val)
        {
            ReplaceStat(ifNode, ifNode.TrueStat);
        }
    }
}
```

### Место в общем проекте (Интеграция)

```csharp
public static class ASTOptimizer
{
    private static IReadOnlyList<ChangeVisitor> ASTOptimizations { get; } = new List<ChangeVisitor>
    {
        /* ... */
        new OptStatIfTrue(),
        /* ... */
    };

    public static void Optimize(Parser parser, IReadOnlyList<ChangeVisitor> Optimizations = null)
    {
        Optimizations = Optimizations ?? ASTOptimizations;
        var optInd = 0;
        do
        {
            parser.root.Visit(Optimizations[optInd]);
            if (Optimizations[optInd].Changed)
            {
                optInd = 0;
            }
            else
            {
                ++optInd;
            }
        } while (optInd < Optimizations.Count);
    }
}
```

### Тесты

В тестах проверяется, что данная оптимизация на месте условного оператора `if (true)` оставляет только true-ветку
```csharp
[TestCase(@"
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
",
    ExpectedResult = new[]
    {
        "var a, b;",
        "a = b;",
        "b = 1;",
        "if (a > b) {",
        "    a = b;",
        "    b = (b + 1);",
        "    b = (b / 5);",
        "}"
    },
    TestName = "IfTrueComplex")]

public string[] TestOptStatIfTrue(string sourceCode) =>
    TestASTOptimization(sourceCode, new OptStatIfTrue());
```
