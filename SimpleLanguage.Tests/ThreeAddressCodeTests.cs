using NUnit.Framework;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;
using SimpleScanner;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLanguage.Tests
{
    [TestFixture]
    public class ThreeAddressCodeTests
    {
        List<Instruction> GenTAC(string sourceCode)
        {
            ThreeAddressCodeTmp.ResetTmpName();
            ThreeAddressCodeTmp.ResetTmpLabel();
            SymbolTable.vars.Clear();   // oh yeah, all variables are stored in a static dict :D
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
        public void SimpleFold()
        {
            var TAC = GenTAC(@"
        var a;
        a = 1 + 2;");

            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);
            TAC = ThreeAddressCodeOptimizer.Optimize(TAC);

            var expected = new List<string>()
                    {
                        "#t1 = 3",
                        "a = #t1"
                    };
            var actual = TAC.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void FoldAndPropagateConstants()
        {
            var TAC = GenTAC(@"
        var a;
        a = 1 + 2 * 3 - 7;");

            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodePullingConstants.PullingConstants);
            TAC = ThreeAddressCodeOptimizer.Optimize(TAC);

            var expected = new List<string>()
                    {
                        "#t1 = 6",
                        "#t2 = 7",
                        "#t3 = 0",
                        "a = 0"
                    };
            var actual = TAC.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}