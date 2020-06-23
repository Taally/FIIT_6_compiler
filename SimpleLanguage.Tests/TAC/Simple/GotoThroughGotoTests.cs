using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    using Optimization = Func<IReadOnlyList<Instruction>, (bool wasChanged, IReadOnlyList<Instruction> instructions)>;

    [TestFixture]
    internal class GotoThroughGotoTests : OptimizationsTestBase
    {
        [Test]
        public void Optimization()
        {
            // обновление глобальных переменных для корректной работы теста
            GenTAC(@"var a;");

            var expectedTAC = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "7: if #t1 goto L1",
                "goto L2",
                "L1: goto 3",
                "L2: noop",
                "2: goto 4",
                "3: a = 0",
                "4: a = 1",
                "666: a = False"
            };

            var TAC = new List<Instruction>()
            {
                new Instruction("1", "LESS", "1", "2", "#t1"),
                new Instruction("7", "ifgoto", "#t1", "L1", ""),
                new Instruction("", "goto", "L2", "", ""),
                new Instruction("L1", "goto", "3", "", ""),
                new Instruction("L2", "noop", "", "", ""),
                new Instruction("2", "goto", "4", "", ""),
                new Instruction("3", "assign", "0", "", "a"),
                new Instruction("4", "assign", "1", "", "a"),
                new Instruction("666", "assign", "False", "", "a")
            };

            ThreeAddressCodeTmp.GenTmpLabel(); // L1
            ThreeAddressCodeTmp.GenTmpLabel(); // L2
            ThreeAddressCodeTmp.GenTmpName();  // #t1

            var expected = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "7: #t2 = !#t1",
                "if #t2 goto L2",
                "goto 3",
                "L2: noop",
                "2: goto 4",
                "3: a = 0",
                "4: a = 1",
                "666: a = False"
            };

            //CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var optimizations = new List<Optimization> { ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString()).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void OptimizationAfterOtherOptimizations()
        {
            GenTAC(@"var a;");  // для корректной работы теста

            var expectedTAC = new List<string>()
            {
                "input a",
                "1: #t1 = a == 0",
                "#t2 = !#t1",
                "if #t2 goto 2",
                "goto 3",
                "2: a = 2",
                "3: a = 3",
            };

            var TAC = new List<Instruction>()
            {
                new Instruction("", "input", "", "", "a"),
                new Instruction("1", "EQUAL", "a", "0", "#t1"),
                new Instruction("", "NOT", "#t1", "", "#t2"),
                new Instruction("", "ifgoto", "#t2", "2", ""),
                new Instruction("", "goto", "3", "", ""),
                new Instruction("2", "assign", "2", "", "a"),
                new Instruction("3", "assign", "3", "", "a"),
            };

            ThreeAddressCodeTmp.GenTmpName();  // #t1
            ThreeAddressCodeTmp.GenTmpName();  // #t2

            //CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var expected = new List<string>()
            {
                "input a",
                "1: #t1 = a == 0",
                "#t2 = !#t1",
                "#t3 = !#t2",
                "if #t3 goto 3",
                "2: a = 2",
                "3: a = 3",
            };

            var optimizations = new List<Optimization> { ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString()).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void WithoutOptimizationIncorrectIf()
        {
            GenTAC(@"var a;"); //
            // нет false ветки и соотв. goto
            var expectedTAC = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "L1: goto 3",
                "a = 5 * 6",
                "L2: noop"
            };

            var TAC = new List<Instruction>()
            {
                new Instruction("1", "LESS", "1", "2", "#t1"),
                new Instruction("", "ifgoto", "#t1", "L1", ""),
                new Instruction("L1", "goto", "3", "", ""),
                new Instruction("", "MULT", "5", "6", "a"),
                new Instruction("L2", "noop", "", "", "")
            };

            ThreeAddressCodeTmp.GenTmpLabel(); // L1
            ThreeAddressCodeTmp.GenTmpLabel(); // L2
            ThreeAddressCodeTmp.GenTmpName();  // #t1

            var expected = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "L1: goto 3",
                "a = 5 * 6",
                "L2: noop"
            };

            //CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var optimizations = new List<Optimization> { ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, allCodeOptimizations: optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void WithoutOptimizationCorrectIf()
        {
            GenTAC(@"var a;"); //
            // instructions aren't changed
            var expectedTAC = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto 4",
                "a = 5 * 6",
                "L1: #t2 = 5 * 6",
                "#t3 = 4 + #t2",
                "a = #t3",
                "L2: noop"
            };
            var TAC = new List<Instruction>()
            {
                new Instruction("1", "LESS", "1", "2", "#t1"),
                new Instruction("", "ifgoto", "#t1", "L1", ""),
                new Instruction("", "goto", "4", "", ""),
                new Instruction("", "MULT", "5", "6", "a"),
                new Instruction("L1", "MULT", "5", "6", "#t2"),
                new Instruction("", "PLUS", "4", "#t2", "#t3"),
                new Instruction("", "assign", "#t3", "", "a"),
                new Instruction("L2", "noop", "", "", "")
            };

            ThreeAddressCodeTmp.GenTmpLabel(); // L1
            ThreeAddressCodeTmp.GenTmpLabel(); // L2
            ThreeAddressCodeTmp.GenTmpName();  // #t1
            ThreeAddressCodeTmp.GenTmpName();  // #t2
            ThreeAddressCodeTmp.GenTmpName();  // #t3

            var expected = new List<string>()
            {
                "1: #t1 = 1 < 2",
                "if #t1 goto L1",
                "goto 4",
                "a = 5 * 6",
                "L1: #t2 = 5 * 6",
                "#t3 = 4 + #t2",
                "a = #t3",
                "L2: noop"
            };

            //CollectionAssert.AreEqual(TAC.Select(instruction => instruction.ToString()), expectedTAC);

            var optimizations = new List<Optimization> { ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
