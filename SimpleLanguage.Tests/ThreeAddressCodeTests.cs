using NUnit.Framework;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;
using SimpleScanner;
using System.Collections.Generic;

namespace SimpleLanguage.Tests
{
    [TestFixture]
    public class ThreeAddressCodeTests
    {
        List<Instruction> GenTAC(string sourceCode)
        {
            var scanner = new Scanner();
            scanner.SetSource(sourceCode, 0);
            var parser = new Parser(scanner);
            parser.Parse();
            var fillParents = new FillParentsVisitor();
            parser.root.Visit(fillParents);
            var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
            parser.root.Visit(threeAddrCodeVisitor);
            return threeAddrCodeVisitor.Instructions;
        }

        [Test]
        public void FoldConstantsTest()
        {
            var TAC = GenTAC(@"
var a;
a = 1 + 2;");

            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);
            TAC = ThreeAddressCodeOptimizer.Optimize(TAC);
            Assert.That(TAC[0].ToString() == "#t1 = 3");
        }
    }
}