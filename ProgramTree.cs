using System.Collections.Generic;
using SimpleLang.Visitors;

namespace ProgramTree
{
    public enum OpType
    {
        OR,
        AND,
        EQUAL,
        NOTEQUAL,
        GREATER,
        LESS,
        EQGREATER,
        EQLESS,
        PLUS,
        MINUS,
        MULT,
        DIV
    };

    public abstract class Node
    {
        public Node Parent { get; set; }
        public List<ExprNode> ExprChildren { get; set; } = new List<ExprNode>();
        public List<StatementNode> StatChildren { get; set; } = new List<StatementNode>();
        public abstract void Visit(Visitor v);
    }

    public abstract class ExprNode : Node
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }

        public IdNode(string name)
        {
            Name = name;
        }

        public override void Visit(Visitor v)
        {
            v.VisitIdNode(this);
        }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }

        public IntNumNode(int num)
        {
            Num = num;
        }

        public override void Visit(Visitor v)
        {
            v.VisitIntNumNode(this);
        }
    }

    public class BoolValNode : ExprNode
    {
        public bool Val { get; set; }

        public BoolValNode(bool val)
        {
            Val = val;
        }

        public override void Visit(Visitor v)
        {
            v.VisitBoolValNode(this);
        }
    }

    public class BinOpNode : ExprNode
    {
        public ExprNode Left
        {
            get { return ExprChildren[0]; }
            set { ExprChildren[0] = value; }
        }

        public ExprNode Right
        {
            get { return ExprChildren[1]; }
            set { ExprChildren[1] = value; }
        }

        public OpType Op { get; set; }

        public BinOpNode(ExprNode left, ExprNode right, OpType op)
        {
            Op = op;
            ExprChildren.Add(left);
            ExprChildren.Add(right);
        }

        public override void Visit(Visitor v)
        {
            v.VisitBinOpNode(this);
        }
    }

    public abstract class StatementNode : Node
    {
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }

        public ExprNode Expr
        {
            get { return ExprChildren[0]; }
            set { ExprChildren[0] = value; }
        }

        public AssignNode(IdNode id, ExprNode expr)
        {
            Id = id;
            ExprChildren.Add(expr);
        }

        public override void Visit(Visitor v)
        {
            v.VisitAssignNode(this);
        }
    }

    public class WhileNode : StatementNode
    {
        public ExprNode Expr
        {
            get { return ExprChildren[0]; }
            set { ExprChildren[0] = value; }
        }

        public StatementNode Stat
        {
            get { return StatChildren[0]; }
            set { StatChildren[0] = value; }
        }

        public WhileNode(ExprNode expr, StatementNode stat)
        {
            ExprChildren.Add(expr);
            StatChildren.Add(stat);
        }

        public override void Visit(Visitor v)
        {
            v.VisitWhileNode(this);
        }
    }

    public class ForNode : StatementNode
    {
        public IdNode Id { get; set; }

        public ExprNode From
        {
            get { return ExprChildren[0]; }
            set { ExprChildren[0] = value; }
        }

        public ExprNode To
        {
            get { return ExprChildren[1]; }
            set { ExprChildren[1] = value; }
        }

        public StatementNode Stat
        {
            get { return StatChildren[0]; }
            set { StatChildren[0] = value; }
        }

        public ForNode(IdNode id, ExprNode from, ExprNode to, StatementNode stat)
        {
            Id = id;
            ExprChildren.Add(from);
            ExprChildren.Add(to);
            StatChildren.Add(stat);
        }

        public override void Visit(Visitor v)
        {
            v.VisitForNode(this);
        }
    }

    public class StListNode : StatementNode
    {
        public StListNode(StatementNode stat)
        {
            Add(stat);
        }

        public void Add(StatementNode stat)
        {
            StatChildren.Add(stat);
        }

        public override void Visit(Visitor v)
        {
            v.VisitStListNode(this);
        }
    }

    public class IfElseNode : StatementNode
    {
        public ExprNode Expr
        {
            get { return ExprChildren[0]; }
            set { ExprChildren[0] = value; }
        }

        public StatementNode TrueStat
        {
            get { return StatChildren[0]; }
            set { StatChildren[0] = value; }
        }

        public StatementNode FalseStat
        {
            get { return StatChildren[1]; }
            set { StatChildren[1] = value; }
        }

        public IfElseNode(ExprNode expr, StatementNode trueSt, StatementNode falseSt = null)
        {
            ExprChildren.Add(expr);
            StatChildren.Add(trueSt);
            StatChildren.Add(falseSt);
        }

        public override void Visit(Visitor v)
        {
            v.VisitIfElseNode(this);
        }
    }

    public class PrintNode : StatementNode
    {
        public ExprListNode ExprList { get; set; }

        public PrintNode(ExprListNode list)
        {
            ExprList = list;
            ExprChildren = ExprList.ExprChildren;
        }

        public override void Visit(Visitor v)
        {
            v.VisitPrintNode(this);
        }
    }

    public class InputNode : StatementNode
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
        public ExprListNode(ExprNode expr)
        {
            Add(expr);
        }

        public void Add(ExprNode expr)
        {
            ExprChildren.Add(expr);
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

    public class GotoNode : StatementNode
    {
        public IntNumNode Label { get; set; }
        public GotoNode(int num) { Label = new IntNumNode(num); }
        public override void Visit(Visitor v)
        {
            v.VisitGotoNode(this);
        }
    }

    public class LabelStatementNode : StatementNode
    {
        public IntNumNode Label { get; set; }
        public StatementNode Stat { get { return StatChildren[0]; } set { StatChildren[0] = value; } }
        public LabelStatementNode(int num, StatementNode stat)
        {
            Label = new IntNumNode(num);
            StatChildren.Add(stat);
        }
        public override void Visit(Visitor v)
        {
            v.VisitLabelstatementNode(this);
        }
    }

    public class BlockNode : StatementNode
    {
        public StListNode List { get; set; }
        public BlockNode(StListNode st)
        {
            List = st;
            StatChildren = List.StatChildren;
        }
        public override void Visit(Visitor v)
        {
            v.VisitBlockNode(this);
        }
    }
}