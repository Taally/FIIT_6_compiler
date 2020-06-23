## AST-оптимизация замены выражений вида 2*3 => 6

### Постановка задачи
Реализовать оптимизацию по AST дереву вида 2*3 => 6

### Команда
И. Потапов

### Зависимые и предшествующие задачи
Предшествующие:

- Построение AST-дерева
- Базовые визиторы

### Теоретическая часть
Данная оптимизация выполняется на AST-дереве, построенном для данной программы. Необходимо найти в нём узлы вида ```b = 2*3``` и заменить их на ```b = 6```.

### Практическая часть
Нужная оптимизация производится с применением паттерна Visitor, для этого созданный класс наследует `ChangeVisitor` и переопределяет метод `PostVisit`.

```csharp
public class OptExprAlgebraic : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            // Algebraic expressions of the form: 2 * 3 => 6
            if (n is BinOpNode binop && binop.Left is IntNumNode intNumLeft && binop.Right is IntNumNode intNumRight)
            {
                var result = new IntNumNode(0);
                switch (binop.Op)
                {
                    case OpType.PLUS:
                        result.Num = intNumLeft.Num + intNumRight.Num;
                        break;
                    case OpType.MINUS:
                        result.Num = intNumLeft.Num - intNumRight.Num;
                        break;
                    case OpType.DIV:
                        result.Num = intNumLeft.Num / intNumRight.Num;
                        break;
                    case OpType.MULT:
                        result.Num = intNumLeft.Num * intNumRight.Num;
                        break;
                    default:
                        return;
                }
                ReplaceExpr(binop, result);
            }
        }
    }
```

### Место в общем проекте (Интеграция)
Данная оптимизация применяется в классе `ASTOptimizer` наряду со всеми остальными оптимизациями по AST-дереву.

### Пример работы

```csharp
[Test]
public void SumNumTest()
{
    var AST = BuildAST(@"
            var a, b;
            a = 1 + 41;
            ");
    var expected = new[] {
            "var a, b;",
            "a = 42;"
            };

    var result = ApplyOpt(AST, new OptExprAlgebraic());
    CollectionAssert.AreEqual(expected, result);
}

[Test]
public void MultNumTest()
{
    var AST = BuildAST(@"
        var a, b;
        a = 6 * 7;
        ");
    var expected = new[] {
        "var a, b;",
        "a = 42;"
        };

    var result = ApplyOpt(AST, new OptExprAlgebraic());
    CollectionAssert.AreEqual(expected, result);
}
```
