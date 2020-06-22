using NUnit.Framework;
using SimpleLang.Visitors;

namespace SimpleLanguage.Tests.AST
{
    internal class OptExprWithOperationsBetweenConstsTests : ASTTestsBase
    {
        [Test]
        public void TestOpLess1()
        {
            var AST = BuildAST(@"
var c;
c = 3 < 15;
");
            var expected = new[] {
                "var c;",
                "c = true;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpLess2()
        {
            var AST = BuildAST(@"
var c;
c = 3 < 2;
");
            var expected = new[] {
                "var c;",
                "c = false;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpGreater1()
        {
            var AST = BuildAST(@"
var c;
c = 3 > 2;
");
            var expected = new[] {
                "var c;",
                "c = true;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpGreater2()
        {
            var AST = BuildAST(@"
var c;
c = 3 > 4;
");
            var expected = new[] {
                "var c;",
                "c = false;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpEQGREATER1()
        {
            var AST = BuildAST(@"
var c;
c = 3 >= 2;
");
            var expected = new[] {
                "var c;",
                "c = true;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpEQGREATER2()
        {
            var AST = BuildAST(@"
var c;
c = 3 >= 4;
");
            var expected = new[] {
                "var c;",
                "c = false;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpEQGREATER3()
        {
            var AST = BuildAST(@"
var c;
c = 3 >= 3;
");
            var expected = new[] {
                "var c;",
                "c = true;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpEQLESS1()
        {
            var AST = BuildAST(@"
var c;
c = 3 <= 4;
");
            var expected = new[] {
                "var c;",
                "c = true;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpEQLESS2()
        {
            var AST = BuildAST(@"
var c;
c = 3 <= 2;
");
            var expected = new[] {
                "var c;",
                "c = false;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpEQLESS3()
        {
            var AST = BuildAST(@"
var c;
c = 3 <= 3;
");
            var expected = new[] {
                "var c;",
                "c = true;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpNOTEQUAL1()
        {
            var AST = BuildAST(@"
var c;
c = 3 != 2;
");
            var expected = new[] {
                "var c;",
                "c = true;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpNOTEQUAL2()
        {
            var AST = BuildAST(@"
var c;
c = 3 != 3;
");
            var expected = new[] {
                "var c;",
                "c = false;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpNOTEQUAL3()
        {
            var AST = BuildAST(@"
var c;
c = true != true;
");
            var expected = new[] {
                "var c;",
                "c = false;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestOpNOTEQUAL4()
        {
            var AST = BuildAST(@"
var c;
c = false != true;
");
            var expected = new[] {
                "var c;",
                "c = true;"
            };

            var result = ApplyOpt(AST, new OptExprWithOperationsBetweenConsts());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
