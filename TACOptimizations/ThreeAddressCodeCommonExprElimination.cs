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

        public static (bool wasChanged, IReadOnlyList<Instruction> instruction) CommonExprElimination(IReadOnlyList<Instruction> instructions)
        {
            var exprToResults = new StringToStrings();
            var argToExprs = new StringToStrings();
            var resultToExpr = new Dictionary<string, string>();

            var wasChanged = false;
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

            foreach (var instruction in instructions)
            {
                if (instruction.Operation == "noop")
                {
                    continue;
                }

                var expr = uniqueExpr(instruction);
                if (instruction.Operation != "assign" && exprToResults.TryGetValue(expr, out var results) && results.Count != 0)
                {
                    wasChanged = true;

                    newInstructions.Add(new Instruction(instruction.Label, "assign", results.First(), "", instruction.Result));
                }
                else
                {
                    newInstructions.Add(instruction.Copy());
                    addLink(argToExprs, instruction.Argument1, expr);
                    addLink(argToExprs, instruction.Argument2, expr);
                }

                if (resultToExpr.TryGetValue(instruction.Result, out var oldExpr) &&
                    exprToResults.ContainsKey(oldExpr))
                {
                    exprToResults[oldExpr].Remove(instruction.Result);
                }

                resultToExpr[instruction.Result] = expr;
                addLink(exprToResults, expr, instruction.Result);

                if (argToExprs.ContainsKey(instruction.Result))
                {
                    foreach (var delExpr in argToExprs[instruction.Result])
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
            return (wasChanged, newInstructions);
        }
    }
}
