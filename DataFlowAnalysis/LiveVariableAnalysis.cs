using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    class InOutSet {
        public HashSet<string> IN { get; set; }
        public HashSet<string> OUT { get; set; }

        public InOutSet() {
            IN = new HashSet<string>();
            OUT = new HashSet<string>();
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


    class LiveVariableAnalysis{
        Stack<BasicBlock> st; //надо как-то по-солиднее назвать
        Dictionary<int, InOutSet> dictInOut; //надо как-то по-солиднее назвать
        Dictionary<int, DefUseSet> dictDefUse;

        public void Execute(ControlFlowGraph cfg) {
            //Находим последний блок, записываем его родителей в стек, его пустой IN в словарь
            var cur = cfg.VertexOf(cfg.GetCurrentBasicBlocks().Last());
            dictInOut.Add(cur, new InOutSet());
            dictDefUse.Add(cur, new DefUseSet());
            //Запихнули всех родителей в стек
            foreach (var (i,block) in cfg.GetParentsBasicBlocks(cur)) {
                st.Push(block);
            }

            var currentBlock = st.Peek();

            while (true) {
                //Пока есть родители у блока, подумать над правильностью
                while (true) {
                    int num = cfg.VertexOf(currentBlock);

                    //Если этот блок есть в словаре - мы его не вычисляем
                    if (dictInOut.ContainsKey(num)) {
                        st.Pop();
                        currentBlock = st.Peek();
                        continue;
                    }
                       
                    var children = cfg.GetChildrenBasicBlocks(num);

                    //Если все дети вычислены
                    if (children.All(x => dictInOut.ContainsKey(x.Item1)))
                    {
                        dictInOut.Add(num, new InOutSet());
                        //Вычисляем OUT текущего блока
                        dictInOut[num].OUT =
                            children
                            .Select(x => dictInOut[x.Item1].IN)
                            .Aggregate(new HashSet<string>(), (a, b) => a.Union(b).ToHashSet());

                        //Вычисляем def-use текущего блока
                        dictDefUse.Add(num,
                            new DefUseSet(FillDefUse(currentBlock.GetInstructions())));

                        //Вычисляем IN текущего блока
                        dictInOut[num].IN =
                            (dictInOut[num].IN.Union(dictDefUse[num].use))
                            .Union(dictInOut[num].OUT.Except(dictDefUse[num].def))
                            .ToHashSet();
                        st.Pop();
                        //Если нет родителей - выходим из цикла и проверяем, входной это блок или нет
                        if (cfg.GetParentsBasicBlocks(num).Count == 0)
                            break;
                        //Если родители есть - добавляем в стек
                        foreach (var (i, block) in cfg.GetParentsBasicBlocks(num))
                            st.Push(block);
                    }
                    //Если дети не вычислены
                    else {
                        //Ищем кто не вычислен и запихиваем в стек
                        foreach (var ch in children.Where(x => !dictInOut.ContainsKey(x.Item1)))
                            st.Push(ch.Item2); 
                    }

                    currentBlock = st.Peek();
                }

                //Проверяем, входной это блок или нет. Если да - значит информация построена вся
                if (currentBlock == cfg.GetCurrentBasicBlocks().First())
                    break;
                //Если нет, значит цепочка оборвана. Либо мы берем следующий блок из стека
                if (st.Count != 0)
                    currentBlock = st.Peek();
                //Иначе ищем по блокам последний из тех, кто еще не вычислен
                else {
                    currentBlock = cfg.GetCurrentBasicBlocks()
                        .Where(x => !dictInOut.ContainsKey(cfg.VertexOf(x)))
                        .LastOrDefault();
                    if (currentBlock == default(BasicBlock))
                        throw new Exception("А так может быть??");
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

            foreach (var x in cfg.GetCurrentBasicBlocks()
                .Skip(1)
                .Take(cfg.GetCurrentBasicBlocks().Count-2)) {
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
}
