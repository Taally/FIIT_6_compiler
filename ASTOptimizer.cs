using SimpleLang.Visitors;
using SimpleParser;
using System.Collections.Generic;

namespace SimpleLang
{
    static class ASTOptimizer
    {
        public static List<ChangeVisitor> Optimizations { get; } = new List<ChangeVisitor>
        {
            new OptExprEqualToItself(),
            new OptExprMultDivByOne(),
            new OptStatIfTrue(),
            new OptStatIfFalse(),
            new OptExprEqualBoolNumId(),
            new OptWhileFalseVisitor(),
            new OptExprSimilarNotEqual(),
            new OptAssignEquality(),
            new IfNullElseNull()
        };

        public static void Optimize(Parser parser)
        {
            int optInd = 0;
            do
            {
                parser.root.Visit(Optimizations[optInd]);
                if (Optimizations[optInd].Changed)
                    optInd = 0;
                else
                    ++optInd;
            } while (optInd < Optimizations.Count);
        }
    }
}
