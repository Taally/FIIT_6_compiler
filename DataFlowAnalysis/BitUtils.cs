using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public static class BitUtils
    {
        public static BitArray TurnIntoBits(
            IEnumerable<Instruction> instructions,
            IDictionary<Instruction, int> map)
        {
            var result = new BitArray(map.Count);
            foreach (var instruction in instructions)
            {
                if (map.ContainsKey(instruction))
                {
                    result[map[instruction]] = true;
                }
            }
            return result;
        }

        public static IEnumerable<Instruction> TurnIntoInstructions(
            BitArray bits,
            List<Instruction> allInstructions
        )
        {
            var result = new List<Instruction>();
            for (var i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                {
                    result.Add(allInstructions[i]);
                }
            }
            return result;
        }

        public static BitArray Except(BitArray a, BitArray b)
        {
            var result = new BitArray(a.Count);
            for (var i = 0; i < a.Count; i++)
            {
                result[i] = !b[i] && a[i];
            }
            return result;
        }

        public static bool AreEqual(BitArray a, BitArray b)
        {
            for (var i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static Dictionary<BasicBlock, BitArray> GroupByBlockAndTurnIntoInstructions(
            IEnumerable<DefinitionInfo> defs,
            Dictionary<Instruction, int> idsByInstruction
        )
        {
            var result = defs
                .ToLookup(x => x.BasicBlock, x => x.Instruction)
                .ToDictionary(x => x.Key, x => TurnIntoBits(x.ToList(), idsByInstruction));

            return result;
        }
    }
}
