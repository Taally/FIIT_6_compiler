using System.Collections.Generic;
using SimpleLang.Visitors;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public abstract class Node // базовый класс для всех узлов    
    {
        public abstract void Visit(Visitor v);
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
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

    public class BoolValNode : ExprNode
    {
        public bool Val { get; set; }
        public BoolValNode(bool val) { Val = val; }
        public override void Visit(Visitor v)
        {
            v.VisitBoolValNode(this);
        }
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
        public override void Visit(Visitor v)
        {
            v.VisitBinOpNode(this);
        }
    }

    public abstract class StatementNode : Node // базовый класс для всех операторов
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
        public override void Visit(Visitor v)
        {
            v.VisitAssignNode(this);
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
        public override void Visit(Visitor v)
        {
            v.VisitForNode(this);
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
        public override void Visit(Visitor v)
        {
            v.VisitIfElseNode(this);
        }
    }

    public class PrintNode : StatementNode
    {
        public ExprListNode exprList { get; set; }
        public PrintNode(ExprListNode list)
        {
            exprList = list;
        }
        public override void Visit(Visitor v)
        {
            v.VisitPrintNode(this);
        }
    }

    public class InputNode: StatementNode
    {
        public IdNode Ident { get; set; }
        public InputNode(IdNode ident)
        {
            Ident = ident;
        }
        public override void Visit(Visitor v)
        {
            v.VisitInputNode(this);
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
        public override void Visit(Visitor v)
        {
            v.VisitExprListNode(this);
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
        public override void Visit(Visitor v)
        {
            v.VisitVarListNode(this);
        }
    }

    public class EmptyNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitEmptyNode(this);
        }
    }
}