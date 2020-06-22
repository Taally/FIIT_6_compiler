using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptStatIfFalseTests : ASTTestsBase
    {
        [Test]
        public void IfFalseTest()
        {
            var AST = BuildAST(@"
var a, b;
if false
a = b;
else
a = 1;
");

            var expected = new[] {
                "var a, b;",
                "a = 1;"
            };

            var result = ApplyOpt(AST, new OptStatIfFalse());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void IfFalseBlockTest()
        {
            var AST = BuildAST(@"
var a, b;
if false {
a = b;
b = 1;
}
else
a = 1;
");

            var expected = new[] {
                "var a, b;",
                "a = 1;"
            };

            var result = ApplyOpt(AST, new OptStatIfFalse());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void IfFalseComplexTest()
        {
            var AST = BuildAST(@"
var a, b;
if false
a =1;
else
if false {
a = b;
b = 1;
}
else
a = 1;

if a > b{
a = b;
if false{
b = b + 1;
b = b / 5;
}
}
");

            var expected = new[] {
                "var a, b;",
                "a = 1;",
                "if (a > b) {",
                "  a = b;",
                "}"
            };

            var result = ApplyOpt(AST, new OptStatIfFalse());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
