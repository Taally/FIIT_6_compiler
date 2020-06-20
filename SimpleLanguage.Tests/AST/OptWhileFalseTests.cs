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
            var expected = new[] {
                "var a;"
            };

            var result = ApplyOpt(new OptWhileFalseVisitor());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestShouldNotCreateNoop()
        {
            
            var AST = BuildAST(@"var a;
a = false;
while a
  a = true;");

            var expected = new[] {
                "var a;",
                "a = false;",
                "while a",
                "  a = true;"
            };

            var result = ApplyOpt(new OptWhileFalseVisitor());
            CollectionAssert.AreEqual(expected, result);
        }
    }
    
}
