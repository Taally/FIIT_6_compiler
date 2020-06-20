using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    public class OptExprEqualBoolNumTests: ASTTestsBase
    {
        [Test]
        public void SumNumTest()
        {
            var AST = BuildAST(@"var b, c, d;
b = true == true;
while (5 == 5)
  c = true == false;
d = 7 == 8;");
            var expected = @"var b, c, d;
b = true;
while true
  c = false;
d = false;";

            var opt = new OptExprEqualBoolNum();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
}
