using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprFoldUnaryTests : ASTTestsBase
    {
        [Test]
        public void EqualIDTest()
        {
            var AST = BuildAST(@"
var a, b;
b = !a == !a;
b = !a != !a;
");
            var expected = new[] {
                "var a, b;",
                "b = true;",
                "b = false;"
            };

            var result = ApplyOpt(AST, new OptExprFoldUnary());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void LeftRightUnaryTest()
        {
            var AST = BuildAST(@"
var a, b;
b = !a == a;
b = !a != a;
b = a == !a;
b = a != !a;
");
            var expected = new[] {
                "var a, b;",
                "b = false;",
                "b = true;",
                "b = false;",
                "b = true;"
            };

            var result = ApplyOpt(AST, new OptExprFoldUnary());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
