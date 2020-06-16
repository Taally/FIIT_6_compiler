using System.Collections.Generic;

namespace SimpleLang
{
    // Накапливание Def-Use информации и удаление определений с Uses.Count == 0
    public static class ThreeAddressCodeDefUse
    {
        public static List<Def> DefList;

        internal static bool IsId(string id) =>
            id != null && id != "" && id != "True" && id != "False" &&
                    (char.IsLetter(id[0]) || id[0] == '#' || id[0] == '_') && id[0] != 'L';

        private static void AddUse(string id, Instruction c, int num)
        {
            if (IsId(id))
            {
                var def = DefList.FindLastIndex(x => x.Id == id);
                var use = new Use(num, c);

                if (def != -1)
                {
                    use.Parent = DefList[def];
                    DefList[def].Uses.Add(use);
                }
            }
        }

        private static readonly List<string> operations = new List<string>()
            { "PLUS", "MINUS", "MULT", "DIV", "EQUAL", "NOTEQUAL", "LESS", "EQLESS", "GREATER", "EQGREATER", "AND", "OR",
            "UNMINUS", "NOT", "assign", "input"};

        private static void FillLists(List<Instruction> commands)
        {
            DefList = new List<Def>();
            for (var i = 0; i < commands.Count; ++i)
            {
                if (operations.Contains(commands[i].Operation))
                {
                    DefList.Add(new Def(i, commands[i].Result));
                }
                AddUse(commands[i].Argument1, commands[i], i);
                AddUse(commands[i].Argument2, commands[i], i);
            }
        }

        private static void DeleteUse(string id, int i)
        {
            if (id == "" || id == null)
            {
                return;
            }
            var d = DefList.FindLast(x => x.Id == id && x.OrderNum < i);
            if (d == null)
            {
                return;
            }
            d.Uses.RemoveAt(d.Uses.FindLastIndex(x => x.OrderNum == i));
        }

        public static (bool wasChanged, List<Instruction> instructions) DeleteDeadCode(List<Instruction> commands)
        {
            var result = new List<Instruction>();
            FillLists(commands);
            var wasChanged = false;

            for (var i = commands.Count - 1; i >= 0; --i)
            {
                var c = commands[i];
                var lastDefInd = DefList.FindLastIndex(x => x.Id == c.Result);
                var curDefInd = DefList.FindIndex(x => x.OrderNum == i);

                if (curDefInd != -1 && DefList[curDefInd].Uses.Count == 0
                        && (c.Result[0] == '#' || curDefInd != lastDefInd))
                {
                    DeleteUse(commands[i].Argument1, i);
                    DeleteUse(commands[i].Argument2, i);
                    result.Add(new Instruction(commands[i].Label, "noop", null, null, null));
                    wasChanged = true;
                }
                else
                {
                    result.Add(commands[i]);
                }
            }
            result.Reverse();
            return (wasChanged, result);
        }
    }

    public class Use
    {
        public Def Parent { get; set; }
        public int OrderNum { get; set; }
        public Instruction Command { get; set; }

        public Use(int n, Instruction c, Def p = null)
        {
            Parent = p;
            Command = c;
            OrderNum = n;
        }
    }

    public class Def
    {
        public List<Use> Uses { get; set; }
        public int OrderNum { get; set; }
        public string Id { get; set; }

        public Def(int n, string id)
        {
            Uses = new List<Use>();
            Id = id;
            OrderNum = n;
        }
    }
}
