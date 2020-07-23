## AST-оптимизация неравенства констант

### Постановка задачи

Реализовать оптимизацию по AST дереву вида:

![Постановка задачи](1_OptExprWithOperationsBetweenConsts/t1.JPG)

### Команда
Д. Лутченко, М. Письменский

### Зависимые и предшествующие задачи

Предшествующие:

- Построение AST-дерева
- Базовые визиторы
- ChangeVisitor

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
Подход к решению задачи был следующий: если при обходе AST-дерева встречаем узел `BinOpNode`, то проверяем его левый и правый операнды и операцию в BinOpNode. Если узлы - константы, то проверяем неравенство с приведённой операцией.

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
[TestCase(@"
var c;
c = 3 < 15;
",
    ExpectedResult = new[]
    {
        "var c;",
        "c = true;"
    },
    TestName = "OpLessTrue")]
```
Тест для проверки операции "<":

```csharp
[TestCase(@"
var c;
c = 3 < 2;
",
    ExpectedResult = new[]
    {
        "var c;",
        "c = false;"
    },
    TestName = "OpLessFalse")]
```
Тест для проверки операции ">=":

```csharp
[TestCase(@"
var c;
c = 3 >= 2;
",
    ExpectedResult = new[]
    {
        "var c;",
        "c = true;"
    },
    TestName = "OpEQGREATERTrue1")]
```
