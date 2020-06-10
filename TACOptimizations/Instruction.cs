namespace SimpleLang
{
    public class Instruction
    {
        public string Label { get; internal set; }
        public string Operation { get; }
        public string Argument1 { get; }
        public string Argument2 { get; }
        public string Result { get; }

        public Instruction(string label, string operation, string argument1, string argument2, string result)
        {
            Label = label;
            Operation = operation;
            Argument1 = argument1;
            Argument2 = argument2;
            Result = result;
        }

        public Instruction Copy() => new Instruction(Label, Operation, Argument1, Argument2, Result);

        public override string ToString()
        {
            var label = Label != "" ? Label + ": " : "";
            switch (Operation)
            {
                case "assign":
                    return label + Result + " = " + Argument1;
                case "ifgoto":
                    return $"{label}if {Argument1} goto {Argument2}";
                case "goto":
                    return $"{label}goto {Argument1}";
                case "input":
                    return $"{label}input {Result}";
                case "print":
                    return $"{label}print {Argument1}";
                case "NOT":
                case "UNMINUS":
                    return $"{label}{Result} = {ConvertToMathNotation(Operation)}{Argument1}";
                case "OR":
                case "AND":
                case "EQUAL":
                case "NOTEQUAL":
                case "LESS":
                case "GREATER":
                case "EQGREATER":
                case "EQLESS":
                case "PLUS":
                case "MINUS":
                case "MULT":
                case "DIV":
                    return $"{label}{Result} = {Argument1} {ConvertToMathNotation(Operation)} {Argument2}";
                case "noop":
                    return $"{label}noop";
                default:
                    return $"label: {Label}; op {Operation}; arg1: {Argument1}; arg2: {Argument2}; res: {Result}";
            }
        }

        private string ConvertToMathNotation(string operation)
        {
            switch (operation)
            {
                case "OR":
                    return "or";
                case "AND":
                    return "and";
                case "EQUAL":
                    return "==";
                case "NOTEQUAL":
                    return "!=";
                case "LESS":
                    return "<";
                case "GREATER":
                    return ">";
                case "EQGREATER":
                    return ">=";
                case "EQLESS":
                    return "<=";
                case "PLUS":
                    return "+";
                case "MINUS":
                case "UNMINUS":
                    return "-";
                case "MULT":
                    return "*";
                case "DIV":
                    return "/";
                case "NOT":
                    return "!";
                default:
                    return operation;
            }
        }
    }
}
