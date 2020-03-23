%{
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
    public Node root;
%}

%output = SimpleYacc.cs

%using ProgramTree

%namespace SimpleParser

%union { 
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
       }


%token BEGIN END ASSIGN SEMICOLON COMMA FOR PLUS MINUS MULT DIV LPAR RPAR WHILE IF ELSE INPUT PRINT
%token VAR OR AND EQUAL NOTEQUAL LESS GREATER EQGREATER EQLESS GOTO COLON BOOL

%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident A B C E T F
%type <stVal> assign statement for while if input print var labelstatement goto
%type <blVal> stlist block progr


%%

progr   : stlist { root = $1; }
		;

stlist	: statement 
		{ 
			$$ = new BlockNode($1); 
		}
		| stlist statement 
		{
			$1.Add($2); 
			$$ = $1;
		} 
		;

statement: assign SEMICOLON { $$ = $1; }
		| block { $$ = $1; }
		| for { $$ = $1; }
		| while { $$ = $1; }
		| if { $$ = $1; }
		| input SEMICOLON { $$ = $1; }
		| print SEMICOLON { $$ = $1; }
		| var SEMICOLON { $$ = $1; }
		| labelstatement { $$ = $1; }
		| goto SEMICOLON { $$ = $1; }
		;

ident 	: ID { $$ = new IdNode($1); }
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr	: expr OR A
		| A
		;

A		: A AND B
		| B
		;

B		: B EQUAL C
		| B NOTEQUAL C
		| C
		;

C		: C GREATER E
		| C LESS E
		| C EQGREATER E
		| C EQLESS E
		| E
		;

E		: E PLUS T  
		| E MINUS T
		| T
		;

T		: T MULT F
		| T DIV F
		| F
		;

F		: ident
		| INUM
		| LPAR expr RPAR
		| BOOL
		;

block	: BEGIN stlist END 
		;

for		: FOR ident ASSIGN INUM COMMA INUM statement
		;

while	: WHILE expr statement
		;

if		: IF expr statement ELSE statement
		| IF expr statement
		;

input	: INPUT LPAR ident RPAR
		;

exprlist : expr
		| exprlist COMMA expr
		;

print	: PRINT LPAR exprlist RPAR
		;

varlist	: ident
		| varlist COMMA ident
		;

var		: VAR varlist
		;

goto	: GOTO INUM
		;

labelstatement	: INUM COLON statement
		;
%%