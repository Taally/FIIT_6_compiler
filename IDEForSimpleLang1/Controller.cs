using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang;
using SimpleLang.Visitors;
using SimpleParser;
using SimpleScanner;
//using GraphVizWrapper;
//using GraphVizWrapper.Commands;
//using GraphVizWrapper.Queries;
using System.IO;
using System.Drawing;

namespace IDEForSimpleLang1
{
    using Optimization = Func<IReadOnlyList<Instruction>, (bool wasChanged, IReadOnlyList<Instruction> instructions)>;

    internal class Controller
    {
        internal static Parser GetParser(string sourceCode)
        {
            SymbolTable.vars.Clear();
            var scanner = new Scanner();
            scanner.SetSource(sourceCode, 0);
            var parser = new Parser(scanner);

            return parser; 
        }

        private static readonly List<ChangeVisitor> ASTOptimizations = new List<ChangeVisitor>{
            new OptExprAlgebraic(),
            new OptExprEqualBoolNum(),
            new OptExprFoldUnary(),
            new OptExprMultDivByOne(),
            new OptExprMultZero(),
            new OptExprSimilarNotEqual(),
            new OptExprSubEqualVar(),
            new OptExprSumZero(),
            new OptExprTransformUnaryToValue(),
            new OptExprVarEqualToItself(),
            new OptExprWithOperationsBetweenConsts(),
            new IfNullElseNull(),
            new OptAssignEquality(),
            new OptStatIfFalse(),
            new OptStatIfTrue(),
            new OptWhileFalseVisitor()
        };

        internal static string GetASTWithOpt(Parser parser, List<int> lstCheck)
        {
            var b = parser.Parse();
            if (!b)
            {
                return "Error. AST not build";
            }

            var fillParents = new FillParentsVisitor();
            parser.root.Visit(fillParents);
            var listOpt = new List<ChangeVisitor>();
            if (lstCheck.Count > 0) {
                foreach (var n in lstCheck)
                {
                    listOpt.Add(ASTOptimizations[n]);
                }
                ASTOptimizer.Optimize(parser, listOpt);
            }
            
            var pp = new PrettyPrintVisitor();
            parser.root.Visit(pp);
            return pp.Text;
        }

        private static readonly List<Optimization> BasicBlockOptimizations = new List<Optimization>()
        {
            ThreeAddressCodeDefUse.DeleteDeadCode,
            DeleteDeadCodeWithDeadVars.DeleteDeadCode,
            ThreeAddressCodeRemoveAlgebraicIdentities.RemoveAlgebraicIdentities,
            ThreeAddressCodeCommonExprElimination.CommonExprElimination,
            ThreeAddressCodeCopyPropagation.PropagateCopies,
            ThreeAddressCodeConstantPropagation.PropagateConstants,
            ThreeAddressCodeFoldConstants.FoldConstants
        };

        private static readonly List<Optimization> AllCodeOptimizations = new List<Optimization>
        {
            ThreeAddressCodeGotoToGoto.ReplaceGotoToGoto,
            ThreeAddressCodeRemoveGotoThroughGoto.RemoveGotoThroughGoto,
            ThreeAddressCodeRemoveNoop.RemoveEmptyNodes
        };

        internal static (string str, IReadOnlyList<Instruction> instructions) GetTACWithOpt(
            Parser parser, List<int> lstCheck)
        {
            ThreeAddressCodeTmp.ResetTmpLabel();
            ThreeAddressCodeTmp.ResetTmpName();
            var threeAddrCodeVisitor = new ThreeAddrGenVisitor();
            parser.root.Visit(threeAddrCodeVisitor);
            var threeAddressCode = threeAddrCodeVisitor.Instructions;

            if (lstCheck.Count > 0)
            {
                List<Optimization> bBlOpt = new List<Optimization>(), 
                    allCodeOpt = new List<Optimization>();
                var numPos = BasicBlockOptimizations.Count;
                var numPosFalse = BasicBlockOptimizations.Count + AllCodeOptimizations.Count;

                foreach (var n in lstCheck.TakeWhile(x=>x < numPos))
                {
                    bBlOpt.Add(BasicBlockOptimizations[n]);
                }

                foreach (var n in lstCheck.SkipWhile(x => x < numPos).TakeWhile(x => x < numPosFalse))
                {
                    allCodeOpt.Add(AllCodeOptimizations[n - numPos]);
                }

                var UCE = lstCheck[lstCheck.Count - 1] == numPosFalse;

                threeAddressCode = ThreeAddressCodeOptimizer.Optimize(threeAddressCode,
                    bBlOpt, allCodeOpt, UCE).ToList();
            }

            var str = new StringBuilder();
            foreach (var x in threeAddressCode)
            {
                str.Append(x);
                str.Append(Environment.NewLine);
            }

            return (str.ToString(), threeAddressCode);
        }


        internal static (string str, ControlFlowGraph cfg) BuildCFG(IReadOnlyList<Instruction> instructions) {
            var divResult = BasicBlockLeader.DivideLeaderToLeader(instructions);
            var cfg = new ControlFlowGraph(divResult);

            var str = new StringBuilder();

            foreach (var block in cfg.GetCurrentBasicBlocks())
            {
                str.Append($"{cfg.VertexOf(block)} --------\r\n");
                foreach (var inst in block.GetInstructions())
                {
                    str.Append(inst.ToString());
                    str.Append("\r\n");
                }
                str.Append($"----------\r\n");

                var children = cfg.GetChildrenBasicBlocks(cfg.VertexOf(block));

                var childrenStr = string.Join(" | ", children.Select(v => v.vertex));
                str.Append($" children: {childrenStr}\r\n");

                var parents = cfg.GetParentsBasicBlocks(cfg.VertexOf(block));
                var parentsStr = string.Join(" | ", parents.Select(v => v.vertex));
                str.Append($" parents: {parentsStr}\r\n\r\n");
            }

            return (str.ToString(), cfg);
        }


        internal static string GetGraphInformation(ControlFlowGraph cfg) {
            var str = new StringBuilder();
            str.Append("Классификация ребер:\r\n");
            foreach (var pair in cfg.ClassifiedEdges)
            {
                str.Append($"{ pair }\r\n");
            }




            return str.ToString();
        }
    }
}
