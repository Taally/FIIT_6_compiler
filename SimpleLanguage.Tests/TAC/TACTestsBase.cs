﻿using System.Collections.Generic;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;
using SimpleScanner;

namespace SimpleLanguage.Tests
{
    public class TACTestsBase
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
            => new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(GenTAC(program)));

        protected ControlFlowGraph GenCFG(List<Instruction> TAC)
            => new ControlFlowGraph(BasicBlockLeader.DivideLeaderToLeader(TAC));
    }
}
