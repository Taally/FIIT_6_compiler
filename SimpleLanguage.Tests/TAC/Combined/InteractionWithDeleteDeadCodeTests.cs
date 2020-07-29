using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Combined
{
    using Optimization = Func<IReadOnlyList<Instruction>, (bool wasChanged, IReadOnlyList<Instruction> instructions)>;

    [TestFixture]
    internal class InteractionWithDeleteDeadCodeTests : OptimizationsTestBase
    {
        [TestCase(@"
if (true == true)
    print(0);
",
            ExpectedResult = new string[]
            {
                "if False goto L1",
                "print 0",
                "L1: noop",
            },
            TestName = "PropagateCopiesDeleteDeadCode")]

        public IEnumerable<string> ComplexInteractionTest(string sourceCode) =>
            ThreeAddressCodeOptimizer.Optimize(
                GenTAC(sourceCode),
                basicBlockOptimizations: new List<Optimization>()
                {
                    DeleteDeadCodeWithDeadVars.DeleteDeadCode,
                    ThreeAddressCodeCopyPropagation.PropagateCopies,
                    ThreeAddressCodeFoldConstants.FoldConstants,
                    ThreeAddressCodeRemoveNoop.RemoveEmptyNodes
                })
            .Select(instruction => instruction.ToString());
    }
}
