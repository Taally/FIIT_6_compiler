## AST-оптимизация замены сравнения переменной с собой на булевскую константу false

### Постановка задачи
Реализовать оптимизацию по AST дереву вида (a > a, a != a ) = False

### Команда
К. Галицкий, А. Черкашин

### Зависимые и предшествующие задачи

Предшествующие:

- Построение AST-дерева
- Базовые визиторы
- ChangeVisitor

### Теоретическая часть
Реализовать оптимизацию по AST дереву вида (a > a, a != a ) = False:

  * До
  
```csharp
#t1 = a > a;
```

  * После

```csharp
#t1 = False;
```

  * До

```csharp
#t1 = a != a;
```

  * После

```csharp
#t1 = False;
```

### Практическая часть
Если в выражениях вида ">", "<", "!=" операции сравнения проводятся с одной и той же переменной, необходимо заменить сравнение на результат "False".
Пример реализации метода:

```csharp
public override void PostVisit(Node n)
{
    if (n is BinOpNode binOpNode &&
        (binOpNode.Op == OpType.GREATER || binOpNode.Op == OpType.LESS || binOpNode.Op == OpType.NOTEQUAL)
        &&
        // Для цифр и значений bool:
        (binOpNode.Left is IntNumNode inl && binOpNode.Right is IntNumNode inr && inl.Num == inr.Num
        || binOpNode.Left is BoolValNode bvl && binOpNode.Right is BoolValNode bvr && bvl.Val == bvr.Val
        // Для переменных:
        || binOpNode.Left is IdNode idl && binOpNode.Right is IdNode idr && idl.Name == idr.Name))
    {
        ReplaceExpr(binOpNode, new BoolValNode(false));
    }
}
```

### Место в общем проекте (Интеграция)
Данная оптимизация выполняется на AST-дереве, построенном для данной программы. Применяется в классе `ASTOptimizer`.
```csharp
private static IReadOnlyList<ChangeVisitor> ASTOptimizations { get; } = new List<ChangeVisitor>
{
    /* ... */
    new OptExprSimilarNotEqual(),
    /* ... */
};
```

### Тесты
В тестах проверяется работоспособность оптимизации и соответствие результатов:
```csharp
[TestCase(@"
var a, b, d, k, c;
c = a>a;
b = k<k;
d = a != a;
d = 1 > 1;
",
    ExpectedResult = new[]
    {
        "var a, b, d, k, c;",
        "c = false;",
        "b = false;",
        "d = false;",
        "d = false;",
    },
    TestName = "SimilarNotEqual")]

public string[] TestOptExprSimilarNotEqual(string sourceCode) =>
    TestASTOptimization(sourceCode, new OptExprSimilarNotEqual());
```
