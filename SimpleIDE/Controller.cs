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
using SimpleLang.DataFlowAnalysis;

namespace SimpleIDE
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
                    allCodeOpt = new List<Optimization>(), allAllOpt = new List<Optimization>();
                var numPos = BasicBlockOptimizations.Count;
                var numPosFalse = BasicBlockOptimizations.Count + AllCodeOptimizations.Count;

                foreach (var n in lstCheck.TakeWhile(x=>x < numPos))
                {
                    bBlOpt.Add(BasicBlockOptimizations[n]);
                    allAllOpt.Add(BasicBlockOptimizations[n]);
                }

                foreach (var n in lstCheck.SkipWhile(x => x < numPos).TakeWhile(x => x < numPosFalse))
                {
                    allCodeOpt.Add(AllCodeOptimizations[n - numPos]);
                    allAllOpt.Add(AllCodeOptimizations[n - numPos]);
                }

                var UCE = lstCheck[lstCheck.Count - 1] == numPosFalse;

                var result = ThreeAddressCodeOptimizer.OptimizeForIDE(threeAddressCode,
                        bBlOpt, allCodeOpt, UCE).ToList();

                var strR = new StringBuilder();
                foreach (var x in result)
                {
                    strR.AppendLine(x.ToString());
                }
                return (strR.ToString(), threeAddressCode);
            }

            var str = new StringBuilder();
            foreach (var x in threeAddressCode)
            {
                str.AppendLine(x.ToString());
            }

            return (str.ToString(), threeAddressCode);
        }


        internal static (string str, ControlFlowGraph cfg) BuildCFG(IReadOnlyList<Instruction> instructions) {
            var divResult = BasicBlockLeader.DivideLeaderToLeader(instructions);
            var cfg = new ControlFlowGraph(divResult);

            var str = new StringBuilder();

            foreach (var block in cfg.GetCurrentBasicBlocks())
            {
                str.AppendLine($"{cfg.VertexOf(block)} --------");
                foreach (var inst in block.GetInstructions())
                {
                    str.AppendLine(inst.ToString());
                }
                str.AppendLine($"----------");

                var children = cfg.GetChildrenBasicBlocks(cfg.VertexOf(block));

                var childrenStr = string.Join(" | ", children.Select(v => v.vertex));
                str.AppendLine($" children: {childrenStr}");

                var parents = cfg.GetParentsBasicBlocks(cfg.VertexOf(block));
                var parentsStr = string.Join(" | ", parents.Select(v => v.vertex));
                str.AppendLine($" parents: {parentsStr}\r\n");
            }

            return (str.ToString(), cfg);
        }


        internal static string GetGraphInformation(ControlFlowGraph cfg) {
            var str = new StringBuilder();

            str.AppendLine("Доминаторы:");
            var domTree = new DominatorTree().GetDominators(cfg);

            foreach (var pair in domTree)
            {
                foreach (var x in pair.Value)
                {
                    str.AppendLine($"{cfg.VertexOf(x)} dom {cfg.VertexOf(pair.Key)}");
                }
                str.AppendLine("----------------");
            }


            str.AppendLine("\r\nКлассификация рёбер:");

            foreach (var pair in cfg.ClassifiedEdges)
            {
                str.AppendLine($"{ pair }");
            }

            str.AppendLine("\r\nОбходы графа:");

            str.AppendLine($"Прямой: { string.Join(" -> ", cfg.PreOrderNumeration) }");
            str.AppendLine($"Обратный: { string.Join(" -> ", cfg.PostOrderNumeration) }");

            str.AppendLine($"\r\nГлубинное остовное дерево:");
            foreach (var x in cfg.DepthFirstSpanningTree)
            {
                str.AppendLine($"({x.from} - > {x.to})");
            }

            var backEdges = cfg.GetBackEdges();
            if (backEdges.Count > 0)
            {
                str.AppendLine("\r\nОбратные рёбра:");
                foreach (var x in backEdges)
                {
                    str.AppendLine($"({cfg.VertexOf(x.Item1)}, {cfg.VertexOf(x.Item2)})");
                }
            }
            else
            {
                str.AppendLine("\r\nОбратных рёбер нет");
            }


            var answ = cfg.IsReducibleGraph() ? "Граф приводим" : "Граф неприводим";
            str.AppendLine($"\r\n{answ}");

            if (cfg.IsReducibleGraph())
            {
                var natLoops = NaturalLoop.GetAllNaturalLoops(cfg);
                if (natLoops.Count > 0)
                {
                    str.AppendLine($"\r\nЕстественные циклы:");
                    foreach (var x in natLoops)
                    {
                        if (x.Count == 0)
                        {
                            continue;
                        }
                        for (var i = 0; i < x.Count; i++)
                        {
                            str.AppendLine($"Номер блока: {i}");
                            foreach (var xfrom in x[i].GetInstructions())
                            {
                                str.AppendLine(xfrom.ToString());
                            }
                        }
                        str.AppendLine();
                        str.AppendLine("-------------");
                    }
                }
                else
                {
                    str.AppendLine($"\r\nЕстественных циклов нет");
                }
            }
            else
            {
                str.AppendLine($"\r\nНевозможно определить естественные циклы, т.к. граф неприводим");
            }

            return str.ToString();
        }

        internal static (string, string) ApplyIterativeAlgorithm(ControlFlowGraph cfg, List<string> opts) {
            var strReturn = new StringBuilder();
            var strBefore = new StringBuilder();


            foreach (var b in cfg.GetCurrentBasicBlocks())
            {
                foreach (var inst in b.GetInstructions())
                {
                    strBefore.AppendLine(inst.ToString());
                }
                strBefore.AppendLine("----------");
            }

            foreach (var opt in opts)
            {
                switch (opt)
                {
                    case "Доступные выражения":
                        var inout = new AvailableExpressions().Execute(cfg);
                        AvailableExpressionsOptimization.Execute(cfg, inout);
                        break;
                    case "Активные переменные":
                        LiveVariablesOptimization.DeleteDeadCode(cfg);
                        break;
                    case "Достигающие определения":
                        var reachDef = new ReachingDefinitionsOptimization();
                        reachDef.DeleteDeadCode(cfg);
                        break;
                }
            }


            foreach (var b in cfg.GetCurrentBasicBlocks())
            {
                foreach (var inst in b.GetInstructions())
                {
                    strReturn.AppendLine(inst.ToString());
                }
                strReturn.AppendLine("----------");
            }

            return (strBefore.ToString(),strReturn.ToString());
        }
    }
}
