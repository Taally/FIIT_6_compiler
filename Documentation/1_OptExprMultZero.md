## AST-оптимизация выражений, в которых есть умножение на ноль

### Постановка задачи
Реализовать оптимизацию по AST - дереву — в случае, если один из аргументов выражения умножается на ноль, то данное выражение заменить на ноль:

- a = b * 0 => a = 0
- a = 0 * b => a = 0

### Команда

С. Рыженков, А.Евсеенко

### Зависимые и предшествующие задачи

Предшествующие:

- Построение AST-дерева
- Базовые визиторы
- ChangeVisitor

### Теоретическая часть
Данная оптимизация выполняется на AST - дереве, построенном для программы. Необходимо найти выражения, в которых происходит умножение на ноль и заменить, например, выражение типа a = b * 0 на a = 0.

### Практическая часть
Оптимизация реализуется с применением паттерна Visitor, для этого созданный класс (реализующий оптимизацию) наследует `ChangeVisitor` и переопределяет метод  `PostVisit`. 

```csharp
public class OptExprMultZero : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        if (n is BinOpNode binOpNode && binOpNode.Op == OpType.MULT &&
            (binOpNode.Left is IntNumNode intNumLeft && intNumLeft.Num == 0 ||
            binOpNode.Right is IntNumNode intNumRight && intNumRight.Num == 0))
        {
            {
                ReplaceExpr(binOpNode, new IntNumNode(0));
            }
        }
    }
}
```

### Место в общем проекте (Интеграция)
Данная оптимизация применяется в классе `ASTOptimizer` наряду со всеми остальными оптимизациями по AST-дереву.

### Тесты

```csharp
[TestCase(@"
var a, b;
a = 0 * b + b * a * 0 + 5;
",
    ExpectedResult = new[]
    {
        "var a, b;",
        "a = ((0 + 0) + 5);"
    },
    TestName = "MultWithRightLeftZero")]

public string[] TestOptExprMultZero(string sourceCode) =>
    TestASTOptimization(sourceCode, new OptExprMultZero());
```
