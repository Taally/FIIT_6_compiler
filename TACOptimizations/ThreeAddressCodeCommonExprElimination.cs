using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    using StringToStrings = Dictionary<string, HashSet<string>>;

    public static class ThreeAddressCodeCommonExprElimination
    {
        public static bool IsCommutative(Instruction instr)
        {
            switch (instr.Operation)
            {
                case "OR":
                case "AND":
                case "EQUAL":
                case "NOTEQUAL":
                case "PLUS":
                case "MULT":
                    return true;
            }
            return false;
        }
        public static Tuple<bool, List<Instruction>> CommonExprElimination(List<Instruction> instructions)
        {
            var exprToResults = new StringToStrings();
            var argToExprs = new StringToStrings();
            var resultToExpr = new Dictionary<string, string>();

            var changed = false;
            var newInstructions = new List<Instruction>(instructions.Count);

            string uniqueExpr(Instruction instr) =>
                string.Format(IsCommutative(instr) && string.Compare(instr.Argument1, instr.Argument2) > 0 ?
                        "{2}{1}{0}" : "{0}{1}{2}", instr.Argument1, instr.Operation, instr.Argument2);

            void addLink(StringToStrings dict, string key, string value)
            {
                if (key != null)
                {
                    if (dict.ContainsKey(key))
                    {
                        dict[key].Add(value);
                    }
                    else
                    {
                        dict[key] = new HashSet<string>() { value };
                    }
                }
            }

            for (var i = 0; i < instructions.Count; ++i)
            {
                var expr = uniqueExpr(instructions[i]);
                if (instructions[i].Operation != "assign" && exprToResults.TryGetValue(expr, out var results) && results.Count != 0)
                {
                    changed = true;

                    newInstructions.Add(new Instruction(instructions[i].Label, "assign", results.First(), "", instructions[i].Result));
                }
                else
                {
                    newInstructions.Add(instructions[i].Copy());
                    addLink(argToExprs, instructions[i].Argument1, expr);
                    addLink(argToExprs, instructions[i].Argument2, expr);
                }

                if (resultToExpr.TryGetValue(instructions[i].Result, out var oldExpr))
                {
                    if (exprToResults.ContainsKey(oldExpr))
                    {
                        exprToResults[oldExpr].Remove(instructions[i].Result);
                    }
                }

                resultToExpr[instructions[i].Result] = expr;
                addLink(exprToResults, expr, instructions[i].Result);

                if (argToExprs.ContainsKey(instructions[i].Result))
                {
                    foreach (var delExpr in argToExprs[instructions[i].Result])
                    {
                        if (exprToResults.ContainsKey(delExpr))
                        {
                            foreach (var res in exprToResults[delExpr])
                            {
                                resultToExpr.Remove(res);
                            }
                        }
                        exprToResults.Remove(delExpr);
                    }
                }
            }
            return Tuple.Create(changed, newInstructions);
        }
    }
}
