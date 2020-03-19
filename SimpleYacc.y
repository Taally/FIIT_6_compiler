%{
// Ёти объ€влени€ добавл€ютс€ в класс GPPGParser, представл€ющий собой парсер, генерируемый системой gppg
    public BlockNode root; //  орневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
	private bool InDefSect = false;
%}

%output = SimpleYacc.cs

%union { 
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public bool bVal;
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END VAR FOR WHILE ASSIGN SEMICOLON COMMA IF ELSE PRINT INPUT LPAR RPAR OR AND EQUAL NEQUAL PLUS MINUS MULT DIV MOD GREATER LESS EQGREATER EQLESS GOTO
%token <iVal> INUM 
%token <dVal> RNUM 
%token <bVal> BOOL 

%token <sVal> ID

%type <eVal> ident exprlist expr A B C D E F
%type <stVal> assign statement while for if print input var varlist goto label empty
%type <blVal> stlist block

%%

progr   : block { root = $1; }
		;

stlist	: statement 
			{ 
				$$ = new BlockNode($1); 
			}
		| stlist SEMICOLON statement 
			{ 
				$1.Add($3); 
				$$ = $1; 
			}
		;

statement: assign	{ $$ = $1; }
		| block		{ $$ = $1; }
		| while		{ $$ = $1; }
		| for		{ $$ = $1; }
		| if		{ $$ = $1; }
		| print		{ $$ = $1; }
		| input		{ $$ = $1; }
		| var		{ $$ = $1; }
		| goto		{ $$ = $1; }
		| label		{ $$ = $1; }
		| empty		{ $$ = $1; }
	;

empty	: { $$ = new EmptyNode(); }
		;

ident 	: ID
			{
				if (!InDefSect)
					if (!SymbolTable.vars.ContainsKey($1))
						throw new Exception("("+@1.StartLine+","+@1.StartColumn+"): ѕеременна€ "+$1+" не описана");
				$$ = new IdNode($1); 
			}	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr	: expr OR A { $$ = new BinOpNode($1,$3,"or"); }
		| A			{ $$ = $1; }
		;

A	: A AND B		{ $$ = new BinOpNode($1,$3,"and"); }
	| B				{ $$ = $1;}
	;

B	: B EQUAL C		{ $$ = new BinOpNode($1,$3,"=="); }
	| B NEQUAL C	{ $$ = new BinOpNode($1,$3,"!="); }
	| C				{ $$ = $1; }
	;

C	: C GREATER D	{ $$ = new BinOpNode($1,$3,">"); }
	| C LESS D		{ $$ = new BinOpNode($1,$3,"<"); }
	| C EQGREATER D	{ $$ = new BinOpNode($1,$3,">="); }
	| C EQLESS D	{ $$ = new BinOpNode($1,$3,"<="); }
	| D				{ $$ = $1; }
	;

D	: D PLUS E		{ $$ = new BinOpNode($1,$3,"+"); }
	| D MINUS E		{ $$ = new BinOpNode($1,$3,"-"); }
	| E				{ $$ = $1; }
	;

E	: E MULT F		{ $$ = new BinOpNode($1,$3,"*"); }
	| E DIV F		{ $$ = new BinOpNode($1,$3,"/"); }
	| E MOD F		{ $$ = new BinOpNode($1,$3,"%"); }
	| F				{ $$ = $1; }
	;

F	: ident			{ $$ = $1 as IdNode; }
	| INUM			{ $$ = new IntNumNode($1); }
	| BOOL			{ $$ = new BoolValNode($1); }
	| LPAR expr RPAR { $$ = $2; }
	;

block	: BEGIN stlist END { $$ = $2; }
		;

while	: WHILE expr statement { $$ = new WhileNode($2, $3); }
		;

for 	: FOR ident ASSIGN INUM COMMA INUM statement 
			{ 
				$$ = new ForNode($2 as IdNode, $4, $6, $7); 
			}
		;

if 		: IF expr statement ELSE statement	{ $$ = new IfElseNode($2, $3, $5); }
		| IF expr statement					{ $$ = new IfElseNode($2, $3); }
		;

print    : PRINT LPAR exprlist RPAR	{ $$ = new PrintNode($3 as ExprListNode); }
		 ;

input    : INPUT LPAR ident RPAR	{ $$ = new InputNode($3 as IdNode); }
		 ;

exprlist : expr	
			{ 
				$$ = new ExprListNode($1 as ExprNode); 
			}
		 | exprlist COMMA expr
			{
				($1 as ExprListNode).Add($3 as ExprNode);
				$$ = $1;
			}
		 ;

varlist	: ident
			{ 
				$$ = new VarListNode($1 as IdNode); 
			}
		| varlist COMMA ident
			{
				($1 as VarListNode).Add($3 as IdNode);
				$$ = $1;
			}
		;

var		: VAR { InDefSect = true; } varlist
			{ 
				foreach (var v in ($3 as VarListNode).vars)
					SymbolTable.NewVarDef(v.Name, type.tint);
				InDefSect = false;	
			}
		;

goto	: GOTO label
		;

label	: INUM
		;
	
%%

