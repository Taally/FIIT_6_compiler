
## AST-оптимизация неравенства констант

### Постановка задачи
Реализовать оптимизацию по AST дереву вида:
| До оптимизации | Результат оптимизации |
|-|-|
| c = `3 < 4`  <br>c = `3 < 2`  <br>c = `3 > 2` <br>c = `3 > 4` <br>c = `3 >= 4` <br>c = `3 >= 2` | c = `true`<br>c = `false` <br>c = `true`<br>c = `false`<br>c = `false`<br>c = `true`|

### Команда
Д. Лутченко, М. Письменский

### Зависимые и предшествующие задачи
Предшествующие задачи:

* Построение AST дерева

### Теоретическая часть
Реализовать оптимизацию по AST дереву вида 3 < 4 -> true, 3 < 2 -> false и т.п.

- До оптимизации
```csharp
c = 3 < 4;
b = 2 >= 5;
```
- После оптимизации
```csharp
c = true;
b = false;
```
Подход к решению задачи был следующий: если при обходе AST-дерева встречаем узел BinOpNode, то проверяем его левый и правый операнды и операцию в BinOpNode. Если узлы - константы, то проверяем неравенство с приведенной операцией.

### Практическая часть
Эта оптимизация представляет собой визитор, унаследованный от ChangeVisitor. Пример реализации метода:

```csharp
public class OptExprWithOperationsBetweenConsts : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        if (n is BinOpNode binop)
        {
            if (binop.Left is IntNumNode lbn && binop.Right is IntNumNode rbn)
            {
                switch (binop.Op)
                {
                    case OpType.LESS:
                        ReplaceExpr(binop, new BoolValNode(lbn.Num < rbn.Num));
                        break;

                    case OpType.GREATER:
                        ReplaceExpr(binop, new BoolValNode(lbn.Num > rbn.Num));
                        break;

                    case OpType.EQGREATER:
                        ReplaceExpr(binop, new BoolValNode(lbn.Num >= rbn.Num));
                        break;

                    case OpType.EQLESS:
                        ReplaceExpr(binop, new BoolValNode(lbn.Num <= rbn.Num));
                        break;
                    case OpType.NOTEQUAL:
                        ReplaceExpr(binop, new BoolValNode(lbn.Num != rbn.Num));
                        break;
                }
            }
            else
            if (binop.Left is BoolValNode left && binop.Right is BoolValNode right
                && binop.Op == OpType.NOTEQUAL)
            {
                ReplaceExpr(binop, new BoolValNode(left.Val != right.Val));
            }
        }
    }
}
```

### Место в общем проекте (Интеграция)
Данная оптимизация входит в состав оптимизаций по AST-дереву
```csharp
public static class ASTOptimizer
{
    private static IReadOnlyList<ChangeVisitor> ASTOptimizations { get; } = new List<ChangeVisitor>
    {
        /* ... */
        new OptExprWithOperationsBetweenConsts(),
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
Тест для проверки операции "<":

```csharp
[Test]
public void TestOpLess1()
{
    var AST = BuildAST(@"
var c;
c = 3 < 15;
");
    var expected = new[] {
        "var c;",
        "c = true;"
    };

    var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
    CollectionAssert.AreEqual(expected, result);
}
```
Тест для проверки операции "<":

```csharp
[Test]
public void TestOpLess2()
{
    var AST = BuildAST(@"
var c;
c = 3 < 2;
");
    var expected = new[] {
        "var c;",
        "c = false;"
    };

    var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
    CollectionAssert.AreEqual(expected, result);
}
```
Тест для проверки операции ">=":

```csharp
[Test]
public void TestOpEQGREATER1()
{
    var AST = BuildAST(@"
var c;
c = 3 >= 2;
");
    var expected = new[] {
        "var c;",
        "c = true;"
    };

    var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
    CollectionAssert.AreEqual(expected, result);
}
```
