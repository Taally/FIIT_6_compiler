%{
    public StListNode root;
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
    private bool InDefSect = false;
%}

%output = SimpleYacc.cs

%union {
            public bool bVal;
            public int iVal;
            public string sVal;
            public Node nVal;
            public ExprNode eVal;
            public StatementNode stVal;
            public StListNode blVal;
       }

%using System.IO;
%using ProgramTree;

%namespace SimpleParser

%token BEGIN END ASSIGN SEMICOLON FOR COMMA COLON LPAR RPAR WHILE IF ELSE INPUT PRINT
VAR OR AND EQUAL NOTEQUAL LESS GREATER EQGREATER EQLESS GOTO PLUS MINUS MULT DIV NOT
%token <iVal> INUM
%token <bVal> BOOL
%token <sVal> ID

%type <eVal> expr ident A B C E T F exprlist
%type <stVal> assign statement for while if input print varlist var labelstatement goto block
%type <blVal> stlist progr

%%

progr   : stlist { root = $1; }
        ;

stlist	: statement { $$ = new StListNode($1); }
        | stlist statement
            {
                $1.Add($2);
                $$ = $1;
            }
        ;

statement: assign SEMICOLON { $$ = $1; }
        | for { $$ = $1; }
        | while { $$ = $1; }
        | if { $$ = $1; }
        | block { $$ = $1; }
        | input SEMICOLON { $$ = $1; }
        | print SEMICOLON { $$ = $1; }
        | var SEMICOLON { $$ = $1; }
        | goto SEMICOLON { $$ = $1; }
        | labelstatement { $$ = $1; }
        ;

ident 	: ID {
            if (!InDefSect)
                if (!SymbolTable.vars.ContainsKey($1))
                    throw new Exception("("+@1.StartLine+","+@1.StartColumn+"): Variable "+$1+" not described");
            $$ = new IdNode($1);
        }
        ;

assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
        ;

block	: BEGIN stlist END { $$ = new BlockNode($2); }
        ;

for		: FOR ident ASSIGN expr COMMA expr statement
        { $$ = new ForNode($2 as IdNode, $4, $6, $7); }
        ;

while	: WHILE expr statement { $$ = new WhileNode($2, $3); }
        ;

if		: IF expr statement ELSE statement { $$ = new IfElseNode($2, $3, $5); }
        | IF expr statement { $$ = new IfElseNode($2, $3); }
        ;

expr	: expr OR A { $$ = new BinOpNode($1, $3, OpType.OR); }
        | A { $$ = $1; }
        ;

A		: A AND B { $$ = new BinOpNode($1, $3, OpType.AND); }
        | B { $$ = $1; }
        ;

B		: B EQUAL C { $$ = new BinOpNode($1, $3, OpType.EQUAL); }
        | B NOTEQUAL C { $$ = new BinOpNode($1, $3, OpType.NOTEQUAL); }
        | C { $$ = $1; }
        ;

C		: C GREATER E { $$ = new BinOpNode($1, $3, OpType.GREATER); }
        | C LESS E { $$ = new BinOpNode($1, $3, OpType.LESS); }
        | C EQGREATER E { $$ = new BinOpNode($1, $3, OpType.EQGREATER); }
        | C EQLESS E { $$ = new BinOpNode($1, $3, OpType.EQLESS); }
        | E { $$ = $1; }
        ;

E		: E PLUS T  { $$ = new BinOpNode($1, $3, OpType.PLUS); }
        | E MINUS T { $$ = new BinOpNode($1, $3, OpType.MINUS); }
        | T { $$ = $1; }
        ;

T		: T MULT F { $$ = new BinOpNode($1, $3, OpType.MULT); }
        | T DIV F { $$ = new BinOpNode($1, $3, OpType.DIV); }
        | F { $$ = $1; }
        ;

F		: ident { $$ = $1 as IdNode; }
        | INUM { $$ = new IntNumNode($1); }
        | LPAR expr RPAR { $$ = $2; }
        | BOOL { $$ = new BoolValNode($1); }
        | MINUS F { $$ = new UnOpNode($2, OpType.UNMINUS); }
        | NOT F { $$ = new UnOpNode($2, OpType.NOT);}
        ;

input	: INPUT LPAR ident RPAR { $$ = new InputNode($3 as IdNode); }
        ;

exprlist : expr { $$ = new ExprListNode($1); }
        | exprlist COMMA expr
        {
            ($1 as ExprListNode).Add($3);
            $$ = $1;
        }
        ;

print	: PRINT LPAR exprlist RPAR { $$ = new PrintNode($3 as ExprListNode); }
        ;

varlist	: ident { $$ = new VarListNode($1 as IdNode); }
        | varlist COMMA ident
        {
            ($1 as VarListNode).Add($3 as IdNode);
            $$ = $1;
        }
        ;

var		: VAR { InDefSect = true; } varlist
            {
                foreach (var v in ($3 as VarListNode).vars)
                    SymbolTable.NewVarDef(v.Name);
                InDefSect = false;
            }
        ;

goto	: GOTO INUM { $$ = new GotoNode($2); }
        ;

labelstatement	: INUM COLON statement { $$ = new LabelStatementNode($1, $3); }
        ;

%%
