using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptAssignEqualityTests : ASTTestsBase
    {
        [Test]
        public void RemoveNode()
        {
            var AST = BuildAST(@"
var a, b;
a = a;
{ b = b; }
");
            var expected = @"var a, b;

{

}";
            var opt = new OptAssignEquality();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void WithoutRemoveConstants()
        {
            var AST = BuildAST(@"var a;
a = a + 0;
a = a - 0;
a = a * 1;
");
            var expected = @"var a;
a = (a + 0);
a = (a - 0);
a = (a * 1);";

            var opt = new OptAssignEquality();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
}
