## Удаление пустых операторов в трёхадресном коде  
  
### Постановка задачи  

Необходимо совершить оптимизацию, очищающую трёхадресный код от пустых операторов.   
  
### Команда  

А.Пацеев, И.Ушаков  
  
### Зависимые и предшествующие задачи  

Предшествующие:

- Генерация трёхадресного кода

Зависимые:

- Интеграция оптимизаций трёхадресного кода между собой
  
### Теоретическая часть  

Пустые операторы могут появиться в трёхадресном коде в качестве результата применения других оптимизирующих операций.  
В ходе данной задачи было реализовано удаление таких операторов.  
  
Пустой оператор может находиться в одной из трёх позиций:  

```
// Вариант 1
noop // без метки
```  

```  
// Вариант 2
L1: noop  
a = 5 // или любая другая операция без метки  
```  
  
```
// Вариант 3  
L1: noop
L2: a = 5 // или любая другая операция с меткой
```
  
В первом случае `noop` можно просто удалить. Во втором случае возможно объединить две операции, получив `L1: a = 5`. Третий случай является самым сложным, поскольку необходимо удалить операцию `L1: noop`, а затем совершить изменение всех `goto L1` и `ifgoto L1`  на `goto L2` и `ifgoto L2` в коде, как и в уже просмотренных операциях, так и в тех, которые только предстоить просмотреть. Оптимизация удаления пустых операторов является достаточно трудоемкой, потому что она может требовать нескольких проходов по TAC.
  
### Практическая часть  
Для решения данной задачи используется подход пересоздания TAC. В цикле совершается проход по исходному TAC и аккумулируется новый оптимизированный TAC. 
```csharp
var commandsTmp = new List<Instruction>(commands);
if (commands.Count == 0)
{
    return (false, commandsTmp);
}
var results = new List<Instruction>();
var wasChanged = false;
var toAddLast = true;

for (var i = 0; i < commandsTmp.Count - 1; i++)  
{  
    var currentCommand = commandsTmp[i];
    // случай 1, просто удаляем
    if (currentCommand.Operation == "noop" && currentCommand.Label == "")
    {
        wasChanged = true;
    }
    // случаи 2 и 3, проверяем следующую операцию на наличие метки
    else if (currentCommand.Operation == "noop")
    {
       // случай 2, следующей метки нет, сливаем операции
       if (commandsTmp[i + 1].Label == "")
       {
           var nextCommand = commandsTmp[i + 1];
           wasChanged = true;
            result.Add(
                new Instruction(
                    currentCommand.Label,
                    nextCommand.Operation,
                    nextCommand.Argument1,
                    nextCommand.Argument2,
                    nextCommand.Result
                )
            );
            i += 1;
            if (i == commandsTmp.Count - 1)
            {
                toAddLast = false;
            }
       }
       // случай 3, следующая метка есть, 
       // необходимо переименовать goto по всему коду
       else
       {
            var nextCommand = commandsTmp[i + 1];
            wasChanged = true;
            var currentLabel = currentCommand.Label;
            var nextLabel = nextCommand.Label;
            result = result.Select(/* переименование */).ToList();
            for (var j = i + 1; j < commandsTmp.Count; j++)
                commands[j] = /* переименование */;
       }
    }
    // иначе просто добавляем операцию
    else {
      results.Add(commandsTmp[i]);
    }
}
```
  
### Место в общем проекте (Интеграция)  

Данная оптимизация используется в качестве одного из оптимизаторов, используемых внутри общего оптимизатора под названием `ThreeAddressCodeOptimizer`. В частности, она используется в совокупности с оптимизациями под названиями `RemoveGotoThroughGoto` и `ReplaceGotoToGoto`.
  
### Тесты  
  
В тестах проверяется корректность работы алгоритма при всех трёх возможных случаях. Помимо этого реализованы интеграционные тесты с другими оптимизациями.

Примеры тестовых кейсов:
```
6: a = b   		 	->	 6: a = b
1: noop					 9: b = a
9: b = a

L1: noop    		->   L1: b = a
b = a

1: noop				->	 a = 1
2: noop			 		 b = a
3: a = 1
4: noop
5: noop
6: b = a

goto old_label		->	 goto new_label
old_label: noop			 new_label: a = b
new_label: a = b		 goto new_label
goto old_label
```

Пример проверки корректности интеграции с `GotoThroughGoto`:
```
1: if (1 < 2)               1: #t1 = 1 < 2
    a = 4 + 5 * 6;    ->    if #t1 goto L1
else                        goto 4
    goto 4;                 goto L2
                            L1: #t2 = 5 * 6
                            #t3 = 4 + #t2
                            a = #t3
                            L2: noop
```

Выполняется проверка на не удаление L2: noop, который является последней операцией в программе.
