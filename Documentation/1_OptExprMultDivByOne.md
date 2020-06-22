## AST-оптимизация умножения на единицу слева и справа, деления на единицу справа

### Постановка задачи

Реализовать оптимизацию по AST дереву вида a\*1 = a, 1\*a = a, a/1 = a

### Команда

А. Татарова, Т. Шкуро

### Зависимые и предшествующие задачи

Предшествующие:

- Построение AST-дерева
- Базовые визиторы
- ChangeVisitor

### Теоретическая часть

Эта оптимизация представляет собой визитор, унаследованный от ChangeVisitor и меняющий ссылки между узлами ACT.
Рассмотрим некие узлы АСТ:

![Узлы AСT до оптимизации](1_OptExprMultDivByOne/pic1.png)

Эта блок-схема соответствует строчке  ```b = a * 1```.
Данная оптимизация должна отработать так: ``` b = a ```.
Блок-схема ниже показывает, что происходит с деревом после применения этой оптимизации:

![Узлы AСT после оптимизации](1_OptExprMultDivByOne/pic2.png)

### Практическая часть

Алгоритм заходит только в узлы бинарных операций. Прежде всего проверяются необходимые условия: тип операции либо умножение, либо деление и что один из операндов это единица. Если условия выполняются, в родительском узле происходит замена бинарной операции на переменную.

```csharp
public class OptExprMultDivByOne : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        if (n is BinOpNode binOpNode && (binOpNode.Op == OpType.MULT || binOpNode.Op == OpType.DIV))
        {
            if (binOpNode.Left is IntNumNode intNumNodeLeft && intNumNodeLeft.Num == 1 &&
                binOpNode.Op != OpType.DIV) // Do not replace "1 / a"
            {
                ReplaceExpr(binOpNode, binOpNode.Right);
            }
            else
            if (binOpNode.Right is IntNumNode intNumNodeRight && intNumNodeRight.Num == 1)
            {
                ReplaceExpr(binOpNode, binOpNode.Left);
            }
        }
    }
}
```

### Место в общем проекте (Интеграция)

Данная оптимизация выполняется вместе с остальными АСТ оптимизациями после построения абстрактного синтаксического дерева, но до генерации трехадресного кода. 

### Тесты

```csharp
[Test]
public void MultAndDivByLeftRightOne()
{
    var AST = BuildAST(@"
var a, b;
a = 1 * a * 1 + (1 * b / 1) * 1 / 1;
");

    var expected = new[] {
        "var a, b;",
        "a = (a + b);"
    };

    var result = ApplyOpt(AST, new OptExprMultDivByOne());
    CollectionAssert.AreEqual(expected, result);
}
```
