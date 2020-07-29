using System;
using System.Collections.Generic;
using System.Linq;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;
using SimpleScanner;

namespace SimpleLanguage.Tests
{
    using Optimization = Func<IReadOnlyList<Instruction>, (bool wasChanged, IReadOnlyList<Instruction> instructions)>;
    public class OptimizationsTestBase
    {
        protected List<Instruction> GenTAC(string sourceCode)
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

        protected List<BasicBlock> GenBlocks(string program)
            => BasicBlockLeader.DivideLeaderToLeader(GenTAC(program));

        protected ControlFlowGraph BuildTACOptimizeCFG(string program)
        {
            var TAC = GenTAC(program);
            var optResult = ThreeAddressCodeOptimizer.OptimizeAll(TAC);
            var blocks = BasicBlockLeader.DivideLeaderToLeader(optResult);
            return new ControlFlowGraph(blocks);
        }

        protected ControlFlowGraph GenCFG(string program)
            => new ControlFlowGraph(GenBlocks(program));

        protected ControlFlowGraph GenCFG(List<Instruction> TAC)
            => new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));

        protected IEnumerable<string> TestTACOptimization(
            string sourceCode,
            Optimization basicBlockOptimization = null,
            Optimization allCodeOptimization = null,
            bool unreachableCodeElimination = false) =>
            ThreeAddressCodeOptimizer.Optimize(
                GenTAC(sourceCode),
                basicBlockOptimization == null ? null :
                new List<Optimization>()
                {
                    basicBlockOptimization
                },
                allCodeOptimization == null ? null :
                new List<Optimization>()
                {
                    allCodeOptimization
                },
                unreachableCodeElimination)
            .Select(instruction => instruction.ToString());
    }
}
