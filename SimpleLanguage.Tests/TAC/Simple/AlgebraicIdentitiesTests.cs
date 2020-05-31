﻿using System.Collections.Generic;
using System.Linq;
using SimpleLang;
using NUnit.Framework;
using System;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    class AlebraicIdentitiesTests : TACTestsBase
    {
        readonly string messageErrorResult = "Результат неверный";
        readonly string messageErrorArgument1 = "Первый аргумент неверный";
        readonly string messageErrorArgument2 = "Второй аргумент неверный";
        readonly string messageErrorCheckOptimize = "Оптимизация не прошла";
        readonly string messageErrorOperationIsWrong = "Тип оптимизации неверный";
        readonly string messageErrorLabel = "Метка неверна";

        private void Checker(Instruction instruction, string lbl, string op, string arg1, string arg2, string res)
        {
            Assert.AreEqual(lbl, instruction.Label, messageErrorLabel);
            Assert.AreEqual(op, instruction.Operation, messageErrorOperationIsWrong);
            Assert.AreEqual(arg1, instruction.Argument1, messageErrorArgument1);
            Assert.AreEqual(arg2, instruction.Argument2, messageErrorArgument2);
            Assert.AreEqual(res, instruction.Result, messageErrorResult);
        }

        [Test]
        public void SubstractionSameNumbers() // a - a = 0
        {
            var threeAddressCode =
                ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities(new List<Instruction>()
                { new Instruction("0", "MINUS", "1", "1", "0") });
            var checkOptimize = threeAddressCode.Item1;
            var checkResult = threeAddressCode.Item2[0];
            Assert.AreEqual(true, checkOptimize, messageErrorCheckOptimize);
            Checker(checkResult, "0", "assign", "0", "", "0");
        }

        [Test]
        public void MultiplicationOnOne() // a * 1 = a || 1 * a = a
        {
            var threeAddressCode =
                ThreeAddressCodeRemoveAlgebraicIdentities
                .RemoveAlgebraicIdentities(new List<Instruction>()
                {
                    new Instruction("0", "MULT", "a", "1", "a"),
                    new Instruction("1", "MULT", "1", "a", "a"),
                    new Instruction("2", "MULT", "5", "1", "5"),
                    new Instruction("3", "MULT", "1", "5", "5"),
                });
            var checkOptimize = threeAddressCode.Item1;
            var listOfResult = threeAddressCode.Item2;

            Assert.AreEqual(true, checkOptimize, messageErrorCheckOptimize);

            Checker(listOfResult[0], "0", "assign", "a", "", "a");
            Checker(listOfResult[1], "1", "assign", "a", "", "a");
            Checker(listOfResult[2], "2", "assign", "5", "", "5");
            Checker(listOfResult[3], "3", "assign", "5", "", "5");
        }

        //Суммирование и вычитание с 0
        //Умножение на 1
        /*------------------------------*/
        [Test]
        public void SummationWithZero()
        {
            var threeAddressCode =
                ThreeAddressCodeRemoveAlgebraicIdentities
                .RemoveAlgebraicIdentities(new List<Instruction>()
                {
                    new Instruction("0", "PLUS", "a", "0", "a"),
                    new Instruction("1", "PLUS", "0", "a", "a"),
                    new Instruction("2", "PLUS", "5", "0", "5"),
                    new Instruction("3", "PLUS", "0", "5", "5"),
                });

            Assert.AreEqual(true, threeAddressCode.Item1, messageErrorCheckOptimize);
            var listOfResult = threeAddressCode.Item2;

            Checker(listOfResult[0], "0", "assign", "a", "", "a");
            Checker(listOfResult[1], "1", "assign", "a", "", "a");
            Checker(listOfResult[2], "2", "assign", "5", "", "5");
            Checker(listOfResult[3], "3", "assign", "5", "", "5");
        }

        [Test]
        public void DifferenceWithZero()
        {
            var threeAddressCode =
                ThreeAddressCodeRemoveAlgebraicIdentities
                .RemoveAlgebraicIdentities(new List<Instruction>()
                {
                    new Instruction("0", "MINUS", "0", "666", "-666"),
                    new Instruction("1", "MINUS", "0", "a", "-a"),
                    new Instruction("2", "MINUS", "1", "0", "1"),
                    new Instruction("3", "MINUS", "a", "0", "a"),
                });

            Assert.AreEqual(true, threeAddressCode.Item1, messageErrorCheckOptimize);
            var listOfResult = threeAddressCode.Item2;
            Checker(listOfResult[0], "0", "assign", "-666", "", "-666");
            Checker(listOfResult[1], "1", "assign", "-a", "", "-a");
            Checker(listOfResult[2], "2", "assign", "1", "", "1");
            Checker(listOfResult[3], "3", "assign", "a", "", "a");
        }

        /*------------------------------*/
        [Test]
        public void MultiplicationOnZero()
        {
            var threeAddressCode =
                ThreeAddressCodeRemoveAlgebraicIdentities
                    .RemoveAlgebraicIdentities(new List<Instruction>()
                    {
                       new Instruction("0", "MULT", "a", "0", "b"),
                       new Instruction("1", "MULT", "0", "a", "b"),
                       new Instruction("2", "MULT", "5", "0", "b"),
                       new Instruction("3", "MULT", "0", "5", "b"),
                       new Instruction("4", "MULT", "true", "0", "b"),
                       new Instruction("5", "MULT", "0", "true", "b"),
                    });
            var listOfResult = threeAddressCode.Item2;
            Assert.AreEqual(true, threeAddressCode.Item1, messageErrorCheckOptimize);
            Checker(listOfResult[0], "0", "assign", "0", "", "b");
            Checker(listOfResult[1], "1", "assign", "0", "", "b");
            Checker(listOfResult[2], "2", "assign", "0", "", "b");
            Checker(listOfResult[3], "3", "assign", "0", "", "b");
            Checker(listOfResult[4], "4", "MULT", "true", "0", "b");
            Checker(listOfResult[5], "5", "MULT", "0", "true", "b");
        }

        [Test]
        public void ZeroDivideOnNonZero()
        {
            var threeAddressCode =
                ThreeAddressCodeRemoveAlgebraicIdentities
                    .RemoveAlgebraicIdentities(new List<Instruction>()
                    {
                        new Instruction("0", "DIV", "0", "1", "b"),
                        new Instruction("1", "DIV", "0", "a", "b"),
                        new Instruction("2", "DIV", "0", "true", "b"),
                    });

            var checkOptimize = threeAddressCode.Item1;
            var listOfResult = threeAddressCode.Item2;

            Assert.AreEqual(true, checkOptimize, messageErrorCheckOptimize);
            Checker(listOfResult[0], "0", "assign", "0", "", "b");
            Checker(listOfResult[1], "1", "assign", "0", "", "b");
            Checker(listOfResult[2], "2", "DIV", "0", "true", "b");
        }

        [Test]
        public void DivideOnOne()
        {
            var threeAddressCode =
                ThreeAddressCodeRemoveAlgebraicIdentities
                    .RemoveAlgebraicIdentities(new List<Instruction>()
                    {
                        new Instruction("0", "DIV", "5", "1", "b"),
                        new Instruction("1", "DIV", "a", "1", "b"),
                        new Instruction("2", "DIV", "0", "1", "b"),
                        new Instruction("3", "DIV", "true", "1", "b"),
                    });
            var listOfResult = threeAddressCode.Item2;
            Assert.AreEqual(true, threeAddressCode.Item1, messageErrorCheckOptimize);
            Checker(listOfResult[0], "0", "assign", "5", "", "b");
            Checker(listOfResult[1], "1", "assign", "a", "", "b");
            Checker(listOfResult[2], "2", "assign", "0", "", "b");
            Checker(listOfResult[3], "3", "DIV", "true", "1", "b");
        }

        //a / a = 1
        [Test]
        public void DivideSameNumbers()
        {
            var threeAddressCode =
               ThreeAddressCodeRemoveAlgebraicIdentities
               .RemoveAlgebraicIdentities(new List<Instruction>()
               {
                   new Instruction("0", "DIV", "10", "10", "a"),
                   new Instruction("1", "DIV", "b", "b", "a"),
               });
            Checker(threeAddressCode.Item2[0], "0", "assign", "1", "", "a");
            Checker(threeAddressCode.Item2[1], "1", "assign", "1", "", "a");
        }

        [Test]
        public void ComplexTest()
        {
            var TAC = GenTAC(@"
            var a, b;
            b = a - a;
            b = a * 0;
            b = 0 * a;
            b = 1 * a;
            b = a * 1;
            b = a / 1;
            b = a + 0;
            b = 0 + a;
            b = a - 0;
            b = 0 - a;
            b = b / b;
            ");

            var expected = new List<string>()
                        {
                            "#t1 = 0",
                            "b = #t1",
                            "#t2 = 0",
                            "b = #t2",
                            "#t3 = 0",
                            "b = #t3",
                            "#t4 = a",
                            "b = #t4",
                            "#t5 = a",
                            "b = #t5",
                            "#t6 = a",
                            "b = #t6",
                            "#t7 = a",
                            "b = #t7",
                            "#t8 = a",
                            "b = #t8",
                            "#t9 = a",
                            "b = #t9",
                            "#t10 = -a",
                            "b = #t10",
                            "#t11 = 1",
                            "b = #t11"
                        };
            var optimizations = new List<Func<List<Instruction>, Tuple<bool, List<Instruction>>>>
            {
                ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities
            };
            var actual = ThreeAddressCodeOptimizer.Optimize(TAC, optimizations)
                .Select(instruction => instruction.ToString());
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
