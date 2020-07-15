using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;

namespace SimpleLanguage.Tests.AST
{
    internal class ASTOPtimizerTests : ASTTestsBase
    {
        private string[] ApplyOptimizations(Parser AST, IReadOnlyList<ChangeVisitor> Optimizations = null)
        {
            ASTOptimizer.Optimize(AST, Optimizations);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            return pp.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        [Test]
        public void SumMultZero()
        {
            var AST = BuildAST(@"
var a;
a = (0 + 0) * a;
");
            var optimizations = new List<ChangeVisitor>
            {
                new OptExprMultZero(),
                new OptExprSumZero()
            };

            var result = ApplyOptimizations(AST, optimizations);
            var expected = new[]
            {
                "var a;",
                "a = 0;"
            };

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void SubItselfSumZero()
        {
            var AST = BuildAST(@"
var a;
a = a - ((a - a) + a);
");
            var optimizations = new List<ChangeVisitor>
            {
                new OptExprSumZero(),
                new OptExprSubEqualVar()
            };

            var result = ApplyOptimizations(AST, optimizations);
            var expected = new[]
            {
                "var a;",
                "a = 0;"
            };

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void ExprAndStat()
        {
            var AST = BuildAST(@"
var a;
a = (2 * 3) - 6 + 1 / 1 * a;
if true
    if false
        a = 31;
    else
        a = 42;
else
    a = 21;
");
            var optimizations = new List<ChangeVisitor>
            {
                new OptExprAlgebraic(),
                new OptExprSumZero(),
                new OptExprMultDivByOne(),
                new OptAssignEquality(),
                new OptStatIfTrue(),
                new OptStatIfFalse()

            };

            var result = ApplyOptimizations(AST, optimizations);
            var expected = new[]
            {
                "var a;",
                "a = 42;"
            };

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void ComplexExpr()
        {
            var AST = BuildAST(@"
var a, b;
a = (2 * 3) - 6 + (a * 0 + 1 * a * 1 - a) * b - -21 * 2;
");
            var optimizations = new List<ChangeVisitor>
            {
                new OptExprAlgebraic(),
                new OptExprSumZero(),
                new OptExprSubEqualVar(),
                new OptExprTransformUnaryToValue(),
                new OptExprMultDivByOne(),
                new OptExprMultZero()

            };

            var result = ApplyOptimizations(AST, optimizations);
            var expected = new[]
            {
                "var a, b;",
                "a = 42;"
            };

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
