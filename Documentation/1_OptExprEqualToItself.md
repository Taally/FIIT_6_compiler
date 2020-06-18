### AST-оптимизация замены сравнения с собой на true

#### Постановка задачи
Реализовать оптимизацию по AST-дереву — замена сравнения с собой на true:
- a == a => true
- a <= a => true
- a >= a => true

#### Команда
Д. Володин, Н. Моздоров

#### Зависимые и предшествующие задачи
Предшествующие:
- Построение AST-дерева
- Базовые визиторы

#### Теоретическая часть
Данная оптимизация выполняется на AST-дереве, построенном для данной программы. Необходимо найти в нём узлы, содержащие операции сравнения (==, <=, >=) с одной и той же переменной, и заменить эти сравнения на True.

#### Практическая часть
Нужная оптимизация производится с применением паттерна Visitor, для этого созданный класс наследует `ChangeVisitor` и
переопределяет метод `PostVisit`.
```csharp
internal class OptExprEqualToItself : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        if (n is BinOpNode binop)
        {
            if (binop.Left is IdNode Left && binop.Right is IdNode Right && Left.Name == Right.Name &&
            (binop.Op == OpType.EQUAL || binop.Op == OpType.EQLESS || binop.Op == OpType.EQGREATER))
            {
                ReplaceExpr(binop, new BoolValNode(true));
            }
        }
    }
}
```

#### Место в общем проекте (Интеграция)
Данная оптимизация применяется в классе `ASTOptimizer` наряду со всеми остальными оптимизациями по AST-дереву.

#### Пример работы
До:
```
var a, b;
a = 1;
b = (a == a);
b = (a <= a);
b = (a >= a);
```
После:
```
var a, b;
a = 1;
b = true;
b = true;
b = true;
```
