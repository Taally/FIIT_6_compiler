### AST - оптимизация выражений, в которых есть суммирование с нулем

#### Постановка задачи
Реализовать оптимизацию по AST - дереву — в случае, если один из аргументов выражения с суммированием равен нулю, то данное выражение заменить на аргумент отличный от нуля:
- a = b + 0 => a = b
- a = 0 + b => a = b

#### Команда
С. Рыженков, А.Евсеенко

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* Построение AST - дерева
* Базовые визиторы

#### Теоретическая часть
Данная оптимизация выполняется на AST - дереве, построенном для программы. Необходимо найти выражения, одним аргументом которого является 0, которые содержат бинарную оперцию "+", и заменить выражение типа a = b + 0 на a = b.

#### Практическая часть
Оптимизация реализуется с применением паттерна Visitor, для этого созданный класс (реализующий оптимизацию) наследует `ChangeVisitor` и переопредеяет метод  `PostVisit`. 
```csharp
public class OptExprSumZero : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        if (n is BinOpNode binOpNode && binOpNode.Op == OpType.PLUS)
        {
            if (binOpNode.Left is IntNumNode intNodeLeft && intNodeLeft.Num == 0)
            {
                ReplaceExpr(binOpNode, binOpNode.Right);
            }
            else if (binOpNode.Right is IntNumNode intNodeRight && intNodeRight.Num == 0)
            {
                ReplaceExpr(binOpNode, binOpNode.Left);
            }
        }
    }
}
```

#### Место в общем проекте (Интеграция)
Данная оптимизация применяется в классе `ASTOptimizer` наряду со всеми остальными оптимизациями по AST-дереву.

#### Тесты
```csharp
[Test]
public void SumWithRightZero()
{
    var AST = BuildAST(@"
var a, b;
a = b + 0;
");
    var expected = new[] {
        "var a, b;",
        "a = b;"
    };

    var result = ApplyOpt(AST, new OptExprSumZero());
    CollectionAssert.AreEqual(expected, result);
}
```
