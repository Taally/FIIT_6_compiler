## AST-оптимизация замены if(false) на его else ветку

### Постановка задачи
Реализовать оптимизацию по AST дереву вида if(false) st1 else st2 => st2

### Команда
К. Галицкий, А. Черкашин

### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево

### Теоретическая часть
Реализовать оптимизацию по AST дереву вида if(false) st1 else st2 => st2
  * До
  ```csharp
  if(false)
    st1;
  else
    st2;
  ```
  * После
  ```csharp
  st2;
  ```

### Практическая часть
Если условием выражения "if" является константа "false" необходимо заменить все выражение на его "else ветку", в случае отсутствия ветки "else" производим замену на пустой оператор.
Пример реализации метода:

```csharp
if (n is IfElseNode ifNode) // Если это корень if
{
    if (ifNode.Expr is BoolValNode boolNode && boolNode.Val == false) // Если выражение == false
    {
        if (ifNode.FalseStat != null) // Если ветка false не null
        {
            ifNode.FalseStat.Visit(this);
            ReplaceStat(ifNode, ifNode.FalseStat); // Меняем наш корень на ветку else
        }
        else
        {
            ReplaceStat(ifNode, new EmptyNode());
        }
    }
}
```

### Место в общем проекте (Интеграция)
Данная оптимизация выполняется на AST-дереве, построенном для данной программы. Применяется в классе `ASTOptimizer`.
```csharp
private static IReadOnlyList<ChangeVisitor> ASTOptimizations { get; } = new List<ChangeVisitor>
{
    /* ... */
    new OptStatIfFalse(),
    /* ... */
};
```


### Тесты
В тестах проверяется работоспособность оптимизации и соответствие результатов:
```csharp
[Test]
public void IfFalseBlockTest()
{
    var AST = BuildAST(@"
var a, b;
if false {
a = b;
b = 1;
}
else
a = 1;
");

    var expected = new[] {
        "var a, b;",
        "a = 1;"
    };

    var result = ApplyOpt(AST, new OptStatIfFalse());
    CollectionAssert.AreEqual(expected, result);
}
```
