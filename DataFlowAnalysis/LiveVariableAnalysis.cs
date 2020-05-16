using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    class LiveVariableAnalysis
    {
        // Пока базовый блок - список конструкций
        // Во множествах def-use будем хранить пока строки - названия переменных
        HashSet<string> use;
        HashSet<string> def;

        List<Instruction> block;

        public LiveVariableAnalysis() {
            block = new List<Instruction>()
            {
                new Instruction("", "PLUS", "b", "c", "#t1"),
                new Instruction("", "assign", "#t1","","a"),
                new Instruction("","PLUS", "#t1","1","#t2"),
                new Instruction("", "assign", "#t2","","d"),
                new Instruction("","PLUS", "#t1","7","#t3"),
                new Instruction("", "assign", "#t3","","c"),
                new Instruction("","PLUS", "#t3","1","#t4"),
                new Instruction("", "assign", "#t4","","b")
            };
            use = new HashSet<string>();
            def = new HashSet<string>();
        }

        Func<string, bool> IsId = ThreeAddressCodeDefUse.IsId;

        public void FillDefUse() {
            for (int i = 0; i < block.Count; ++i) {
                var inst = block[i];

                if (IsId(inst.Argument1) && !def.Contains(inst.Argument1))
                    use.Add(inst.Argument1);
                if (IsId(inst.Argument2) && !def.Contains(inst.Argument2))
                    use.Add(inst.Argument2);
                if (IsId(inst.Result) && !use.Contains(inst.Result))
                    def.Add(inst.Result);
            }
        }

        public override string ToString(){
            StringBuilder str = new StringBuilder();
            str.Append($"---use set---\n");
            str.Append("{");
            foreach (string i in use)
                str.Append($" {i}");
            str.Append(" }");
            str.Append($"\n\n---def set---\n");
            str.Append("{");
            foreach (string i in def)
                str.Append($" {i}");
            str.Append(" }");

            return str.ToString();
        }

    }
}
