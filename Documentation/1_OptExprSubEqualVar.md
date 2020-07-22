## AST-оптимизация замены выражений вида a-a => 0

### Постановка задачи
Реализовать оптимизацию по AST дереву вида a-a => 0

### Команда
И. Потапов

### Зависимые и предшествующие задачи
Предшествующие:

- Построение AST-дерева
- Базовые визиторы

### Теоретическая часть
Данная оптимизация выполняется на AST-дереве, построенном для данной программы. Необходимо найти в нём узлы вида ```b = a-a``` и заменить их на ```b = 0```.

### Практическая часть
Нужная оптимизация производится с применением паттерна Visitor, для этого созданный класс наследует `ChangeVisitor` и переопределяет метод `PostVisit`.
```csharp
public class OptExprSubEqualVar : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        if (n is BinOpNode binop && binop.Op == OpType.MINUS
            && binop.Left is IdNode id1 && binop.Right is IdNode id2 && id1.Name == id2.Name)
        {
            if (id1.Name == id2.Name)
            {
                ReplaceExpr(binop, new IntNumNode(0));
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
a = b - b;
",
    ExpectedResult = new[]
    {
        "var a, b;",
        "a = 0;"
    },
    TestName = "SubID")]

[TestCase(@"
var a, b;
print(a - a, b - b, b - a, a - a - b);
",
    ExpectedResult = new[]
    {
        "var a, b;",
        "print(0, 0, (b - a), (0 - b));"
    },
    TestName = "SubIDInPrint")]

public string[] TestOptExprSubEqualVar(string sourceCode) =>
    TestASTOptimization(sourceCode, new OptExprSubEqualVar());
```
