using NUnit.Framework;
using ProgramTree;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprTransformUnaryToValueTests : ASTTestsBase
    {
        [Test]
        public void TransformToInt()
        {
            var AST = BuildAST(@"
var a, b;
a = (-1);
");
            var expected = new[]
            {
                "var a, b;",
                "a = -1;"
            };

            var result = ApplyAstOpt(AST, new OptExprTransformUnaryToValue());
            CollectionAssert.AreEqual(expected, result);
            Assert.IsTrue((AST.root.StatChildren[1] as AssignNode).Expr is IntNumNode);
        }

        [Test]
        public void TransformToBool()
        {
            var AST = BuildAST(@"
var a, b;
a = !true;
b = !false;
");
            var expected = new[]
            {
                "var a, b;",
                "a = false;",
                "b = true;"
            };

            var result = ApplyAstOpt(AST, new OptExprTransformUnaryToValue());
            CollectionAssert.AreEqual(expected, result);
            Assert.IsTrue((AST.root.StatChildren[1] as AssignNode).Expr is BoolValNode);
            Assert.IsTrue((AST.root.StatChildren[2] as AssignNode).Expr is BoolValNode);
        }

        [TestCase(@"
var a, b;
a = !!b;
b = --a;
a = --b - ---a;
",
            ExpectedResult = new[]
            {
                "var a, b;",
                "a = b;",
                "b = a;",
                "a = (b - (-a));"
            },
            TestName = "TransformTwiceUnary")]

        public string[] TestOptExprTransformUnaryToValue(string sourceCode) =>
            TestASTOptimization(sourceCode, new OptExprTransformUnaryToValue());
    }
}
