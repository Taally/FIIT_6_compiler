### BasicBlockLeader
#### Постановка задачи
Реализовать разбиение на базовые блоки от лидера до лидера.
#### Команда
К. Галицкий, А. Черкашин
#### Зависимые и предшествующие задачи
Предшествующие задачи:
* Трехадресный код
* Создание структуры ББл и CFG – графа ББл
Зависимые задачи:
* Def-Use информация и удаление мертвого кода
* Свертка констант
* Учет алгебраических тождеств
* Протяжка констант
* Протяжка копий
* Живые и мертвые перем и удаление мертвого кода
* Построение CFG. Обход потомков и обход предков для каждого базового блока

#### Теоретическая часть
В рамках этой задачи необходимо было реализовать разбиение трехадресного кода на базовые блоки.
Базовый блок – это блок команд от лидера до лидера.
Команды лидеры:
* первая команда
* любая команда, на которую есть переход
* любая команда, непосредственно следующая за переходом

#### Практическая часть
Реализовали создание списка операций лидеров:
```csharp
List<BasicBlock> basicBlockList = new List<BasicBlock>(); // список ББл
List<Instruction> temp = new List<Instruction>(); // временный список, для хранения трёхадресных команд для текущего ББл
List<int> listOfLeaders = new List<int>(); Список лидеров
    for (int i = 0; i < instructions.Count; i++) // формируем список лидеров
    {
        if (i == 0) //Первая команда трехадресного кода
        {
            listOfLeaders.Add(i);
        }

        if (instructions[i].Label != null
            && IsLabelAlive(instructions, instructions[i].Label)) //Команда содержит метку, на которую существует переход
        {
            if (listOfLeaders.Contains(i)) // проверка на наличие данного лидера в списке лидеров
            {
                continue;
            }
            else
            {
                listOfLeaders.Add(i);
            }
        }

        if (instructions[i].Operation == "goto"
            || instructions[i].Operation == "ifgoto") //Команда является следующей после операции перехода (goto или ifgoto)
        {
            if (listOfLeaders.Contains(i + 1)) // проверка на наличие данного лидера в списке лидеров
            {
                continue;
            }
            else
            {
                listOfLeaders.Add(i + 1);
            }
        }
    }
```

Заполнение списка базовых блоков:

```csharp
int j = 0;
for (int i = 0; i < instructions.Count; i++) // заполняем BasicBlock
{   //Заполняем временный список
    temp.Add(new Instruction(instructions[i].Label,
                                instructions[i].Operation,
                                instructions[i].Argument1,
                                instructions[i].Argument2,
                                instructions[i].Result));

    if (i + 1 >= instructions.Count
        || i == listOfLeaders[((j + 1) >= listOfLeaders.Count ? j : j + 1)] - 1) // Следующая команда в списке принадлежит другому лидеру или последняя команда трехадресного
    {
        basicBlockList.Add(new BasicBlock(temp)); //Добавляем ББл
        temp = new List<Instruction>(); //Создаем новый пусток список
        j++;
    }
}
```

Результатом работы является список базовых блоков, состоящий из команд трехадресного кода разбитых от лидера до лидера:
```csharp
	return basicBlockList;
```

#### Место в общем проекте (Интеграция)
Используется после создания трехадресного кода. Необходим для разбиение трехадресного кода на базовые блоки.
```csharp
/* Main.cs */
var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
parser.root.Visit(threeAddrCodeVisitor);
var threeAddressCode = threeAddrCodeVisitor.Instructions;
var optResult = ThreeAddressCodeOptimizer.OptimizeAll(threeAddressCode);
var divResult = BasicBlockLeader.DivideLeaderToLeader(optResult);
```
#### Пример работы
Исходный код программы:
```csharp
1: b = True
100: a = True
101: #t1 = !True
if #t1 goto 201
a = b
L2: goto 201
#t2 = !a
if #t2 goto L4
c = 18
L4: a = True
goto 201
201: c = 6
```
Результат работы разбиение на базовые блоки:
```csharp
ББл №1
1: b = True
100: a = True
101: #t1 = !True
if #t1 goto 201

ББл №2
a = b
L2: goto 201

ББл №3
#t2 = !a
if #t2 goto L4

ББл №4
c = 18

ББл №5
L4: a = True
goto 201

ББл №6
201: c = 6
```
