## Итерационный алгоритм в обобщённой структуре

### Постановка задачи

Реализовать итеративный алгоритм в обобщённой структуре.

### Команда

К. Галицкий, А. Черкашин

### Зависимые и предшествующие задачи

Предшествующие задачи:

* Построение CFG. Обход потомков и обход предков для каждого базового блока

Зависимые задачи:

* Вычисление передаточной функции для достигающих определений композицией передаточных функций команд
* Активные переменные
* Доступные выражения
* Передаточная функция в структуре распространения констант
* Достигающие определения
* Итерационный алгоритм в структуре распространения констант


### Теоретическая часть

В рамках этой задачи необходимо было реализовать обобщённый итерационный алгоритм.

Входы итерационного алгоритма:

* Граф потока данных с помеченными  входными и выходными узлами
* Направление потока данных
* Множество значений V
* Оператор сбора /\
* Множество функций f где f(b) из F представляет собой передаточную функцию для блока b
* Константное значение V вход или V выход из V, представляющее собой граничное условие для прямой и обратной структуры соответственно.

Выходы итерационного алгоритма:

* Значения из V для in(b) и out(b) для каждого блока b в CFG


Алгоритм для решения прямой задачи потока данных: 

![Прямая задача](3_GenericIterativeAlgorithm/pic2.JPG)

Алгоритм для решения обратной задачи потока данных: 

![Обратная задача](3_GenericIterativeAlgorithm/pic1.JPG)

Служит для избежания базового итеративного алгоритма для каждой структуры потока данных используемой на стадии оптимизации.
Его задача вычисление in и out для каждого блока как ряд последовательных приближений. А также его использование предоставляет ряд полезных свойств приведённых ниже:

![Свойства алгоритма](3_GenericIterativeAlgorithm/pic3.JPG)

### Практическая часть

Реализовали класс выходных данных:
```csharp
public class InOutData<T> : Dictionary<BasicBlock, (T In, T Out)> // Вид выходных данных вида (Базовый блок, (его входы, его выходы))
```

Указываем вид прохода алгоритма:
```csharp
public enum Direction { Forward, Backward }
```

Реализовали обобщённый итерационный алгоритм для прямого и обратного прохода. Алгоритм реализован в виде абстрактного класса, это предоставит возможность каждому итерационному алгоритму самостоятельно переопределить входные данные, передаточную функцию, верхний или нижний элемент пула решётки (относительно прохода алгоритма) и оператор сбора.
Пример реализации:
```csharp
public abstract class GenericIterativeAlgorithm<T> where T : IEnumerable
{
    public virtual InOutData<T> Execute(ControlFlowGraph graph, bool useRenumbering = true)
    {
        GetInitData(graph, useRenumbering, out var blocks, out var data,
            out var getPreviousBlocks, out var getDataValue, out var combine);

        var outChanged = true;
        Iterations = 0;
        while (outChanged)
        {
            outChanged = false;
            foreach (var block in blocks)
            {
                var inset = getPreviousBlocks(block).Aggregate(Init, (x, y) => CollectingOperator(x, getDataValue(y)));
                var outset = TransferFunction(block, inset);

                if (!Compare(outset, getDataValue(block)))
                {
                    outChanged = true;
                }
                data[block] = combine(inset, outset);
            }
            ++Iterations;
        }
        return data;
    }
}
```

Переопределение входных данных, передаточной функции, оператора сбора и элементов пула решётки вынесли во вспомогательный алгоритм.

### Место в общем проекте (Интеграция)

Используется для вызова итерационных алгоритмов в единой структуре.

### Тесты

В тестах проверяется использование итерационных алгоритмов в обобщённой структуре, результаты совпадают с ожидаемыми. Ниже приведён тест проверки работы алгоритма живых переменных.

```csharp
[Test]
public void LiveVariables()
{
    var program = @"
var a,b,c;

input (b);
a = b + 1;
if a < c
    c = b - a;
else
    c = b + a;
print (c);
";

    var cfg = GenCFG(program);
    var resActiveVariable = new LiveVariables().Execute(cfg);
    var actual = cfg.GetCurrentBasicBlocks()
        .Select(z => resActiveVariable[z])
        .Select(p => ((IEnumerable<string>)p.In, (IEnumerable<string>)p.Out))
        .ToList();

    var expected =
        new List<(IEnumerable<string>, IEnumerable<string>)>()
        {
            (new HashSet<string>(){"c"}, new HashSet<string>(){ "c" }),
            (new HashSet<string>(){"c"}, new HashSet<string>(){"a", "b"}),
            (new HashSet<string>(){"a", "b"}, new HashSet<string>(){ "c" }),
            (new HashSet<string>(){"a", "b"}, new HashSet<string>(){"c"}),
            (new HashSet<string>(){"c"}, new HashSet<string>(){ }),
            (new HashSet<string>(){ }, new HashSet<string>(){ })
        };

    AssertSet(expected, actual);
}
```
