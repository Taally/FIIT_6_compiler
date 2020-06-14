### Pretty printer

#### Постановка задачи
Создать визитор, которой по AST-дереву восстанавливает исходный код программы в отформатированном виде.

#### Команда
А. Татарова, Т. Шкуро

#### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST-дерево

#### Теоретическая часть
Для восстановления кода программы по AST необходимо совершить обход по дереву, сохраняя код в поле визитора Text. Класс визитора PrettyPrintVistor является наследником Visitor. Отступы создаются с помощью переменной Indent, увеличиваемую на 2 при входе в блок и уменьшаемую на 2 перед выходом из него.

#### Практическая часть
Список методов визитора для обхода узлов:
```csharp
void VisitBinOpNode(BinOpNode binop) 
void VisitUnOpNode(UnOpNode unop)
void VisitBoolValNode(BoolValNode b)
void VisitAssignNode(AssignNode a)
void VisitBlockNode(BlockNode bl)
void VisitStListNode(StListNode bl)
void VisitVarListNode(VarListNode v)
void VisitForNode(ForNode f)
void VisitWhileNode(WhileNode w)
void VisitLabelstatementNode(LabelStatementNode l)
void VisitGotoNode(GotoNode g)
void VisitIfElseNode(IfElseNode i)
void VisitExprListNode(ExprListNode e)
void VisitPrintNode(PrintNode p)
void VisitInputNode(InputNode i)
```
Примеры реализации методов визитора:
```csharp
public override void VisitAssignNode(AssignNode a)
{
    Text += IndentStr();
    a.Id.Visit(this);
    Text += " = ";
    a.Expr.Visit(this);
    Text += ";";
}

public override void VisitBlockNode(BlockNode bl)
{
    Text += "{" + Environment.NewLine;
    IndentPlus();
    bl.List.Visit(this);
    IndentMinus();
    Text += Environment.NewLine + IndentStr() + "}";
}
```

#### Место в общем проекте (Интеграция)
Визитор используется после создания парсером AST-дерева:
```csharp
Scanner scanner = new Scanner();
scanner.SetSource(Text, 0);
Parser parser = new Parser(scanner);
var pp = new PrettyPrintVisitor();
parser.root.Visit(pp);
Console.WriteLine(pp.Text);
```

#### Пример работы
Исходный код программы:
```csharp
var a, b, c, d, i; 
a = 5 + 3 - 1;
b = (a - 3) / -b;
if a > b 
{
c = 1;
} else c = 2;
for i=1,5
c = c+1;
d = a <= b;
if c == 6 goto 777;
d = d or a < 10;
777: while c < 25 { 
a = a + 3; 
b = b * 2; 
}
```
Результат работы PrettyPrintVisitor:
```csharp
var a, b, c, d, i;
a = ((5 + 3) - 1);
b = ((a - 3) / (-b));
if (a > b) {
  c = 1;
}
else
  c = 2;
for i = 1, 5
  c = (c + 1);
d = (a <= b);
if (c == 6)
  goto 777;
d = (d or (a < 10));
777: while (c < 25) {
  a = (a + 3);
  b = (b * 2);
}
```
