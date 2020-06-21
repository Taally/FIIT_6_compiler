### AST-оптимизация замены while(false) st ветки на null

#### Постановка задачи
Реализовать оптимизацию по AST дереву вида while (false) st на null

#### Команда
А. Пацеев, И. Ушаков

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево

#### Теоретическая часть
Реализовать оптимизацию по AST дереву вида while(false) st -> null
  * До
  ```csharp
	while (false) 
    	a = 5; 
  ```
  * После
  ```csharp
	null
  ```

#### Практическая часть
Эта оптимизация представляет собой визитор, унаследованный от ChangeVisitor. Пример реализации метода:

```csharp
internal class OptWhileFalseVisitor : ChangeVisitor
    {
        public override void PostVisit(Node nd)
        {
            if (!(nd is WhileNode n))
            {
                return;
            }

            if (n.Expr is BoolValNode bn && !bn.Val)
            {
                ReplaceStat(n, new EmptyNode());
            }
            else
            {
                n.Expr.Visit(this);
            }
        }
    }
```

#### Место в общем проекте (Интеграция)
```csharp
public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
       {
             /* ... */
           new OptWhileFalseVisitor(),
             /* ... */
       };

       public static void Optimize(Parser parser)
       {
           int optInd = 0;
           do
           {
               parser.root.Visit(Optimizations[optInd]);
               if (Optimizations[optInd].Changed)
                   optInd = 0;
               else
                   ++optInd;
           } while (optInd < Optimizations.Count);
       }
```

#### Тесты

```csharp
[Test]
public void TestShouldCreateNoop()
{
    var AST = BuildAST(@"var a;
while false
a = true;");
    var expected = new[] {
        "var a;"
    };

    var result = ApplyOpt(AST, new OptWhileFalseVisitor());
    CollectionAssert.AreEqual(expected, result);
}

[Test]
public void TestShouldNotCreateNoop()
{
    var AST = BuildAST(@"var a;
a = false;
while a
a = true;");

    var expected = new[] {
        "var a;",
        "a = false;",
        "while a",
        "  a = true;"
    };

    var result = ApplyOpt(AST, new OptWhileFalseVisitor());
    CollectionAssert.AreEqual(expected, result);
}
```
