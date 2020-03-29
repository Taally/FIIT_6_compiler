using System.Collections.Generic;

namespace ProgramTree
{
    public enum OpType { OR, AND, EQUAL, NOTEQUAL, GREATER, LESS, EQGREATER, EQLESS, PLUS, MINUS, MULT, DIV };
    public class Node // base class for all nodes 
    {
    }

    public class ExprNode : Node // base class for all expressions
    {
    }

    public class BinOpNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public OpType Op { get; set; }
        public BinOpNode(ExprNode left, ExprNode right, OpType op)
        {
            Left = left;
            Right = right;
            Op = op;
        }
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
    }

    public class StatementNode : Node // base class for all statements
    {
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignNode(IdNode id, ExprNode expr)
        {
            Id = id;
            Expr = expr;
        }
    }

    public class ForNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode From { get; set; }
        public ExprNode To { get; set; }
        public StatementNode Stat { get; set; }
        public ForNode(IdNode id, ExprNode from, ExprNode to, StatementNode stat)
        {
            Id = id;
            From = from;
            To = to;
            Stat = stat;
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();

        public BlockNode(StatementNode stat) => Add(stat);

        public void Add(StatementNode stat) => StList.Add(stat);
    }

    public class WhileNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode StatTrue { get; set; }

        public StatementNode StatFalse { get; set; }

        public IfNode(ExprNode expr, StatementNode statTrue, StatementNode statFalse)
        {
            Expr = expr;
            StatTrue = statTrue;
            StatFalse = statFalse;
        }

        public IfNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            StatTrue = stat;
            StatFalse = null;
        }
    }

    public class InputNode : StatementNode
    {
        public IdNode Id { get; set; }

        public InputNode(IdNode id) => Id = id;
    }

    public class ExprListNode : ExprNode
    {
        public List<ExprNode> ExprList = new List<ExprNode>();

        public ExprListNode(ExprNode expr) => Add(expr);

        public void Add(ExprNode expr) => ExprList.Add(expr);
    }

    public class PrintNode : StatementNode
    {
        public ExprListNode ExprList { get; set; }

        public PrintNode(ExprListNode exprList) => ExprList = exprList;
    }

    public class IdListNode : StatementNode
    {
        public List<ExprNode> ExprList = new List<ExprNode>();

        public IdListNode(ExprNode expr) => Add(expr);

        public void Add(ExprNode expr) => ExprList.Add(expr);
    }

    public class VarNode : StatementNode
    {
        public IdListNode IdList { get; set; }

        public VarNode(IdListNode idList) => IdList = idList;
    }

    public class GoToNode : StatementNode
    {
        public IntNumNode Label { get; set; }

        public GoToNode(int label) => Label = new IntNumNode(label);
    }

    public class LabelStatementNode : StatementNode
    {
        public IntNumNode Label { get; set; }

        public StatementNode Stat { get; set; }

        public LabelStatementNode(int label, StatementNode stat)
        {
            Label = new IntNumNode(label);
            Stat = stat;
        }
    }

    public class BoolValNode : ExprNode
    {
        public bool Val { get; set; }

        public BoolValNode(bool val) { Val = val; }
    }
}