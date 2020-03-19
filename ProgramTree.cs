using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public class Node // базовый класс для всех узлов    
    {
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {
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

    public class BoolValNode : ExprNode
    {
        public bool Val { get; set; }
        public BoolValNode(bool val) { Val = val; }
    }

    public class BinOpNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public string Op { get; set; }
        public BinOpNode(ExprNode Left, ExprNode Right, string op)
        {
            this.Left = Left;
            this.Right = Right;
            this.Op = op;
        }
    }

    public class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
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

    public class ForNode : StatementNode
    {
        public IdNode Ident { get; set; }
        public IntNumNode Start { get; set; }
        public IntNumNode Finish { get; set; }
        public StatementNode Stat { get; set; }
        public ForNode(IdNode ident, int start, int finish, StatementNode stat)
        {
            Ident = ident;
            Start = new IntNumNode(start);
            Finish = new IntNumNode(finish);
            Stat = stat;
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }

    public class IfElseNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode TrueStat { get; set; }
        public StatementNode FalseStat { get; set; }

        public IfElseNode(ExprNode expr, StatementNode trueSt, StatementNode falseSt = null)
        {
            Expr = expr;
            TrueStat = trueSt;
            FalseStat = falseSt;
        }
    }

    public class PrintNode : StatementNode
    {
        public ExprListNode exprList { get; set; }
        public PrintNode(ExprListNode list)
        {
            exprList = list;
        }
    }

    public class InputNode: StatementNode
    {
        public IdNode Ident { get; set; }
        public InputNode(IdNode ident)
        {
            Ident = ident;
        }
    }

    public class ExprListNode : ExprNode
    {
        public List<ExprNode> exprList = new List<ExprNode>();
        public ExprListNode(ExprNode expr)
        {
            Add(expr);
        }
        public void Add(ExprNode expr)
        {
            exprList.Add(expr);
        }
    }

    public class VarListNode : StatementNode
    {
        public List<IdNode> vars = new List<IdNode>();
        public VarListNode(IdNode id)
        {
            Add(id);
        }
        public void Add(IdNode id)
        {
            vars.Add(id);
        }
    }

    public class EmptyNode : StatementNode
    {
    }
}