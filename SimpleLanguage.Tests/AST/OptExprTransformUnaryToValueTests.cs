using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang.Visitors;
using ProgramTree;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprTransformUnaryToValueTests : ASTTestsBase
    {
        [Test]
        public void TransformToIntTest() {
            var AST = BuildAST(@"
var a, b;
a = (-1);
");
            var expected = new[] {
                "var a, b;",
                "a = -1;"
            };

            var result = ApplyOpt(AST, new OptExprTransformUnaryToValue());
            CollectionAssert.AreEqual(expected, result);
            Assert.IsNotNull((AST.root.StatChildren[1] as AssignNode).Expr is IntNumNode);
        }

        [Test]
        public void TransformToBoolTest()
        {
            var AST = BuildAST(@"
var a, b;
a = !true;
b = !false;
");
            var expected = new[] {
                "var a, b;",
                "a = false;",
                "b = true;"
            };

            var result = ApplyOpt(AST, new OptExprTransformUnaryToValue());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TransformTwiceUnaryTest()
        {
            var AST = BuildAST(@"
var a, b;
a = !!b;
b = --a;
a = --b - ---a;
");
            var expected = new[] {
                "var a, b;",
                "a = b;",
                "b = a;",
                "a = (b - (-a));"
            };

            var result = ApplyOpt(AST, new OptExprTransformUnaryToValue());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
