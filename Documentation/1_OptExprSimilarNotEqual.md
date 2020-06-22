## AST-оптимизация замены сравнения переменной с собой на булевскую константу false

### Постановка задачи
Реализовать оптимизацию по AST дереву вида (a > a, a != a ) = False

### Команда
К. Галицкий, А. Черкашин

### Зависимые и предшествующие задачи
Предшествующие задачи:
* AST дерево

### Теоретическая часть
Реализовать оптимизацию по AST дереву вида (a > a, a != a ) = False:
  * До
  ```csharp
  #t1 = a > a;
  ```
  * После
  ```csharp
  #t1 = False;
  ```
  * До
  ```csharp
  #t1 = a != a;
  ```
  * После
  ```csharp
  #t1 = False;
  ```

### Практическая часть
Если в выражениях вида ">", "<", "!=" операции сравнения проводятся с одной и той же переменной, необходимо заменить сравнение на результат "False".
Пример реализации метода:

```csharp
public override void VisitBinOpNode(BinOpNode binop)
{
	if (
		binop.Left is IntNumNode iLeft && binop.Right is IntNumNode iRight && iLeft.Num == iRight.Num 
		&& (binop.Op == OpType.GREATER || binop.Op == OpType.LESS || binop.Op == OpType.NOTEQUAL)
		|| binop.Left is IdNode idLeft && binop.Right is IdNode idRight && idLeft.Name == idRight.Name 
		&& (binop.Op == OpType.GREATER || binop.Op == OpType.LESS || binop.Op == OpType.NOTEQUAL)
		)
	{
		ReplaceExpr(binop, new BoolValNode(false));
	}
	else
	{
		base.VisitBinOpNode(binop);
	}

}
```

### Место в общем проекте (Интеграция)
Данная оптимизация выполняется на AST-дереве, построенном для данной программы. Применяется в классе `ASTOptimizer`.
```csharp
public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
       {
             /* ... */
           new OptExprSimilarNotEqual(),
             /* ... */
       };
```

### Тесты
В тестах проверяется работоспособность оптимизации и соответствие результатов:
```csharp
public void SimilarNotEqualTest()
{
	var AST = BuildAST(@"
var a, b, d, k, c;
c = a>a;
b = k<k;
d = a != a;
d = 1 > 1;
");

	var expected = new[] {
		"var a, b, d, k, c;",
		"c = false;",
		"b = false;",
		"d = false;",
		"d = false;",
	};

	var result = ApplyOpt(AST, new OptExprSimilarNotEqual());
	CollectionAssert.AreEqual(expected, result);
}
```
