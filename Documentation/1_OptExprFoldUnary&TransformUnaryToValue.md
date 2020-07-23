## AST-оптимизации свертки и устранения унарных операций

### Постановка задачи
Реализовать оптимизации по AST дереву:

1. Свертка двух унарных операций

- op a == op a => True
- op a != op a => False
где op = {!, -}
- !a == a => False
- a != !a => True

2. Устранение унарной операции

- Превращение унарной операции «минус» с узлом целых чисел с Num\==1 в узел целых чисел с Num==-1
- !True=> False и !False => True
- !!a=>a
- \-\-a => a

### Команда
А. Татарова, Т. Шкуро

### Зависимые и предшествующие задачи

Предшествующие:

- Построение AST-дерева
- Базовые визиторы
- ChangeVisitor

### Теоретическая часть
Данные оптимизации должны по необходимым условиям преобразовать поддерево АСТ таким образом (Fig.5, Fig.6):

![Оптимизация свертки унарных операций](1_OptExprFoldUnary&TransformUnaryToValue/pic1.png)


![Оптимизация устранения унарных операций](1_OptExprFoldUnary&TransformUnaryToValue/pic2.png)


### Практическая часть
1. Свертка двух унарных операций
Данная оптимизация заходит только в узлы бинарных операций. Прежде всего проверяются необходимые условия: левый и правый операнды представляют собой узлы унарных операций и тип бинарной операции "равно" или "неравно". После разбирается, что в этих операндах только одна и та же переменная/константа, что тип унарных операций одинаков и т.д. Если условия выполняются, в родительском узле происходит замена бинарной операции на значение Boolean. В противном случае узел обрабатывается по умолчанию.
```csharp
public override void PostVisit(Node n)
{
    if (n is BinOpNode binOpNode)
    {
        var left = binOpNode.Left as UnOpNode;
        var right = binOpNode.Right as UnOpNode;

        if (left != null && right != null && left.Op == right.Op
            && left.Op == OpType.NOT && left.Expr is IdNode idl)
        {
            if (right.Expr is IdNode idr && idl.Name == idr.Name)
            {
                if (binOpNode.Op == OpType.EQUAL)
                {
                    ReplaceExpr(binOpNode, new BoolValNode(true));
                }
                else if (binOpNode.Op == OpType.NOTEQUAL)
                {
                    ReplaceExpr(binOpNode, new BoolValNode(false));
                }
            }
        }
        else
        if (left != null && left.Op == OpType.NOT && left.Expr is IdNode idl2
            && binOpNode.Right is IdNode idr2 && idl2.Name == idr2.Name)
        {
            /*...*/
        }
        else 
        if (right != null && right.Op == OpType.NOT && right.Expr is IdNode idr3 
            && binOpNode.Left is IdNode idl3 && idr3.Name == idl3.Name)
        {
            /*...*/
        }
    }
}
```

2. Устранение унарных операций
Данная оптимизация работает с узлами унарных операций. Прежде всего проверяется: выражение должно быть переменной или константой. Если условие не выполняется, то узел обрабатывается по умолчанию.
Если условие выполняется, то производятся следующие проверки и действия при их выполнении:

- если выражение было целочисленной константой, в родительском узле происходит замена унарной операции на узел целых чисел со значением, умноженным на -1;
- если выражение было значением Boolean, в родительском узле происходит замена унарной операции на  значение Boolean, взятое с отрицанием (было !true, стало false);
- если выражение было переменной, то дополнительно проверяется, является ли родительский узел также унарной операцией с тем же типом операции. Если является, то в родительском узле второго порядка происходит замена выражения на переменную. 

```csharp
public override void PostVisit(Node n)
{
    if (n is UnOpNode unOpNode)
    {
        if (unOpNode.Expr is IntNumNode num)
        {
            var vForNum = unOpNode.Op == OpType.UNMINUS ? -1 * num.Num
                : throw new ArgumentException("IntNumNode linked with UNMINUS");
            ReplaceExpr(unOpNode, new IntNumNode(vForNum));
        }
        else
        if (unOpNode.Expr is BoolValNode b)
        {
            var vForBool = unOpNode.Op == OpType.NOT ? !b.Val
                : throw new ArgumentException("BoolValNode linked with NOT");
            ReplaceExpr(unOpNode, new BoolValNode(vForBool));
        }
        else
        if (unOpNode.Expr is IdNode
                && unOpNode.Parent is UnOpNode && (unOpNode.Parent as UnOpNode).Op == unOpNode.Op)
        {
            if (unOpNode.Parent is UnOpNode parent && parent.Op == unOpNode.Op)
            {
                ReplaceExpr(parent, unOpNode.Expr);
            }
        }
    }
}
```

### Место в общем проекте (Интеграция)
Данные оптимизации выполняются вместе с остальными АСТ оптимизациями после построения абстрактного синтаксического дерева, но до генерации трёхадресного кода.

### Тесты
1. Свертка двух унарных операций

```csharp
[TestCase(@"
var a, b;
b = !a == !a;
b = !a != !a;
",
    ExpectedResult = new[]
    {
        "var a, b;",
        "b = true;",
        "b = false;"
    },
    TestName = "EqualID")]

[TestCase(@"
var a, b;
b = !a == a;
b = !a != a;
b = a == !a;
b = a != !a;
",
    ExpectedResult = new[]
    {
        "var a, b;",
        "b = false;",
        "b = true;",
        "b = false;",
        "b = true;"
    },
    TestName = "LeftRightUnary")]

public string[] TestOptExprFoldUnary(string sourceCode) =>
    TestASTOptimization(sourceCode, new OptExprFoldUnary());
```
2. Устранение унарных операций

```csharp
public void TransformToInt()
{
    var AST = BuildAST(@"
var a, b;
a = (-1);
");
    var expected = new[]
    {
        "var a, b;",
        "a = -1;"
    };

    var result = ApplyAstOpt(AST, new OptExprTransformUnaryToValue());
    CollectionAssert.AreEqual(expected, result);
    Assert.IsTrue((AST.root.StatChildren[1] as AssignNode).Expr is IntNumNode);
}

[Test]
public void TransformToBool()
{
    var AST = BuildAST(@"
var a, b;
a = !true;
b = !false;
");
    var expected = new[]
    {
        "var a, b;",
        "a = false;",
        "b = true;"
    };

    var result = ApplyAstOpt(AST, new OptExprTransformUnaryToValue());
    CollectionAssert.AreEqual(expected, result);
    Assert.IsTrue((AST.root.StatChildren[1] as AssignNode).Expr is BoolValNode);
    Assert.IsTrue((AST.root.StatChildren[2] as AssignNode).Expr is BoolValNode);
}

[TestCase(@"
var a, b;
a = !!b;
b = --a;
a = --b - ---a;
",
    ExpectedResult = new[]
    {
        "var a, b;",
        "a = b;",
        "b = a;",
        "a = (b - (-a));"
    },
    TestName = "TransformTwiceUnary")]

public string[] TestOptExprTransformUnaryToValue(string sourceCode) =>
    TestASTOptimization(sourceCode, new OptExprTransformUnaryToValue());
```
