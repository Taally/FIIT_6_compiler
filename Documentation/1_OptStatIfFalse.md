### AST-оптимизация замены if(false) на его else ветку

#### Постановка задачи
Реализовать оптимизацию по AST дереву вида if(false) st1 else st2 => st2

#### Команда
К. Галицкий, А. Черкашин

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево

#### Теоретическая часть
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

#### Практическая часть
Примеры реализации метода:

```csharp
    if (n is IfElseNode ifNode)  // Если это корень if
        if (ifNode.Expr is BoolValNode boolNode && boolNode.Val == false) // Если выражение == false
        {
            if (ifNode.FalseStat != null)  // Если ветка fasle не NULL
            {
                ifNode.FalseStat.Visit(this);
                ReplaceStat(ifNode, ifNode.FalseStat);  //  Меняем наш корень на ветку else
            }
            else {
                ReplaceStat(ifNode, new EmptyNode());
            }
        }
```

#### Место в общем проекте (Интеграция)
```csharp
public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
       {
             /* ... */
           new OptStatIfFalse(),
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
if(false)
  a = 3;
else
  a = 57;
```
Результат работы:
```csharp
b = 5;
a = 57;
```
