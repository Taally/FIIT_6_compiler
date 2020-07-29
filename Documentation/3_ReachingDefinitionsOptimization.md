## Удаление мёртвого кода, используя InOut информацию из Достигающих определений

### Постановка задачи
Необходимо удалить мёртвый код в пределах всех программы с использованием информации из Достигающих определений.

### Команда
Д. Володин, Н. Моздоров

### Зависимые и предшествующие задачи
Предшествующие: 

- Анализ достигающих определений
- Построение графа потока управления

### Теоретическая часть
После выполнения анализа достигающих определений мы получаем на выходе IN-OUT информацию о каждом базовом блоке программы: какие определения являются достигающими в начале блока и какие в конце. Если какое-то определение входит в IN-множество некоторого блока, но не входит в его OUT-множество, то это означает, что такое определение затирается в данном базовом блоке. Тогда необходимо проанализировать граф потока управления и найти места, где данное определение используется. Если таких мест нет, то такое определение является мёртвым кодом, и его можно удалить.

### Практическая часть
Для данной оптимизации был создан класс `ReachingDefinitionsOptimization` с основным методом `DeleteDeadCode`, который выполняет данную оптимизацию. Вначале выполняется анализ достигающих переменных, и результат записывается в переменную `info`, затем создаются множества используемых переменных для каждого блока `usedVars` и множество `usedDefinitions` определений, которые используются в коде. Далее следует цикл, пока находятся определения для удаления, перебираются все базовые блоки, для каждого находятся определения-кандидаты на удаление как разность множеств IN и OUT, и для каждого кандидата выполняется проверка:

- что определение не было добавлено в `usedDefinitions`
- что определение не используется в текущем блоке до своего затирания
- что определение не используется в блоке, в котором оно находится, после его объявления
- что определение не используется в других блоках

Если все эти условия выполняются, определение удаляется из своего базового блока, и `info` находится заново.

Последний пункт реализуется с помощью обхода графа потока управления в ширину с использованием очереди `queue`. Основной цикл этой проверки выглядит так:

```csharp
while (queue.Count != 0)
{
    var block = queue.Dequeue();
    foreach (var child in graph.GetChildrenBasicBlocks(graph.VertexOf(block)).Select(z => z.block))
    {
        var isRewritten = !info[child].Out.Contains(definitionToCheck);
        var isUsed = usedVars[block].Contains(definitionToCheck.Result);

        if (!isRewritten)
        {
            if (isUsed)
            {
                return true;
            }
            else
            {
                queue.Enqueue(child);
            }
        }
        else
        {
            if (!isUsed)
            {
                continue;
            }
            else
            {
                // we need to check instructions before definitionToCheck is rewritten
                foreach (var instruction in child.GetInstructions())
                {
                    if (instruction.Argument1 == definitionToCheck.Result
                        || instruction.Argument2 == definitionToCheck.Result)
                    {
                        return true;
                    }

                    if (instruction.Result == definitionToCheck.Result)
                    {
                        break;
                    }
                }
            }
        }
    }
}
```

### Место в общем проекте (Интеграция)
Данная оптимизация использует результат работы анализа Доступных выражений и выполняет изменение списков инструкций в базовых блоках. Использование данной оптимизации доступно в IDE.

### Тесты
В тестах проверяется результат выполнения оптимизации для различных тестовых случаев. Имеются тесты на оптимизацию простых программ, программ с циклами и условиями. Самый объёмный тест выглядит следующим образом:

```
var a, b, c, d, i, j;
a = 1;
b = 2;
i = 100;
while i > 0
{
    i = i / 2;
    a = a + 1;
    b = b - 1;
    if i > 20
    {
        a = 10;
        b = 20;
    }
    else
        b = 10;
    d = a * b;
    if (d > 100)
        d = a * a;
    else
        d = b * b;
    for j = 1, 5
        d = d * 2;
}
```