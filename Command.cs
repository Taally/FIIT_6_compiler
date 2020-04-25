using ProgramTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class Command
    {
        public string Op { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public string Result { get; set; }
        public string Label { get; set; }
        
        Command(string op, string arg1, string arg2, string result, string label)
        {
            Op = op;
            Arg1 = arg1;
            Arg2 = arg2;
            Result = result;
            Label = label;
        }

        public static Command GenCommand(string op, string arg1, string arg2, string result, string label)
        {
            return new Command(op, arg1, arg2, result, label);
        }

        public override string ToString()
        {
            string labelText = "";
            if (Label != "")
                labelText = Label + " : ";
            switch (Op)
            {
                case "assign":                    
                    return labelText + Result + " = " + Arg1 + ";";
                default:
                    return labelText + Result + " = " + Arg1 + " " + Op + " " + Arg2 + ";";        
            }
        }
    }
}
