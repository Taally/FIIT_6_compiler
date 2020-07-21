## AST-оптимизация заменой условного оператора на пустой оператор

### Постановка задачи

Реализовать оптимизацию по AST дереву вида if (ex) null; else null; => null

### Команда

Карякин В.В., Карякин Д.В.

### Зависимые и предшествующие задачи

Предшествующие:

- Построение AST-дерева
- Базовые визиторы
- ChangeVisitor

### Теоретическая часть

Принцип работы данной оптимизации показан с помощью блок-схем, соответствующих некоторому поддереву абстрактного синтаксического дерева.

Ниже приведена блок-схема, которая соответствует условному оператору с пустыми узлами по true и false ветке выполнения. Узел ```IfNode``` подлежит оптимизации.

![Узлы AСT до оптимизации](1_OptIfNullElseNull/img1.png)

Данный узел ```IfNode``` должен быть заменён на пустой узел ```EmptyNode```.
Блок-схема ниже показывает, что происходит после применения этой оптимизации.

![Узлы AСT после оптимизации](1_OptIfNullElseNull/img2.png)

### Практическая часть

Данная оптимизация реализована в виде визитора, унаследованного от класса ```ChangeVisitor```.
В визиторе переопределяется метод ```PostVisit```, таким образом, чтобы при значении ```EmptyNode``` или ```null``` веток по true и false условного оператора данный узел абстрактного синтаксического дерева заменялся на ```EmptyNode``` с помощью метода ```ReplaceStat``` унаследованного от класса ```ChangeVisitor```.

Реализация оптимизации:
```csharp
/* IfNullElseNull.cs */
public class IfNullElseNull : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        if (n is IfElseNode ifn &&
            (ifn.FalseStat is EmptyNode || ifn.FalseStat == null) &&
            (ifn.TrueStat is EmptyNode || ifn.TrueStat == null))
        {
            ReplaceStat(ifn, new EmptyNode());
        }
    }
}
```

### Место в общем проекте (Интеграция)

Данная оптимизация выполняется вместе с остальными оптимизациями по абстрактному синтаксическому дереву.
```csharp
/* ASTOptimizer.cs */
private static IReadOnlyList<ChangeVisitor> ASTOptimizations { get; } = new List<ChangeVisitor>
{
    /* ... */
    new IfNullElseNull(),
    /* ... */
};
```

### Тесты

Абстрактное синтаксическое дерево для данной оптимизации создаётся в тесте.
Схема тестирования выглядит следующим образом: сначала создаётся AST, затем применяется оптимизация, после проверяется AST. Ниже приведёны несколько тестов.
```csharp
[Test]
public void RemoveInnerIf1()
{
    // if (a)
    //   if (b) EmptyNode; else EmptyNode;
    var ifInner = new IfElseNode(new IdNode("b"), new EmptyNode(), new EmptyNode());
    var ifOuter = new IfElseNode(new IdNode("a"), ifInner);
    ifInner.Parent = ifOuter;

    var root = new StListNode(ifOuter);
    ifOuter.Parent = root;

    var opt = new IfNullElseNull();
    root.Visit(opt);

    Assert.IsNull(root.Parent);
    Assert.AreEqual(root.ExprChildren.Count, 0);
    Assert.AreEqual(root.StatChildren.Count, 1);
    Assert.IsTrue(root.StatChildren[0].GetType() == typeof(EmptyNode));
}

[Test]
public void RemoveInBlock()
{
    // { if (a) EmptyNode; }
    // { if (a) EmptyNode; else EmptyNode; }
    var if1 = new IfElseNode(new IdNode("a"), new EmptyNode());
    var if2 = new IfElseNode(new IdNode("a"), new EmptyNode(), new EmptyNode());

    var block1 = new BlockNode(new StListNode(if1));
    var block2 = new BlockNode(new StListNode(if2));
    if1.Parent = block1;
    if2.Parent = block2;

    var root = new StListNode(block1);
    root.Add(block2);
    block1.Parent = block2.Parent = root;

    var opt = new IfNullElseNull();
    root.Visit(opt);

    Assert.IsNull(root.Parent);
    Assert.AreEqual(root.ExprChildren.Count, 0);
    Assert.AreEqual(root.StatChildren.Count, 2);

    foreach (var node in root.StatChildren)
    {
        Assert.IsTrue(node.GetType() == typeof(BlockNode));
        Assert.AreEqual(node.ExprChildren.Count, 0);
        Assert.AreEqual(node.StatChildren.Count, 1);
        Assert.IsTrue(node.StatChildren[0].GetType() == typeof(EmptyNode));
    }
}
```
