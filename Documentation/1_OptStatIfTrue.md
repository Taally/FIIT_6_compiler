### AST-оптимизация замены if(true) на его true ветку

#### Постановка задачи
Реализовать оптимизацию по AST дереву вида if(true) st1 else st2 => st1

#### Команда
А. Татарова, Т. Шкуро, Д. Володин, Н. Моздоров

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево

#### Теоретическая часть
Реализовать оптимизацию по AST дереву вида if(true) st1 else st2 => st1
  * До
  ```csharp
  if(true)
    st1;
  else
    st2;
  ```
  * После
  ```csharp
  st1;
  ```

#### Практическая часть
Примеры реализации метода:

```csharp
    if (n is IfElseNode ifNode)  // Если это корень if
        if (ifNode.Expr is BoolValNode boolNode && boolNode.Val) // Если выражение == true
        {
            if (ifNode.TrueStat != null)
            {
                ifNode.TrueStat.Visit(this);
            }
            ReplaceStat(ifNode, ifNode.TrueStat);
        }
```

#### Место в общем проекте (Интеграция)
```csharp
public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
       {
             /* ... */
           new OptStatIftrue(),
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
var a;
a = 14;
if(true)
  a = a - 4;
else
  a = a + 10;
```
Результат работы:
```csharp
a = 14;
a = a - 4;
```
