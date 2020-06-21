using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;
using SimpleScanner;

namespace IDEForSimpleLang1
{
    class Controller
    {
        internal static Parser GetParser(string sourceCode)
        {
            SymbolTable.vars.Clear();
            var scanner = new Scanner();
            scanner.SetSource(sourceCode, 0);
            var parser = new Parser(scanner);

            return parser; 
        }

        private static List<ChangeVisitor> ASTOptimizations = new List<ChangeVisitor>{
            new OptExprAlgebraic(),
            new OptExprEqualBoolNum(),
            new OptExprFoldUnary(),
            new OptExprMultDivByOne(),
            new OptExprMultZero(),
            new OptExprSimilarNotEqual(),
            new OptExprSubEqualVar(),
            new OptExprSumZero(),
            new OptExprTransformUnaryToValue(),
            new OptExprVarEqualToItself(),
            new OptExprWithOperationsBetweenConsts(),
            new IfNullElseNull(),
            new OptAssignEquality(),
            new OptStatIfFalse(),
            new OptStatIfTrue(),
            new OptWhileFalseVisitor()
        };

        internal static string GetASTWithOpt(Parser parser, List<int> lstCheck)
        {
            var b = parser.Parse();
            if (!b)
            {
                return "Error. AST not build";
            }

            var fillParents = new FillParentsVisitor();
            parser.root.Visit(fillParents);
            var listOpt = new List<ChangeVisitor>();
            if (lstCheck.Count > 0) {
                foreach (var n in lstCheck)
                {
                    listOpt.Add(ASTOptimizations[n]);
                }
                ASTOptimizer.Optimize(parser, listOpt);
            }
            
            var pp = new PrettyPrintVisitor();
            parser.root.Visit(pp);
            return pp.Text;
        }

        internal static string GetTACWithOpt(Parser parser)
        {
            //Где-то здесь список оптимизаций
            ThreeAddressCodeTmp.ResetTmpLabel();
            ThreeAddressCodeTmp.ResetTmpName();
            var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
            parser.root.Visit(threeAddrCodeVisitor);
            var threeAddressCode = threeAddrCodeVisitor.Instructions;
            StringBuilder str = new StringBuilder();
            foreach (var x in threeAddressCode)
            {
                str.Append(x);
                str.Append(Environment.NewLine);
            }

            return str.ToString();
        }
    }
}
