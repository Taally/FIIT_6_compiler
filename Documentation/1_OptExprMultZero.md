### AST - оптимизация выражений, в которых есть умножение на ноль

#### Постановка задачи
Реализовать оптимизацию по AST - дереву — в случае, если один из аргументов выражения умножается на ноль, то данное выражение заменить на ноль:
- a = b * 0 => a = 0
- a = 0 * b => a = 0

#### Команда
С. Рыженков, А.Евсеенко

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* Построение AST - дерева
* Базовые визиторы

#### Теоретическая часть
Данная оптимизация выполняется на AST - дереве, построенном для программы. Необходимо найти выражения, в которых происходит умножение на ноль и заменить, например, выражение типа a = b * 0 на a = 0.

#### Практическая часть
Оптимизация реализуется с применением паттерна Visitor, для этого созданный класс (реализующий оптимизацию) наследует `ChangeVisitor` и переопредеяет метод  `VisitBinOpNode`. 
```csharp
internal class OptExprMultZero : ChangeVisitor
{
    public override void VisitBinOpNode(BinOpNode binOpNode)
    {
        base.VisitBinOpNode(binOpNode);
        var operationIsMult = binOpNode.Op == OpType.MULT;
        var leftIsZero = binOpNode.Left is IntNumNode && (binOpNode.Left as IntNumNode).Num == 0;
        var rightIsZero = binOpNode.Right is IntNumNode && (binOpNode.Right as IntNumNode).Num == 0;
        if (operationIsMult && (leftIsZero || rightIsZero))
        {
            ReplaceExpr(binOpNode, new IntNumNode(0));
        }
    }
}
```

#### Место в общем проекте (Интеграция)
Данная оптимизация применяется в классе `ASTOptimizer` наряду со всеми остальными оптимизациями по AST-дереву.

#### Тесты
```csharp
[Test]
public void MultWithRightZero()
{
    var AST = BuildAST(@"
var a, b;
a = b * 0;
");
    var expected = new[] { 
        "var a, b;",
        "a = 0;"
    };
    var result = ApplyOpt(AST, new OptExprMultZero());
    CollectionAssert.AreEqual(expected, result);
}
```




