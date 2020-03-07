%{
// ��� ���������� ����������� � ����� GPPGParser, �������������� ����� ������, ������������ �������� gppg
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token BEGIN END INUM RNUM ID ASSIGN SEMICOLON COMMA FOR
OR AND EQUAL NEQUAL GREATER LESS LPAREN RPAREN WHILE BOOL
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
		| ident ASSIGN exprb
		;

label	: INUM
		;

goto	: GOTO label
		;

for 	: FOR ident ASSIGN INUM COMMA INUM statement
		;

while 	: WHILE exprb statement
		;

expr	: ident  
		| INUM
		| LPAREN expr RPAREN
		| expr DIV expr
		| expr MOD expr
		| expr MULT expr
		| expr MINUS expr
		| expr PLUS expr
		;

exprb   : BOOL
		| exprb OR exprb
	    | exprb AND exprb
		| LPAREN exprb RPAREN
		| INUM LESS INUM
	    | INUM GREATER INUM
	    | INUM EQUAL INUM
	    | INUM NEQUAL INUM
		;

print    : PRINT LPAREN exprlist RPAREN
		 ;

input    : INPUT LPAREN ident RPAREN
		 ;

exprlist : expr
		 | exprlist COMMA expr
		 ;

if 		: IF exprb block ELSE block
		| IF exprb block
		;

varlist	: ident
		| varlist COMMA ident
		;

var		: VAR varlist
		;

%%
