%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
BOOL TRUE|FALSE
ID {Alpha}{AlphaDigit}* 

%%

{INTNUM} { 
  return (int)Tokens.INUM; 
}

{REALNUM} { 
  return (int)Tokens.RNUM;
}

{BOOL} {
    return (int)Tokens.BOOL;
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  return res;
}

":=" { return (int)Tokens.ASSIGN; }
";"  { return (int)Tokens.SEMICOLON; }
"," { return (int)Tokens.COMMA; }
">" { return (int)Tokens.GREATER; }
"<" { return (int)Tokens.LESS; }
"==" { return (int)Tokens.EQUAL; }
"!=" { return (int)Tokens.NEQUAL; }
"(" { return (int)Tokens.LPAREN; }
")" { return (int)Tokens.RPAREN; }
"&&" { return (int)Tokens.AND; }
"||" { return (int)Tokens.OR; }
"+" { return (int)Tokens.PLUS; }
"-" { return (int)Tokens.MINUS; }
"*" { return (int)Tokens.MULT; }
"/" { return (int)Tokens.DIV; }
"%" { return (int)Tokens.MOD; }

[^ \r\n] {
	LexError();
	return (int)Tokens.EOF; // ????? ???????
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); // ??????? ??????? (????????????? ??? ???????????????), ???????????? @1 @2 ? ?.?.
%}

%%

public override void yyerror(string format, params object[] args) // ????????? ?????????????? ??????
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): ????????? {2}, ? ????????? {3}", yyline, yycol, args[0], string.Join(" ??? ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
	string errorMsg = string.Format("({0},{1}): ??????????? ?????? {2}", yyline, yycol, yytext);
    throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();
    keywords.Add("begin",(int)Tokens.BEGIN);
    keywords.Add("end",(int)Tokens.END);
    keywords.Add("for", (int)Tokens.FOR);
    keywords.Add("while", (int)Tokens.WHILE);
    keywords.Add("if", (int)Tokens.IF);
    keywords.Add("else", (int)Tokens.ELSE);
    keywords.Add("var", (int)Tokens.VAR);
    keywords.Add("print", (int)Tokens.PRINT);
    keywords.Add("input", (int)Tokens.INPUT);
    keywords.Add("goto", (int)Tokens.GOTO);
  }
  public static int GetIDToken(string s)
  {
    if (keywords.ContainsKey(s.ToLower())) // ???? ?????????????? ? ????????
      return keywords[s];
    else
      return (int)Tokens.ID;
  }
}
