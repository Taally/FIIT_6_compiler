using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    public class OptWhileFalseTests: ASTTestsBase
    {

        [Test]
        public void TestShouldCreateNoop()
        {
            var AST = BuildAST(@"var a;
while false
   a = true;");
            var expected = @"var a;
";

            var opt = new OptWhileFalseVisitor();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }

        [Test]
        public void TestShouldNotCreateNoop()
        {
            
            var AST = BuildAST(@"var a;
a = false;
while a
  a = true;");
            var expected = @"var a;
a = false;
while a
  a = true;";

            var opt = new OptWhileFalseVisitor();
            AST.root.Visit(opt);
            var pp = new PrettyPrintVisitor();
            AST.root.Visit(pp);
            Assert.AreEqual(expected, pp.Text);
        }
    }
    
}
