using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class InOutSet
    {
        public HashSet<string> In { get; set; }
        public HashSet<string> Out { get; set; }

        public InOutSet() {
            In = new HashSet<string>();
            Out = new HashSet<string>();
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("{");
            foreach (string i in In)
                str.Append($" {i}");
            str.Append(" } ");
            str.Append("{");
            foreach (string i in Out)
                str.Append($" {i}");
            str.Append(" } ");

            return str.ToString();
        }
    }

    class DefUseSet
    {
        public HashSet<string> def { get; set; }
        public HashSet<string> use { get; set; }

        public DefUseSet()
        {
            def = new HashSet<string>();
            use = new HashSet<string>();
        }
        public DefUseSet((HashSet<string> def, HashSet<string> use) a) {
            def = a.def;
            use = a.use;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("{");
            foreach (string i in def)
                str.Append($" {i}");
            str.Append(" } ");
            str.Append("{");
            foreach (string i in use)
                str.Append($" {i}");
            str.Append(" } ");

            return str.ToString();
        }
    }


    public class LiveVariableAnalysis{
        Stack<BasicBlock> st; 
        public Dictionary<int, InOutSet> dictInOut; 
        Dictionary<int, DefUseSet> dictDefUse;

        public void Execute(ControlFlowGraph cfg) {
            var blocks = cfg.GetCurrentBasicBlocks();

            foreach (var x in blocks) {
                int n = cfg.VertexOf(x);
                dictInOut.Add(n,new InOutSet());
                dictDefUse.Add(n, new DefUseSet(FillDefUse(x.GetInstructions())));
            }

            bool isChanged = true;
            while (isChanged){
                isChanged = false;
                for (int i = blocks.Count - 1; i >= 0; --i){
                    var children = cfg.GetChildrenBasicBlocks(i);

                    dictInOut[i].Out =
                        children
                        .Select(x => dictInOut[x.Item1].In)
                        .Aggregate(new HashSet<string>(), (a, b) => a.Union(b).ToHashSet());

                    var pred = dictInOut[i].In;

                    dictInOut[i].In =
                        (new HashSet<string>().Union(dictDefUse[i].use))
                        .Union(dictInOut[i].Out.Except(dictDefUse[i].def))
                        .ToHashSet();

                    isChanged = !dictInOut[i].In.SetEquals(pred) || isChanged;
                }
            }

        }

        public LiveVariableAnalysis() {
            st = new Stack<BasicBlock>();
            dictInOut = new Dictionary<int, InOutSet>();
            dictDefUse = new Dictionary<int, DefUseSet>();
        }

        Func<string, bool> IsId = ThreeAddressCodeDefUse.IsId;

        public (HashSet<string> def, HashSet<string> use) FillDefUse(List<Instruction> block) {
            HashSet<string> def = new HashSet<string>();
            HashSet<string> use = new HashSet<string>();
            for (int i = 0; i < block.Count; ++i) {
                var inst = block[i];

                if (IsId(inst.Argument1) && !def.Contains(inst.Argument1))
                    use.Add(inst.Argument1);
                if (IsId(inst.Argument2) && !def.Contains(inst.Argument2))
                    use.Add(inst.Argument2);
                if (IsId(inst.Result) && !use.Contains(inst.Result))
                    def.Add(inst.Result);
            }
            return (def, use);
        }

        public string ToString(ControlFlowGraph cfg){
            StringBuilder str = new StringBuilder();

            foreach (var x in cfg.GetCurrentBasicBlocks()) {
                int n = cfg.VertexOf(x);
                str.Append($"Block № {n} \n\n");
                foreach (var b in x.GetInstructions()) {
                    str.Append(b.ToString() + "\n");
                }
                str.Append($"\n\n---use set---\n");
                str.Append("{");
                foreach (string i in dictDefUse[n].use)
                    str.Append($" {i}");
                str.Append(" }");
                str.Append($"\n\n---def set---\n");
                str.Append("{");
                foreach (string i in dictDefUse[n].def)
                    str.Append($" {i}");
                str.Append(" }");
                str.Append($"\n\n---IN set---\n");
                str.Append("{");
                foreach (string i in dictInOut[n].In)
                    str.Append($" {i}");
                str.Append(" }");
                str.Append($"\n\n---OUT set---\n");
                str.Append("{");
                foreach (string i in dictInOut[n].Out)
                    str.Append($" {i}");
                str.Append(" }\n\n");
            }
            return str.ToString();
        }
    }
}
