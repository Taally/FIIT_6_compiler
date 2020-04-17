using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.ThreeAddrOpt{

    public class Use {
        public Def Parent { get; set; }
        public int OrderNum { get; set; }
        public Command Command { get; set; } //на всякий случай пока запоминаем

        public Use(int n, Command c, Def p = null) {
            Parent = p;
            Command = c;
            OrderNum = n;
        }
    }

    public class Def {
        public List<Use> Uses { get; set; }
        public int OrderNum { get; set; }
        public string Id { get; set; }

        public Def(int n, string id){
            Uses = new List<Use>();
            Id = id;
            OrderNum = n;
        }
    }


    public static class DefUseOpt{
        //Пока не используем UseList
        public static List<Def> DefList = new List<Def>();
        //public static List<Use> UseList = new List<Use>();

        private static bool IsId(string id) {
            return id != "" && id != "true" && id != "false" &&
                    (Char.IsLetter(id[0]) || id[0] == '#' || id[0] == '_');
        }

        private static void AddUse(string id, Command c, int num) {
            if (IsId(id)){
                var def = DefList.FindLastIndex(x => x.Id == id);
                var use = new Use(num, c);

                if (def != -1){
                    use.Parent = DefList[def];
                    DefList[def].Uses.Add(use);
                }
               // UseList.Add(use);
            }
        }

        private static void FillLists(List<Command> commands) {
            for (int i = 0; i < commands.Count; ++i) {
                DefList.Add(new Def(i,commands[i].Result));
                AddUse(commands[i].Arg1, commands[i], i);
                AddUse(commands[i].Arg2, commands[i], i);
            }
        }

        private static void DeleteUse(string id, int i) {
            if (id == "") return;
            //Вот здесь использовать инфу про use ( у него есть родитель! может по другому хранить)
            var d = DefList.FindLast(x => x.Id == id && x.OrderNum < i);
            if (d == null) return;
            d.Uses.RemoveAt(d.Uses.FindLastIndex(x=> x.OrderNum == i));
            //UseList.RemoveAt();
        }

        public static List<Command> DeleteDeadCode(List<Command> commands) {
            List<Command> result = new List<Command>();
            FillLists(commands);
            result.Add(commands[commands.Count - 1]);
            for (int i = commands.Count - 2; i > -1; --i) {
                //Мы удаляем код только! у временных переменных. Причем когда просто списки пусты. Каскад есть, но работает только
                //у временных переменных!
                if ((DefList.Find(x => x.OrderNum == i).Uses.Count == 0) || commands[i].Result[0] == '#') {
                    DeleteUse(commands[i].Arg1, i);
                    DeleteUse(commands[i].Arg2, i);
                    continue;
                }
                result.Add(commands[i]);
            }
            result.Reverse();
            return result;
        }
    }

    public static class ThreeAdrOpt {
        static string Op(string op, int a1, int a2){
            switch (op){
                case "+": return (a1+a2).ToString();
                case "-": return (a1-a2).ToString();
                case "*": return (a1*a2).ToString();
                case "/": return (a1/a2).ToString();
                case "<": return (a1<a2).ToString().ToLower();
                case "<=": return (a1<=a2).ToString().ToLower();
                case ">": return (a1 > a2).ToString().ToLower();
                case ">=": return (a1 >= a2).ToString().ToLower();
                case "==": return (a1 == a2).ToString().ToLower();
                case "!=": return (a1 != a2).ToString().ToLower();
                default: throw new Exception();
            }
        }

        static string Op(string op, bool a1, bool a2) {
            switch (op){
                case "==": return (a1 == a2).ToString().ToLower();
                case "!=": return (a1 != a2).ToString().ToLower();
                case "and": return (a1 && a2).ToString().ToLower();
                case "or": return (a1 || a2).ToString().ToLower();
                default: throw new Exception();
            }
        } 

        public static List<Command> ConvСonstants(List<Command> commands) {
            List<Command> result = new List<Command>();

            for (int i = 0; i < commands.Count; ++i) {
                if (commands[i].Arg2 != "") {
                    int argInt1, argInt2;
                    bool argBool1, argBool2;
                    if (Int32.TryParse(commands[i].Arg1, out argInt1)
                        && Int32.TryParse(commands[i].Arg2, out argInt2)) {
                        result.Add(new Command(commands[i].Result,
                            Op(commands[i].Op, argInt1, argInt2), "", "", ""));
                        continue;
                    }

                    if (bool.TryParse(commands[i].Arg1, out argBool1)
                        && bool.TryParse(commands[i].Arg2, out argBool2)) {
                        result.Add(new Command(commands[i].Result,
                           Op(commands[i].Op, argBool1, argBool2), "", "", ""));
                        continue;
                    }
                }
                result.Add(commands[i]); 
            }
            return result;
        }
    }
}
