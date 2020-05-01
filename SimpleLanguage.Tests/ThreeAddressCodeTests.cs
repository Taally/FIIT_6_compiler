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
a = 1 - 20;
a = 4 * 2;
a = 10 * 5;
a = 9 + 3;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);
            TAC = ThreeAddressCodeOptimizer.Optimize(TAC);

            var expected = new List<string>()
                    {
                        "#t1 = -19",
                        "a = #t1",
                        "#t2 = 8",
                        "a = #t2",
                        "#t3 = 50",
                        "a = #t3",
                        "#t4 = 12",
                        "a = #t4"
                    };
            var actual = TAC.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void PropagateConstants()
        {
            var TAC = GenTAC(@"
var x, y;
x = 14;
y = 7 - x;
x = x + x;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);
            TAC = ThreeAddressCodeOptimizer.Optimize(TAC);

            var expected = new List<string>()
                    {
                        "x = 14",
                        "#t1 = 7 - 14",
                        "y = #t1",
                        "#t2 = 14 + 14",
                        "x = #t2"
            };
            var actual = TAC.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void FoldPropagateConstants1()
        {
            var TAC = GenTAC(@"
var x, y;
x = 14;
y = 7 - x;
x = x + x;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);
            TAC = ThreeAddressCodeOptimizer.Optimize(TAC);

            var expected = new List<string>()
                    {
                        "x = 14",
                        "#t1 = -7",
                        "y = -7",
                        "#t2 = 28",
                        "x = 28"
            };
            var actual = TAC.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void FoldPropagateConstants2()
        {
            var TAC = GenTAC(@"
var a;
a = 1 + 2 * 3 - 7;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);
            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);
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

        [Test]
        public void DeleteDeadCode()
        {
            var TAC = GenTAC(@"
var a, b;
a = 1;
a = 2;
b = 11;
b = 22;
a = 3;
a = b;
");
            ThreeAddressCodeOptimizer.Optimizations.Clear();
            ThreeAddressCodeOptimizer.Optimizations.Add(DeleteDeadCodeWithDeadVars.DeleteDeadCode);
            TAC = ThreeAddressCodeOptimizer.Optimize(TAC);

            var expected = new List<string>()
                    {
                        "noop",
                        "noop",
                        "noop",
                        "b = 22",
                        "noop",
                        "a = b"
                    };
            var actual = TAC.Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        // TODO: right now copy propagation deletes original vars and propagates temps, is it intended?
        //        [Test]
        //        public void PropagateCopies()
        //        {
        //            var TAC = GenTAC(@"
        //var a, b, c, d, e, x, y, k;
        //a = b;
        //c = b – a;
        //d = c + 1;
        //e = d * a;
        //a = x - y;
        //k = c + a;
        //");
        //            ThreeAddressCodeOptimizer.Optimizations.Clear();
        //            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeCopyPropagation.PropagateCopies);
        //            TAC = ThreeAddressCodeOptimizer.Optimize(TAC);

        //            var expected = new List<string>()
        //                    {
        //                        "a = b",
        //                        "#t1 = b - b",
        //                        "c = #t1",
        //                        "#t2 = c + 1",
        //                        "d = #t2",
        //                        "#t3 = d * b",
        //                        "e = #t3",
        //                        "#t4 = x - y",
        //                        "a = #t4",
        //                        "#t5 = c + a",
        //                        "k = #t5"
        //                    };
        //            var actual = TAC.Select(instruction => instruction.ToString());

        //            CollectionAssert.AreEqual(expected, actual);
        //        }
    }
}