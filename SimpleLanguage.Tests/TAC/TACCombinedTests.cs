﻿//using System.Collections.Generic;
//using System.Linq;
//using SimpleLang;
//using NUnit.Framework;

//namespace SimpleLanguage.Tests
//{
//    [TestFixture]
//    class TACCombinedTests : TACTestsBase
//    {
////        [Test]
////        public void FoldPropagateConstantsTest1()
////        {
////            var TAC = GenTAC(@"
////var x, y;
////x = 14;
////y = 7 - x;
////x = x + x;
////");
////            ThreeAddressCodeOptimizer.Optimizations.Clear();
////            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);
////            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);

////            var expected = new List<string>()
////            {
////                "x = 14",
////                "#t1 = -7",
////                "y = -7",
////                "#t2 = 28",
////                "x = 28"
////            };
////            var actual = ThreeAddressCodeOptimizer.OptimizeBlocks(TAC)
////                .Select(instruction => instruction.ToString());

////            CollectionAssert.AreEqual(expected, actual);
////        }

////        [Test]
////        public void FoldPropagateConstantsTest2()
////        {
////            var TAC = GenTAC(@"
////var a;
////a = 1 + 2 * 3 - 7;
////");
////            ThreeAddressCodeOptimizer.Optimizations.Clear();
////            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeFoldConstants.FoldConstants);
////            ThreeAddressCodeOptimizer.Optimizations.Add(ThreeAddressCodeConstantPropagation.PropagateConstants);

////            var expected = new List<string>()
////            {
////                "#t1 = 6",
////                "#t2 = 7",
////                "#t3 = 0",
////                "a = 0"
////            };
////            var actual = ThreeAddressCodeOptimizer.OptimizeBlocks(TAC)
////                .Select(instruction => instruction.ToString());

////            CollectionAssert.AreEqual(expected, actual);
////        }
//    }
//}
