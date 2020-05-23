using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang{
    public class InOutSet {
        public HashSet<string> IN { get; set; }
        public HashSet<string> OUT { get; set; }

        public InOutSet() {
            IN = new HashSet<string>();
            OUT = new HashSet<string>();
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("{");
            foreach (string i in IN)
                str.Append($" {i}");
            str.Append(" } ");
            str.Append("{");
            foreach (string i in OUT)
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
    }

    public class LiveVariableAnalysis
    {
        public Dictionary<int, InOutSet> dictInOut;

        public void Execute(ControlFlowGraph cfg)
        {
            var blocks = cfg.GetCurrentBasicBlocks();
            var transferFunc = new LiveVariableTransferFunc(cfg);

            foreach (var x in blocks)
                dictInOut.Add(cfg.VertexOf(x), new InOutSet());


            bool isChanged = true;
            while (isChanged)
            {
                isChanged = false;
                for (int i = blocks.Count - 1; i >= 0; --i)
                {
                    var children = cfg.GetChildrenBasicBlocks(i);

                    dictInOut[i].OUT =
                        children
                        .Select(x => dictInOut[x.Item1].IN)
                        .Aggregate(new HashSet<string>(), (a, b) => a.Union(b).ToHashSet());

                    var pred = dictInOut[i].IN;
                    dictInOut[i].IN = transferFunc.Transfer(blocks[i], dictInOut[i].OUT);
                    isChanged = !dictInOut[i].IN.SetEquals(pred) || isChanged;
                }
            }
        }

        public class Operation : ICompareOperations<HashSet<string>>
        {
            HashSet<string> _instructions = new HashSet<string>();
            public Operation(List<Instruction> instructions)
            {
                foreach (var x in instructions)
                {
                    if (x.Operation == "assign" 
                        || x.Operation == "input" 
                        || x.Operation == "PLUS")
                        _instructions.Add(x.Result);
                }
            }

            public HashSet<string> Upper => _instructions; //???

            public HashSet<string> Lower => new HashSet<string>();

            public bool Compare(HashSet<string> a, HashSet<string> b)
                => a.SetEquals(b);

            public (HashSet<string>, HashSet<string>) Init()
                => (Lower, Lower);

            public HashSet<string> Operator(HashSet<string> a, HashSet<string> b)
                => a.Union(b).ToHashSet();
        }
    

        public LiveVariableAnalysis() {
            dictInOut = new Dictionary<int, InOutSet>();
        }

        public string ToString(ControlFlowGraph cfg){
            StringBuilder str = new StringBuilder();

            foreach (var x in cfg.GetCurrentBasicBlocks()) {
                int n = cfg.VertexOf(x);
                str.Append($"Block № {n} \n\n");
                foreach (var b in x.GetInstructions()) {
                    str.Append(b.ToString() + "\n");
                }
                str.Append($"\n\n---IN set---\n");
                str.Append("{");
                foreach (string i in dictInOut[n].IN)
                    str.Append($" {i}");
                str.Append(" }");
                str.Append($"\n\n---OUT set---\n");
                str.Append("{");
                foreach (string i in dictInOut[n].OUT)
                    str.Append($" {i}");
                str.Append(" }\n\n");
            }
            return str.ToString();
        }
    }

    public class LiveVariableTransferFunc : ITransFunc<HashSet<string>> {
        Dictionary<BasicBlock, DefUseSet> dictDefUse;

        private (HashSet<string> def, HashSet<string> use) FillDefUse(List<Instruction> block)
        {
            Func<string, bool> IsId = ThreeAddressCodeDefUse.IsId;

            HashSet<string> def = new HashSet<string>();
            HashSet<string> use = new HashSet<string>();
            for (int i = 0; i < block.Count; ++i)
            {
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

        public LiveVariableTransferFunc(ControlFlowGraph cfg) {
            var blocks = cfg.GetCurrentBasicBlocks();
            dictDefUse = new Dictionary<BasicBlock, DefUseSet>();
            foreach (var x in blocks)
                dictDefUse.Add(x, new DefUseSet(FillDefUse(x.GetInstructions())));
        }

        public HashSet<string> Transfer(BasicBlock basicBlock, HashSet<string> OUT) =>
            dictDefUse[basicBlock].use.Union(OUT.Except(dictDefUse[basicBlock].def)).ToHashSet();
    }
}