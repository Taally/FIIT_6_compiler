### AST-оптимизация замены оператора сравнения двух значений на его булево значение

#### Постановка задачи
Реализовать оптимизацию по AST дереву вида false == false -> true, 5 == 6 -> false

#### Команда
А. Пацеев, И. Ушаков

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево

#### Теоретическая часть
Реализовать оптимизацию по AST дереву вида false == false -> true, 5 == 6 -> false
  * До
  ```csharp
    5 == 5
    5 == 6
    false == false
    true == false
  ```
  * После
  ```csharp
  true
  false
  true
  false
  ```

#### Практическая часть
Эта оптимизация представляет собой визитор, унаследованный от ChangeVisitor. Пример реализации метода:

```csharp
internal class OptExprEqualBoolNumId : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binop)
            {
                switch (binop.Op)
                {
                    case OpType.EQUAL:
                        if (binop.Left is IntNumNode leftNode2 && binop.Right is IntNumNode rightNode2)
                        {
                            ReplaceExpr(binop, new BoolValNode(leftNode2.Num == rightNode2.Num));
                            break;
                        }
                        if (binop.Left is BoolValNode leftNode3 && binop.Right is BoolValNode rightNode3)
                        {
                            ReplaceExpr(binop, new BoolValNode(leftNode3.Val == rightNode3.Val));
                        }
                        break;
                }
            }
        }
    }
```

#### Место в общем проекте (Интеграция)
```csharp
public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
       {
             /* ... */
           new OptExprEqualBoolNumId(),
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

#### Пример работы
Исходный код программы:
```csharp
while (true == true) 
    a = 5; 
```

Результат работы:
```csharp
while (true) 
    a = 5; 
```
