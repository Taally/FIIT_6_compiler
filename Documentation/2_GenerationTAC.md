## Генерация трёхадресного кода

### Постановка задачи

Реализовать генерацию трёхадресного кода для всех инструкций языка

### Команда

Д. Володин, А. Татарова, Т. Шкуро

### Зависимые и предшествующие задачи

Предшествующие:

- Построение АСТ
- Базовые визиторы

Зависимые: 

- Разбиение на базовые блоки

### Теоретическая часть

__Трёхадресный код__ (ТАК) — это линеаризованное абстрактное синтаксическое дерево, из которого восстановить текст программы уже нельзя. В трёхадресном коде в правой части выражении допускается только один оператор, т.е. выражение ```x+y*z``` транслируется как

```
t1 = y * z
t2 = x + t1
```
где ```t1```,```t2``` – временные переменные.

На примере ниже можно увидеть разбор АСТ узлов, соответствующих выражению ```a = a + b * c```

![Пример трёхадресного кода](2_GenerationTAC/pic1.jpg)

Представление трёхадресного кода является четвёркой полей 
(op, arg1, arg2, res). На рисунке ниже показано, как разбирается выражение ```a = b * (-c) + b * (-c)``` в виде трёхадресного кода и представляется в таблице четвёрками:

![Пример четвёрок трёхадресного кода](2_GenerationTAC/pic2.jpg)

Для хранения меток перехода добавляется ещё одно поле Label, и тогда транслируемые инструкции становятся пятёрками полей. 

### Практическая часть

Для транслирования АСТ в трёхадресный код создан класс Instruction, в котором хранится пятёрка полей 

```csharp
public string Label { get; internal set; }
public string Operation { get; }
public string Argument1 { get; }
public string Argument2 { get; }
public string Result { get; }
```
Генератор трёхадресного кода представляет собой визитор, обходящий все узлы и генерирующий определённые инструкции в зависимости от типа узла:
- для выражений
```csharp
private string Gen(ExprNode ex)
{
    if (ex is BinOpNode binOp)
    {
        var argument1 = Gen(binOp.Left);
        var argument2 = Gen(binOp.Right);
        var result = ThreeAddressCodeTmp.GenTmpName();
        GenCommand("", binOp.Op.ToString(), argument1, argument2, result);
        return result;
    }
    else if (ex is UnOpNode unOp)
    {
        /*..*/
    }
    else if (ex is IdNode id)
    {
        return id.Name;
    }
    else if (ex is IntNumNode intNum)
    {
        return intNum.Num.ToString();
    }
    else if (ex is BoolValNode bl)
    {
        return bl.Val.ToString();
    }

    return null;
}
```
- для оператора присваивания
```csharp
public override void VisitAssignNode(AssignNode a)
{
    var argument1 = Gen(a.Expr);
    GenCommand("", "assign", argument1, "", a.Id.Name);
}
```
- для условного оператора
```csharp
public override void VisitIfElseNode(IfElseNode i)
{
    // перевод в трёхадресный код условия
    var exprTmpName = Gen(i.Expr);

    var trueLabel = i.TrueStat is LabelStatementNode label
        ? label.Label.Num.ToString()
        : i.TrueStat is BlockNode block
            && block.List.StatChildren[0] is LabelStatementNode labelB
            ? labelB.Label.Num.ToString()
            : ThreeAddressCodeTmp.GenTmpLabel();

    var falseLabel = ThreeAddressCodeTmp.GenTmpLabel();
    GenCommand("", "ifgoto", exprTmpName, trueLabel, "");

    // перевод в трёхадресный код false ветки
    i.FalseStat?.Visit(this);
    GenCommand("", "goto", falseLabel, "", "");

    // перевод в трёхадресный код true ветки
    var instructionIndex = Instructions.Count;
    i.TrueStat.Visit(this);
    Instructions[instructionIndex].Label = trueLabel;

    GenCommand(falseLabel, "noop", "", "", "");
}
```
- для цикла while
```csharp
 public override void VisitWhileNode(WhileNode w)
{
    var exprTmpName = Gen(w.Expr);
    var whileHeadLabel = ThreeAddressCodeTmp.GenTmpLabel();
    var whileBodyLabel = w.Stat is LabelStatementNode label
        ? label.Label.Num.ToString()
        : w.Stat is BlockNode block
                        && block.List.StatChildren[0] is LabelStatementNode labelB
            ? labelB.Label.Num.ToString()
            : ThreeAddressCodeTmp.GenTmpLabel();

    var exitLabel = ThreeAddressCodeTmp.GenTmpLabel();

    Instructions[Instructions.Count - 1].Label = whileHeadLabel;

    GenCommand("", "ifgoto", exprTmpName, whileBodyLabel, "");
    GenCommand("", "goto", exitLabel, "", "");

    var instructionIndex = Instructions.Count;
    w.Stat.Visit(this);
    Instructions[instructionIndex].Label = whileBodyLabel;
    GenCommand("", "goto", whileHeadLabel, "", "");
    GenCommand(exitLabel, "noop", "", "", "");
}
```
- для цикла for (необходимо отметить: здесь делается допущение, что for шагает на +1 до границы, не включая её)
```csharp
 public override void VisitForNode(ForNode f)
{
    var Id = f.Id.Name;
    var forHeadLabel = ThreeAddressCodeTmp.GenTmpLabel();
    var exitLabel = ThreeAddressCodeTmp.GenTmpLabel();

    var fromTmpName = Gen(f.From);
    GenCommand("", "assign", fromTmpName, "", Id);

    var toTmpName = Gen(f.To);
    // Делаем допущение, что for шагает на +1 до границы, не включая её
    var condTmpName = ThreeAddressCodeTmp.GenTmpName();
    GenCommand(forHeadLabel, "EQGREATER", Id, toTmpName, condTmpName);
    GenCommand("", "ifgoto", condTmpName, exitLabel, "");

    f.Stat.Visit(this);

    GenCommand("", "PLUS", Id, "1", Id);
    GenCommand("", "goto", forHeadLabel, "", "");
    GenCommand(exitLabel, "noop", "", "", "");
}
```
- для input и print
```csharp
public override void VisitInputNode(InputNode i) => GenCommand("", "input", "", "", i.Ident.Name);
public override void VisitPrintNode(PrintNode p)
{
    foreach (var x in p.ExprList.ExprChildren)
    {
        var exprTmpName = Gen(x);
        GenCommand("", "print", exprTmpName, "", "");
    }
}
```

