using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    public class OptExprEqualBoolNumIdTests: ASTTestsBase
    {
        [Test]
        public void SumNumTest()
        {
            var AST = BuildAST(@"var a, b, c, d;
a = b == b;
b = true == true;
while (5 == 5)
  c = true == false;
d = 7 == 8;");
            var expected = @"var a, b, c, d;
a = true;
b = true;
while true
  c = false;
d = false;";

            var opt = new OptExprEqualBoolNumId();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
}
