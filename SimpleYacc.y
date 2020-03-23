%{
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token BEGIN END INUM RNUM ID ASSIGN SEMICOLON COMMA FOR PLUS MINUS MULT DIV LPAR RPAR WHILE IF ELSE INPUT PRINT
VAR OR AND EQUAL NOTEQUAL LESS GREATER EQGREATER EQLESS GOTO COLON BOOL

%%

progr   : stlist
		;

stlist	: statement 
		| stlist statement 
		;

statement: assign 
		| block
		| for
		| while
		| if
		| input
		| print
		| var
		| label
		| goto
		;

ident 	: ID 
		;
	
assign 	: ident ASSIGN expr SEMICOLON
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

input	: INPUT LPAR ident RPAR SEMICOLON
		;

exprlist : expr
		| exprlist COMMA expr
		;

print	: PRINT LPAR exprlist RPAR SEMICOLON
		;

varlist	: ident
		| varlist COMMA ident
		;

var		: VAR varlist SEMICOLON
		;

goto	: GOTO INUM SEMICOLON
		;

label	: INUM COLON statement
		;
%%