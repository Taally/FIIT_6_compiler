using System.Collections;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.DataFlowAnalysis
{
    [TestFixture]
    public class ReachingDefinitionBinaryOperationTest
    {
        [Test]
        public void TestLowerShouldBeFalse()
        {
            var op = new ReachingDefinitionBinary.Operation(3);
            Assert.IsTrue(op.Lower.Count == 3);
            CollectionAssert.AreEqual(op.Lower, new BitArray(new [] { false, false , false }));
        }

        [Test]
        public void TestOperatorShouldAdd1()
        {
            var op = new ReachingDefinitionBinary.Operation(3);
            var a = new BitArray(new [] { true, false, false });
            var b = new BitArray(new [] { true, false, true });

            var result = op.Operator(a, b);
            
            Assert.IsTrue(result.Count == 3);
            CollectionAssert.AreEqual(result, new BitArray(new []{ true, false, true }));
        }
    }
}