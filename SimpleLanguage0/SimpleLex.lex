%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
ID {Alpha}{AlphaDigit}* 
BOOL TRUE|FALSE

%%

{INTNUM} { 
  return (int)Tokens.INUM; 
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  return res;
}

"=" { return (int)Tokens.ASSIGN; }
";"  { return (int)Tokens.SEMICOLON; }
"{"  { return (int)Tokens.BEGIN; }
"}"  { return (int)Tokens.END; }
","  { return (int)Tokens.COMMA; }
":"  { return (int)Tokens.COLON; }



[^ \r\n] {
	LexError();
	return (int)Tokens.EOF; // конец разбора
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); // позици€ символа (терминального или нетерминального), возвращаема€ @1 @2 и т.д.
%}

%%

public override void yyerror(string format, params object[] args) // обработка синтаксических ошибок
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): ¬стречено {2}, а ожидалось {3}", yyline, yycol, args[0], string.Join(" или ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
	string errorMsg = string.Format("({0},{1}): Ќеизвестный символ {2}", yyline, yycol, yytext);
    throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();
	keywords.Add("for", (int)Tokens.FOR);
	keywords.Add("while", (int)Tokens.WHILE);
	keywords.Add("var", (int)Tokens.VAR);
	keywords.Add("if", (int)Tokens.VAR);
	keywords.Add("else", (int)Tokens.VAR);


  }
  public static int GetIDToken(string s)
  {
    if (keywords.ContainsKey(s.ToLower())) // €зык нечувствителен к регистру
      return keywords[s];
    else
      return (int)Tokens.ID;
  }
}
