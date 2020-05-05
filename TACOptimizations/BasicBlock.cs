using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleLang
{
    public class BasicBlock
    {
        private List<Instruction> _instructions;
        
        public BasicBlock()
        {
            _instructions = new List<Instruction>();
        }
        
        public BasicBlock(List<Instruction> instructions)
        {
            _instructions = instructions;
        }

        public ReadOnlyCollection<Instruction> GetInstructions => _instructions.AsReadOnly();

        public void InsertInstruction(int index, Instruction instruction)
        {
            _instructions.Insert(index, instruction);
        }
        
        public void AddInstruction(Instruction instruction)
        {
            _instructions.Add(instruction);
        }
        
        public void InsertRangeOfInstructions(int index, List<Instruction> instruction)
        {
            _instructions.InsertRange(index, instruction);
        }
        
        public void AddRangeOfInstructions(List<Instruction> instruction)
        {
            _instructions.AddRange(instruction);
        }

        public void RemoveInstructionByIndex(int index)
        {
            _instructions.RemoveAt(index);
        }
    }
}