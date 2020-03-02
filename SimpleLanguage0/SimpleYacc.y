%{
// Ёти объ€влени€ добавл€ютс€ в класс GPPGParser, представл€ющий собой парсер, генерируемый системой gppg
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token BEGIN END INUM BNUM ID ASSIGN SEMICOLON FOR COMMA COLON WHILE VAR IF ELSE
%%

progr   : block
		;

stlist	: statement 
		| stlist SEMICOLON statement 
		;

statement: assign
		| block  
		| for
		| while
		| var
		| if
		;

ident 	: ID 
		;
	
assign 	: ident ASSIGN expr 
		;

expr	: ident  
		| INUM 
		;

block	: BEGIN stlist END 
		;

for	: FOR ident ASSIGN INUM COMMA INUM statement 
		;

while : WHILE expr statement
	;

varlist : ident
		| varlist COMMA ident
		;
		
var : VAR varlist
	;

if : IF expr statement ELSE statement
	| IF expr statement
	;
%%
