using System.Collections.Generic;
using SimpleLang.Visitors;
using SimpleParser;

namespace SimpleLang
{
    internal static class ASTOptimizer
    {
        public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
        {
            new OptExprEqualToItself(),
            new OptExprMultDivByOne(),
            new OptExprMultZero(),
            new OptExprSumZero(),
            new OptStatIfTrue(),
            new OptStatIfFalse(),
            new OptExprEqualBoolNum(),
            new OptWhileFalseVisitor(),
            new OptExprSimilarNotEqual(),
            new OptAssignEquality(),
            new IfNullElseNull(),
            new OptExprTransformUnaryToValue(),
            new OptExprFoldUnary(),
            new OptExprAlgebraic(),
            new OptExprSubEqualVar()
        };

        public static void Optimize(Parser parser)
        {
            var optInd = 0;
            do
            {
                parser.root.Visit(Optimizations[optInd]);
                if (Optimizations[optInd].Changed)
                {
                    optInd = 0;
                }
                else
                {
                    ++optInd;
                }
            } while (optInd < Optimizations.Count);
        }
    }
}
