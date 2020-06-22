## AST-оптимизация замены оператора сравнения двух значений на его булево значение

### Постановка задачи
Реализовать оптимизацию по AST дереву вида false == false -> true, 5 == 6 -> false

### Команда
А. Пацеев, И. Ушаков

### Зависимые и предшествующие задачи
Предшествующие задачи:
* Построение AST дерева

### Теоретическая часть
Реализовать оптимизацию по AST дереву вида false == false -> true, 5 == 6 -> false
  * До
  ```csharp
  5 == 5
  5 == 6
  false == false
  true == false
  ```
  * После
  ```csharp
  true
  false
  true
  false
  ```

### Практическая часть
Эта оптимизация представляет собой визитор, унаследованный от `ChangeVisitor`. Пример реализации метода:

```csharp
public class OptExprEqualBoolNum : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        if (n is BinOpNode binop && binop.Op == OpType.EQUAL)
        {
            if (binop.Left is IntNumNode intValLeft && binop.Right is IntNumNode intValRight)
            {
                ReplaceExpr(binop, new BoolValNode(intValLeft.Num == intValRight.Num));
            }
            else if (binop.Left is BoolValNode boolValLeft && binop.Right is BoolValNode boolValRight)
            {
                ReplaceExpr(binop, new BoolValNode(boolValLeft.Val == boolValRight.Val));
            }
        }
    }
}
```

### Место в общем проекте (Интеграция)
```csharp
private static IReadOnlyList<ChangeVisitor> ASTOptimizations { get; } = new List<ChangeVisitor>
{
    /* ... */
    new OptExprEqualBoolNum(),
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
```

### Тесты
```csharp
[Test]
public void SumNumTest()
{
    var AST = BuildAST(@"
var b, c, d;
b = true == true;
while (5 == 5)
    c = true == false;
d = 7 == 8;");
    var expected = new[] {
        "var b, c, d;",
        "b = true;",
        "while true",
        "  c = false;",
        "d = false;"
    };

    var result = ApplyOpt(AST, new OptExprEqualBoolNum());
    CollectionAssert.AreEqual(expected, result);
}
```
