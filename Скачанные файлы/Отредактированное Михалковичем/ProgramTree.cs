using System.Collections.Generic;

namespace ProgramTree
{

    /*		
            | while
            | if
            | input
            | print
            | var
            | label
            | goto
    */

    public enum OpType { OR, AND, EQUAL, NOTEQUAL, GREATER , LESS , EQGREATER , EQLESS , PLUS , MINUS , MULT , DIV };
    public class Node // базовый класс для всех узлов    
    {
    }

    public class ExprNode : Node // базовый класс для всех выражений
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

    public class StatementNode : Node // базовый класс для всех операторов
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

    public class ForNode : StatementNode // Record-классы
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
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }

}