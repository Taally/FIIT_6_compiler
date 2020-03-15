%{
// ��� ���������� ����������� � ����� GPPGParser, �������������� ����� ������, ������������ �������� gppg
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token BEGIN END INUM ID ASSIGN SEMICOLON COMMA FOR
OR AND EQUAL NEQUAL GREATER LESS EQGREATER EQLESS LPAR RPAR WHILE BOOL
IF ELSE
PLUS MINUS MULT DIV MOD
VAR PRINT INPUT GOTO

%%

progr   : block
		;

block	: BEGIN stlist END
		;

stlist	: statement 
		| stlist SEMICOLON statement 
		;

statement 	: assign
			| block  
			| for	
			| while
			| if
			| var
			| print
			| input
			| goto
			| label
			;

ident 	: ID 
		;
	
assign 	: ident ASSIGN expr 
		| ident ASSIGN expr
		;

label	: INUM
		;

goto	: GOTO label
		;

for 	: FOR ident ASSIGN INUM COMMA INUM statement
		;

while 	: WHILE expr statement
		;

expr	: expr OR A
		| A
		;

A	: A AND B
	| B
	;

B	: B EQUAL C
	| B NEQUAL C
	| C
	;

C	: C GREATER D
	| C LESS D
	| C EQGREATER D
	| C EQLESS D
	| D
	;

D	: D PLUS E
	| D MINUS E
	| E
	;

E	: E MULT F
	| E DIV F
	| E MOD F
	| F
	;

F	: ident
	| INUM
	| BOOL
	| LPAR expr RPAR
	;

print    : PRINT LPAR exprlist RPAR
		 ;

input    : INPUT LPAR ident RPAR
		 ;

exprlist : expr
		 | exprlist COMMA expr
		 ;

if 		: IF expr block ELSE block
		| IF expr block
		;

varlist	: ident
		| varlist COMMA ident
		;

var		: VAR varlist
		;

%%
