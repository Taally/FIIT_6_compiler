### AST-оптимизация замены сравнения переменной с собой на булевскую константу false

#### Постановка задачи
Реализовать оптимизацию по AST дереву вида (a > a, a != a ) = False

#### Команда
К. Галицкий, А. Черкашин

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево

#### Теоретическая часть
Реализовать оптимизацию по AST дереву вида (a > a, a != a ) = False:
  * До
  ```csharp
  #t1 = a > a;
  ```
  * После
  ```csharp
  #t1 = False;
  ```
  * До
  ```csharp
  #t1 = a != a;
  ```
  * После
  ```csharp
  #t1 = False;
  ```

#### Практическая часть
Примеры реализации метода:

```csharp
        if (
                // Для цифр и значений bool :
                (binop.Left is IntNumNode && binop.Right is IntNumNode && (binop.Left as IntNumNode).Num == (binop.Right as IntNumNode).Num && (binop.Op == OpType.GREATER || binop.Op == OpType.LESS))
                || ((binop.Left is BoolValNode && binop.Right is BoolValNode && (binop.Left as BoolValNode).Val == (binop.Right as BoolValNode).Val && (binop.Op == OpType.GREATER || binop.Op == OpType.LESS)))
                || ((binop.Left is IntNumNode && binop.Right is IntNumNode && (binop.Left as IntNumNode).Num == (binop.Right as IntNumNode).Num && binop.Op == OpType.NOTEQUAL))
                || ((binop.Left is BoolValNode && binop.Right is BoolValNode && (binop.Left as BoolValNode).Val == (binop.Right as BoolValNode).Val && binop.Op == OpType.NOTEQUAL))
                // Для переменных :
                || ((binop.Left is IdNode && binop.Right is IdNode && (binop.Left as IdNode).Name == (binop.Right as IdNode).Name && (binop.Op == OpType.GREATER || binop.Op == OpType.LESS)))
                || ((binop.Left is IdNode && binop.Right is IdNode && (binop.Left as IdNode).Name == (binop.Right as IdNode).Name && binop.Op == OpType.NOTEQUAL))
                )
            {
                binop.Left.Visit(this);
                binop.Right.Visit(this); // сделать то же в правом поддереве
                ReplaceExpr(binop, new BoolValNode(false)); // Заменить себя на своё правое поддерево
            }
            else
            {
                base.VisitBinOpNode(binop);
            }
```

#### Место в общем проекте (Интеграция)
```csharp
public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
       {
             /* ... */
           new OptExprSimilarNotEqual(),
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
var a, b;
b = 5
a = b > b
```
Результат работы:
```csharp
var a, b;
b = 5;
a = false;
```
