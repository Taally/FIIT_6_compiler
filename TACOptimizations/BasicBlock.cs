using System.Collections.Generic;
using System.Linq;

namespace SimpleLang
{
    public class BasicBlock
    {
        private readonly List<Instruction> _instructions;

        public BasicBlock() => _instructions = new List<Instruction>();

        public BasicBlock(List<Instruction> instructions) => _instructions = instructions;
        public BasicBlock(IEnumerable<Instruction> instructions) => _instructions = instructions.ToList();

        public IReadOnlyList<Instruction> GetInstructions() => _instructions;

        public void InsertInstruction(int index, Instruction instruction) => _instructions.Insert(index, instruction);

        public void AddInstruction(Instruction instruction) => _instructions.Add(instruction);

        public void InsertRangeOfInstructions(int index, IEnumerable<Instruction> instruction) => _instructions.InsertRange(index, instruction);

        public void AddRangeOfInstructions(IEnumerable<Instruction> instruction) => _instructions.AddRange(instruction);

        public void RemoveInstructionByIndex(int index) => _instructions.RemoveAt(index);

        public void ClearInstructions() => _instructions.Clear();
    }
}
