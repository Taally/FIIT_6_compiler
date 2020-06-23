## Построение графа потока управления

### Постановка задачи
Реализовать алгоритм построения графа потока управления из списка базовых блоков.

### Команда
Д. Лутченко, М. Письменский

### Зависимые и предшествующие задачи
Зависимые задачи:

- Упорядочение CFG в глубину с построением глубинного остовного дерева
- Итерационный алгоритм в обобщённой структуре
- Анализ достигающих определений
- Альтернативная реализация хранения IN OUT в виде битовых векторов. Интеграция данного представления в существующий итерационный алгоритм.

Предшествующие задачи: 

- Разбиение на ББл (от лидера до лидера)

### Теоретическая часть
В графе потока управления каждый узел (вершина) графа соответствует базовому блоку — прямолинейному участку кода, не содержащему в себе ни операций передачи управления, ни точек, на которые управление передается из других частей программы. Имеется лишь два исключения:

 - точка, на которую выполняется переход, является первой инструкцией в базовом блоке
 - базовый блок завершается инструкцией перехода

Направленные дуги используются в графе для представления инструкций перехода. Также, добавлены два специализированных блока:

- входной блок, через который управление входит в граф
- выходной блок, который завершает все пути в данном графе

### Практическая часть

Граф потока управлений представляет из себя следующие структуры данных:

```csharp
List<BasicBlock> _basicBlocks; // список базовых блоков
List<List<(int vertex, BasicBlock block)>> _children; // списки потомков каждого блока
List<List<(int vertex, BasicBlock block)>> _parents;  // списки предков каждого блока
```

Для определения номера вершины графа по блоку используется соответсвующий словарь:
```
Dictionary<BasicBlock, int> _blockToVertex;
```

Алгорит построения графа заключается в последовательном обходе входного списка базовых блоков, на каждой итерации которого происходит анализ последней инструкция текущего блока, заключающийся в определении ее типа, и последующем определении потомков этого блока.

Определены три типа перехода:

1. Безусловный: ```goto```  
	Осуществляется поиск блока на который происходит переход, который и будет являтся его потомком.
2. Условный: ```ifgoto```  
	Как и в случае безусловного перехода одним из потомков будет являтся блок на который происходит переход, однако, также, потомком будет являться следующий блок в списке, переход на который будет осуществлен в случае невыполнения условия.
3. Последовательный: (отсутствие предыдущих)  
	Случай, в котором единственным потомком является следующий в списке блок.

```csharp
for (int i = 0; i < _basicBlocks.Count; ++i)
{
    var instructions = _basicBlocks[i].GetInstructions();
    var instr = instructions.Last();
    switch (instr.Operation)
    {
        case "goto":
            var gotoOutLabel = instr.Argument1;
            var gotoOutBlock = _basicBlocks.FindIndex(block =>
                    string.Equals(block.GetInstructions().First().Label, gotoOutLabel));

            if (gotoOutBlock == -1)
                throw new Exception($"label {gotoOutLabel} not found");

            _children[i].Add((gotoOutBlock, _basicBlocks[gotoOutBlock]));
            _parents[gotoOutBlock].Add((i, _basicBlocks[i]));
            break;

        case "ifgoto":
            var ifgotoOutLabel = instr.Argument2;
            var ifgotoOutBlock = _basicBlocks.FindIndex(block =>
                    string.Equals(block.GetInstructions().First().Label, ifgotoOutLabel));

            if (ifgotoOutBlock == -1)
                throw new Exception($"label {ifgotoOutLabel} not found");

            _children[i].Add((ifgotoOutBlock, _basicBlocks[ifgotoOutBlock]));
            _parents[ifgotoOutBlock].Add((i, _basicBlocks[i]));

            _children[i].Add((i + 1, _basicBlocks[i + 1]));
            _parents[i + 1].Add((i, _basicBlocks[i]));
            break;

        default:
            if (i < _basicBlocks.Count - 1)
            {
                _children[i].Add((i + 1, _basicBlocks[i + 1]));
                _parents[i + 1].Add((i, _basicBlocks[i]));
            }
            break;
    }
}
```

### Место в общем проекте (Интеграция)
Граф потока управления является одной из фундаметальных частей, структура которого крайне важна для многих оптимизаций.

### Тесты
Тест заключается в тщательной проверке потомков каждого блока в построеном графе потока управления для программы, включающей в себя все типы переходов.

```csharp
var TAC = GenTAC(@"
var a, b, c, d, x, u, e,g, y,zz,i;
goto 200;
200: a = 10 + 5;
for i=2,7 
	x = 1;
if c > a
{
	a = 1;
}
else 
{
    b = 1;
}
");

var blocks = BasicBlockLeader.DivideLeaderToLeader(TAC);
var cfg = new ControlFlowGraph(blocks);

var vertexCount = cfg.GetCurrentBasicBlocks().Count;

Assert.AreEqual(vertexCount, blocks.Count + 2); // standart blocks, in and out
Assert.AreEqual(cfg.GetChildrenBasicBlocks(0).Count, 1); // inblock have 1 child
Assert.AreEqual(cfg.GetParentsBasicBlocks(0).Count, 0);  // inblock not have parents
Assert.AreEqual(cfg.GetChildrenBasicBlocks(vertexCount - 1).Count, 0); // outblock not have childs
Assert.AreEqual(cfg.GetParentsBasicBlocks(vertexCount - 1).Count, 1); // outblock have 1 parent


var graphBlocks = cfg.GetCurrentBasicBlocks();

var vertex1 = cfg.VertexOf(graphBlocks[1]); // goto 200;
Assert.AreEqual(vertex1, 1);
Assert.AreEqual(cfg.GetChildrenBasicBlocks(vertex1).Count, 1);

var vertex2 = cfg.VertexOf(graphBlocks[2]); // 200: a = 10 + 5;
Assert.AreEqual(vertex2, 2);
Assert.AreEqual(cfg.GetChildrenBasicBlocks(vertex2).Count, 1);
//
var vertex3 = cfg.VertexOf(graphBlocks[3]); // for i=2,7
Assert.AreEqual(vertex3, 3);
var children3 = cfg.GetChildrenBasicBlocks(vertex3);
Assert.AreEqual(children3.Count, 2); // for and next block

Assert.AreEqual(children3[0].Item1, 5); // for body
var forBody = children3[0].Item2.GetInstructions();
Assert.AreEqual(forBody[0].ToString(), "L2: x = 1");
Assert.AreEqual(cfg.GetChildrenBasicBlocks(children3[0].Item1).Count, 1); // only goto for

Assert.AreEqual(children3[1].Item1, 4); // next
///
var vertex6 = cfg.VertexOf(graphBlocks[6]); // if
Assert.AreEqual(vertex6, 6);
var children6 = cfg.GetChildrenBasicBlocks(vertex6);
Assert.AreEqual(children6.Count, 2); // 2 ways from if
```
