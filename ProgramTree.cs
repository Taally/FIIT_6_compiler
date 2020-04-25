using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SimpleLang;

namespace ProgramTree
{
    public enum OpType { OR, AND, EQUAL, NOTEQUAL, GREATER, LESS, EQGREATER, EQLESS, PLUS, MINUS, MULT, DIV };

    public static class OpTypeExtensions
    {
        public static string GetName(this OpType op)
        {
            switch (op)
            {
                case OpType.OR:
                    return "|";
                case OpType.AND:
                    return "&";
                case OpType.EQUAL:
                    return "==";
                case OpType.NOTEQUAL:
                    return "!=";
                case OpType.GREATER:
                    return ">";
                case OpType.LESS:
                    return "<";
                case OpType.EQGREATER:
                    return ">=";
                case OpType.EQLESS:
                    return "<=";
                case OpType.PLUS:
                    return "+";
                case OpType.MINUS:
                    return "-";
                case OpType.MULT:
                    return "*";
                case OpType.DIV:
                    return "/";
                default:
                    return null;
            }            
        }
    }
    public abstract class Node // базовый класс для всех узлов    
    {
        public Node Parent { get; set; }
        public abstract void Visit(Visitor v);
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {        
        public override void Visit(Visitor v)
        {
            v.VisitExprNode(this);
        }
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
        
        public override void Visit(Visitor v)
        {
            v.VisitBinOpNode(this);
        }        
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
        public override void Visit(Visitor v)
        {
            v.VisitIdNode(this);
        }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
        public override void Visit(Visitor v)
        {
            v.VisitIntNumNode(this);
        }
    }

    public class BoolNode : ExprNode
    {
        public bool Bool { get; set; }
        public BoolNode(bool b) { Bool = b; }
        public override void Visit(Visitor v)
        {
            v.VisitBoolNode(this);
        }
    }

    public class StatementNode : Node // базовый класс для всех операторов
    {
        public override void Visit(Visitor v)
        {
            v.VisitStatementNode(this);
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

        public override void Visit(Visitor v)
        {
            v.VisitBlockNode(this);
        }
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

        public override void Visit(Visitor v)
        {
            v.VisitAssignNode(this);
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

        public override void Visit(Visitor v)
        {
            v.VisitForNode(this);
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

        public override void Visit(Visitor v)
        {
            v.VisitWhileNode(this);
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

        public IfNode(ExprNode expr, StatementNode statTrue) : this(expr, statTrue, null)
        {
        }

        public override void Visit(Visitor v)
        {
            v.VisitIfNode(this);
        }
    }

    public class InputNode : StatementNode
    {
        public IdNode Id { get; set; }
        public InputNode(IdNode id)
        {
            Id = id;
        }

        public override void Visit(Visitor v)
        {
            v.VisitInputNode(this);
        }
    }
    public class ExprListNode : Node
    {
        public List<ExprNode> ExprList { get; set; }
        public ExprListNode(ExprNode expr)
        {
            ExprList = new List<ExprNode>();
            ExprList.Add(expr);
        }

        public void Add(ExprNode expr)
        {
            ExprList.Add(expr);
        }

        public override void Visit(Visitor v)
        {
            v.VisitExprListNode(this);
        }
    }

    public class PrintNode : StatementNode
    {
        public ExprListNode ExprList { get; set; }
        public PrintNode(ExprListNode exprList)
        {
            ExprList = exprList;
        }

        public override void Visit(Visitor v)
        {
            v.VisitPrintNode(this);
        }
    }

    public class VarNode : StatementNode
    {   
        public override void Visit(Visitor v)
        {
            v.VisitVarNode(this);
        }        
    }

    public class VarListNode : VarNode
    {
        public List<IdNode> VarList { get; set; }
        public VarListNode(IdNode id)
        {
            VarList = new List<IdNode>();
            VarList.Add(id);
        }
        
        public void Add(IdNode id)
        {
            VarList.Add(id);
        }

        public override void Visit(Visitor v)
        {
            v.VisitVarListNode(this);
        }
    }

    public class GotoNode : StatementNode
    {
        public IntNumNode Label;
        public GotoNode(IntNumNode label)
        {
            Label = label;
        }

        public override void Visit(Visitor v)
        {
            v.VisitGotoNode(this);
        }
    }

    public class LabelStatementNode : StatementNode
    {
        public IntNumNode Label { get; set; }
        public StatementNode Stat { get; set; }
        public LabelStatementNode(IntNumNode label, StatementNode stat)
        {
            Label = label;
            Stat = stat;
        }

        public override void Visit(Visitor v)
        {
            v.VisitLabelStatementNode(this);
        }
    }
}