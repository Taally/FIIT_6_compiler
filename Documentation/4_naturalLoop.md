### Определение всех естественных циклов

#### Постановка задачи
Необходимо реализовать определение всех естественных циклов программы с использованием обратных ребр.

#### Команда
К. Галицкий, А. Черкашин

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* Обратные рёбра и определение того, что CFG является приводимым
* Построение CFG. Обход потомков и обход предков для каждого ББл

Зависимые задачи
* Построение областей

#### Теоретическая часть
В рамках этой задачи необходимо было реализовать определение всех естественных циклов.
Циклы в исходной программе могут определятся различными способами: как циклы for, while или же они могут быть определены с использованием меток и инструкций goto. С точки зрения анализа программ, не имеет значения, как именно выглядят циклы в исходном тексте программы. Важно только то, что они обладают свойствами, допускающими простую их оптимизацию. В данном случае, нас интересует, имеется ли у цикла одна точка входа, если это так, то компилятор в ходе анализа может предпологать выполнение некоторых начальных условий, в начале каждой итерации цикла. Эта возможность служит причиной определения "естественного цикла".

такие циклы обладают двумя важными свойствами:
* Цикл должен иметь единственный входной узел, называемый заголовком.
* Должно существовать обратное ребро, ведущее в заголовок цикла. В противном случае поток управления не сможет вернуться в заголовок непосредственно из "цикла", т.е. даная структура циклом в таком случае не является.

Вход алгоритма построения естественного цикла обратной дуги:
* Граф потока G и обратная дуга n -> d.
Выход:
* Множество loop, состоящее из всех узлов естественного цикла n -> d.


#### Практическая часть
Реализовали метод возвращающий все естественные циклы:
```csharp
public class NaturalLoop
    {
        /// <summary>
        /// Принимает Граф потока данных и по нему ищет все естественные циклы
        /// </summary>
        /// <param name="cfg">Граф потока управления</param>
        /// <returns>
        /// Вернет все натуральные циклы
        /// </returns>
        public static List<List<BasicBlock>> GetAllNaturalLoops(ControlFlowGraph cfg) // принимаем граф потока данных
        {
            var natLoops = new List<List<BasicBlock>>(); // список всех циклов
            var allEdges = new BackEdges(cfg); // получаем обратные ребра графа
            var ForwardEdges = cfg.GetCurrentBasicBlocks(); // получаем вершины графа потока управления

            foreach (var (From, To) in allEdges.BackEdgesFromGraph) // проход по всем обратным ребрам
            {
                if (cfg.VertexOf(To) > 0) // проверка на наличие цикла
                {
                    var tmp = new List<BasicBlock>(); // временный список
                    for (var i = cfg.VertexOf(To); i < cfg.VertexOf(From) + 1; i++)
                    {
                        if (!tmp.Contains(ForwardEdges[i])) // содержит ли список данный ББл
                        {
                            tmp.Add(ForwardEdges[i]);
                        }
                    }

                    natLoops.Add(tmp); // Добавляем все циклы 
                }
            }
            // Возвращаем только те циклы, которые являются естественными
            return natLoops.Where(loop => IsNaturalLoop(loop, cfg)).ToList();
        }
```

Вспомогательный метод для проверки циклов на естественность:
```csharp
/// <summary>
/// Проверка цикла на естественность
/// </summary>
/// <param name="loop">Проверяемый цикл</param>
/// <param name="cfg">Граф потока управления</param>
/// <returns>
/// Вернет флаг, естественен ли он
/// </returns>
private static bool IsNaturalLoop(List<BasicBlock> loop, ControlFlowGraph cfg) // принимает цикл и граф потока управления
{
    for (var i = 1; i < loop.Count; i++)
    {
        var parents = cfg.GetParentsBasicBlocks(cfg.VertexOf(loop[i]));// получаем i ББл данного цикла
        // если кол-во родителей больше 1, значит есть вероятность, что цикл содержит метку с переходом извне
        if (parents.Count > 1)  
        {
            foreach (var parent in parents.Select(x => x.block)) // проверяем каждого родителя
            {   // если родитель не принадлежит текущему циклу, этот цикл не является естественным
                if (!loop.Contains(parent))
                {
                    return false;
                }
            }
        }
    }

    return true;
}
```

Результат работы алгоритма :
```csharp
// Возвращаем только те циклы, которые являются естественными
return natLoops.Where(loop => IsNaturalLoop(loop, cfg)).ToList();
```

#### Место в общем проекте (Интеграция)
Используется для вызова итерационных алгоритмов в единой структуре.
```csharp
            /* ... */
            var natcyc = NaturalLoop.GetAllNaturalLoops(cfg);
           /* ... */
```

#### Тесты
В тестах проверяется определение всех естественных циклов.
```csharp
{
    [TestFixture]
    internal class NaturalLoopTest : TACTestsBase
    {
        [Test]
        public void IntersectLoopsTest()
        {
            var TAC = GenTAC(@"
var a, b;

54: a = 5;
55: a = 6;
b = 6;
goto 54;
goto 55;
");

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            var expected = new List<List<BasicBlock>>()
            {
                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[1], TAC[2], TAC[3] }),
                    new BasicBlock(new List<Instruction>(){ TAC[4] })
                }
            };

            AssertSet(expected, actual);
        }

        [Test]
        public void NestedLoopsTest()
        {
            var TAC = GenTAC(@"
var a, b;

54: a = 5;
55: a = 6;
b = 6;
goto 55;
goto 54;

");

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            var expected = new List<List<BasicBlock>>()
            {
                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[1], TAC[2], TAC[3] })
                },
                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[0] }),
                    new BasicBlock(new List<Instruction>(){ TAC[1], TAC[2], TAC[3] }),
                    new BasicBlock(new List<Instruction>(){ TAC[4] })
                },


            };

            AssertSet(expected, actual);
        }

        [Test]
        public void OneRootLoopsTest()
        {
            var TAC = GenTAC(@"
var a, b;

54: a = 5;
b = 6;
goto 54;
goto 54;

");

            var cfg = new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
            var actual = NaturalLoop.GetAllNaturalLoops(cfg);
            var expected = new List<List<BasicBlock>>()
            {
                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[0], TAC[1], TAC[2] })
                },


                new List<BasicBlock>()
                {
                    new BasicBlock(new List<Instruction>(){ TAC[0], TAC[1], TAC[2] }),
                    new BasicBlock(new List<Instruction>(){ TAC[3] })
                }
            };

            AssertSet(expected, actual);
        }

        private void AssertSet(
            List<List<BasicBlock>> expected,
            List<List<BasicBlock>> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                for (var j = 0; j < expected[i].Count; j++)
                {
                    var e = expected[i][j].GetInstructions();
                    var a = actual[i][j].GetInstructions();

                    Assert.AreEqual(a.Count, e.Count);

                    foreach (var pair in a.Zip(e, (x, y) => (actual: x, expected: y)))
                    {
                        Assert.AreEqual(pair.actual.ToString(), pair.expected.ToString());
                    }
                }
            }
        }
    }
}
```