- для goto и узла метки перехода
```csharp
public override void VisitGotoNode(GotoNode g) => GenCommand("", "goto", g.Label.Num.ToString(), "", "");
public override void VisitLabelstatementNode(LabelStatementNode l)
{
    var instructionIndex = Instructions.Count;
    // Чтобы не затиралась временная метка у while
    if (l.Stat is WhileNode)
    {
        GenCommand("", "noop", "", "", "");
    }
    l.Stat.Visit(this);
    Instructions[instructionIndex].Label = l.Label.Num.ToString();
}
```
- для пустого оператора
```csharp
public override void VisitEmptyNode(EmptyNode w) => GenCommand("", "noop", "", "", "");
```
где ```GenCommand``` --- функция, создающая инструкцию с заданной пятеркой полей.

### Место в общем проекте (Интеграция)

Генерация трёхадресного кода происходит после построения АСТ дерева и применения оптимизаций по нему, после генерации происходит разбиение трёхадресного кода на блоки.
```csharp
ASTOptimizer.Optimize(parser);
/*..*/

var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
parser.root.Visit(threeAddrCodeVisitor);
var threeAddressCode = threeAddrCodeVisitor.Instructions;
/*..*/
```

### Тесты

- АСТ дерево после оптимизаций
```
var a, b, c, d, x, zz, i;
goto 777;
777: while ((x < 25) or (a > 100)) {
  x = (x + 1);
  x = (x * 2);
}
for i = 2, 7
  x = (x + 1);
zz = (((a * (b + 1)) / c) - (b * a));
input(zz);
print(zz, a, b);
if (c > a) {
  a = c;
  a = 1;
}
else {
  b = 1;
  a = b;
}
```

- Сгенерированный трёхадресный код
```
goto 777
777: noop
#t1 = x < 25
#t2 = a > 100
L1: #t3 = #t1 or #t2
if #t3 goto L2
goto L3
L2: #t4 = x + 1
x = #t4
#t5 = x * 2
x = #t5
goto L1
L3: noop
i = 2
L4: #t6 = i >= 7
if #t6 goto L5
#t7 = x + 1
x = #t7
i = i + 1
goto L4
L5: noop
#t8 = b + 1
#t9 = a * #t8
#t10 = #t9 / c
#t11 = b * a
#t12 = #t10 - #t11
zz = #t12
input zz
print zz
print a
print b
#t13 = c > a
if #t13 goto L6
b = 1
a = b
goto L7
L6: a = c
a = 1
L7: noop
```
