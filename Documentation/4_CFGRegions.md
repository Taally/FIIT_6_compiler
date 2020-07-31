## Построение областей

### Постановка задачи
В данной задаче необходимо построить иерархию областей графа потока управления.

### Команда
Карякин В.В., Карякин Д.В.

### Зависимые и предшествующие задачи
Предшествующие:

- Построение потока графа управления
- Определение всех естественных циклов

### Теоретическая часть
В процессе анализа на основе областей программа рассматривается как иерархия областей, которые грубо можно считать частями графа потока, имеющими единственную точку входа. Дадим формальное определение области:

![Определение области](4_CFGRegions/img1.PNG)

В рассматриваемом разбиении предполагается, что граф потока приводим. Для построения областей мы идентифицируем естественные циклы. Любые два из которых либо не пересекаются, либо один из них вложен в другой. Приведём используемый алгоритм построения восходящего порядка областей приводимого графа потока.

![Итерационный алгоритм](4_CFGRegions/img2.PNG)

### Практическая часть
Для представления региона использовался класс Region, где поле Initial используется для хранения блока графа, соответствующего областям-листьям.
```csharp
public class Region
{
    public IReadOnlyCollection<Region> IncludedRegions;
    public IReadOnlyCollection<(Region, Region)> Edges;
    public BasicBlock Initial;

    public Region(IReadOnlyCollection<Region> regions = null, IReadOnlyCollection<(Region, Region)> edges = null, BasicBlock initial = null)
    {
        IncludedRegions = regions;
        Edges = edges;
        Initial = initial;
    }
}
```

При построении иерархии областей применяются два метода. `FindRegions`  добавляет в список регионов области-листья, затем отсортированные по вложенности циклы поочередно сводятся к отдельным узлам. После обхода всех естественных циклов добавляем в конец списка область, состоящую из всего графа потока целиком.   

```csharp
private void FindRegions()
{
    foreach (var item in blocks)
    {
        regions.Add(new Region(initial: item));
        BlockToRegion.Add(item, regions.Count - 1);
        curID++;
    }
    for (var i = 0; i < cycles.Count; ++i)
    {
        CollapseCycle(cycles[i]);
    }
    var tempEdges = new List<(Region, Region)>();
    foreach (var entry in children)
    {
        foreach (var second in entry.Value)
        {
            tempEdges.Add((regions[BlockToRegion[entry.Key]], regions[BlockToRegion[second]]));
        }
    }
    regions.Add(new Region(blocks.Select(x => regions[BlockToRegion[x]]).ToList(), tempEdges));
}
```

Метод `CollapseCycle` замещает новым узлом переданный естественный цикл на графе потока управления. Добавляя новый узел, мы перенаправляем рёбра на заголовок цикла, из цикла во внешнюю область. Узлы и ребра внутри цикла будут соответствовать новому региону. 

```csharp
private void CollapseCycle(IReadOnlyCollection<BasicBlock> cycle)
{
    /* ... */
    foreach (var curVertex in blocks)
    {
        if (!cycle.Contains(curVertex))
        {
            var temp = children[curVertex].ToList();
            foreach (var child in temp)
            {
                if (child == cycle.First())
                {
                    children[curVertex].Remove(child);
                    children[curVertex].Add(bodyBlock);
                }
            }
    /* ... */
    var innerRegions = cycle.Select(x => regions[BlockToRegion[x]]).ToList();
    var innerEdged = cycleEdges.Select(x => (regions[BlockToRegion[x.Item1]], regions[BlockToRegion[x.Item2]])).ToList();

    regions.Add(new Region(innerRegions, innerEdged));
    BlockToRegion.Add(bodyBlock, regions.Count - 1);
    /* ... */
}
```

### Место в общем проекте (Интеграция)
Данный метод был успешно интегрирован в проект оптимизирующего компилятора. Использовать предлагаемое решение можно, создав объект класса `CFGRegions`, используя в качестве параметра граф потока управления.

```csharp
var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
var cfg = new ControlFlowGraph(blocks);
var regions = new CFGregions(cfg);
```

#### Тесты
- Разбиение на регионы графа потока управления без цикла

```csharp
[Test]
public void WithoutCycles()
{
    var cfg = GenCFG(@"
var a, b;
a = 5;
if b != 2
{
    a = 6;
}
a = 8;
");
    var result = new CFGRegions(cfg);

    var actual = result.Regions.Select(x => (x.Edges?.Count ?? 0, x.IncludedRegions?.Count ?? 0));
    var expected = new[]
    {
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (5, 5),
    };

    Assert.AreEqual(6, result.Regions.Count);
    CollectionAssert.AreEquivalent(expected, actual);
}
```

- Разбиение на регионы графа потока управления с одним естественным циклом

```csharp
[Test]
public void OneCycle()
{
    var cfg = GenCFG(@"
var a, b, x, c;
for x = 1, 10
{
    a = 2;
}
c = a + b;
");
    var result = new CFGRegions(cfg);

    var actual = result.Regions.Select(x => (x.Edges?.Count ?? 0, x.IncludedRegions?.Count ?? 0));
    var expected = new []
    {
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),                
        (1, 1),
        (1, 2),
        (4, 5)
    };

    Assert.AreEqual(9, result.Regions.Count);
    CollectionAssert.AreEquivalent(expected, actual);
}
```

- Разбиение на регионы графа потока управления с двумя естественными циклами

```csharp
[Test]
public void TwoCycles()
{
    var cfg = GenCFG(@"
var a, b, x, c;
for x = 1, 10
{
    a = 2;
}
for x = 1, 10
{
    b = 55;
}
c = a + b;
");
    var result = new CFGRegions(cfg);

    var actual = result.Regions.Select(x => (x.Edges?.Count ?? 0, x.IncludedRegions?.Count ?? 0));
    var expected = new[]
    {
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (1, 1),
        (1, 2),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (1, 1),
        (1, 2),
        (6, 7)
    };

    Assert.AreEqual(14, result.Regions.Count);
    CollectionAssert.AreEquivalent(expected, actual);
}
```

- Разбиение на регионы графа потока управления с двумя вложенными циклами

```csharp
[Test]
public void TwoNestedCycles()
{
    var cfg = GenCFG(@"
var a, b, c, x;
for x = 1, 10
{
    for a = 1, 10
    {
        c = 2;
    }
    for b = 1, 10
    {
        c = 4;        
    }
}
");
    var loops = NaturalLoop.GetAllNaturalLoops(cfg);
    Assert.AreEqual(3, loops.Count);
    var result = new CFGRegions(cfg);

    var actual = result.Regions.Select(x => (x.Edges?.Count ?? 0, x.IncludedRegions?.Count ?? 0));
    var expected = new[]
    {
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (0, 0),
        (1, 2),
        (1, 1),
        (1, 2),
        (1, 1),
        (5, 6),
        (1, 1),
        (4, 5)
    };

    Assert.AreEqual(19, result.Regions.Count);
    CollectionAssert.AreEquivalent(expected, actual);
}
```
