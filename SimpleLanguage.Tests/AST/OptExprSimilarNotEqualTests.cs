using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprSimilarNotEqualTests : ASTTestsBase
    {
        [Test]
        public void SimilarNotEqualTest()
        {
            var AST = BuildAST(@"
var a, b, d, k, c;
c = a>a;
b = k<k;
d = a != a;
d = 1 > 1;
");

            var expected = new[] {
                "var a, b, d, k, c;",
                "c = false;",
                "b = false;",
                "d = false;",
                "d = false;",
            };

            var result = ApplyOpt(AST, new OptExprSimilarNotEqual());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
