using System.Collections.Generic;

namespace ProgramTree{
    public enum OpType { OR, AND, EQUAL, NOTEQUAL, GREATER, LESS, EQGREATER, EQLESS, PLUS, MINUS, MULT, DIV };
    public abstract class Node{
    }

    public abstract class ExprNode : Node {}

    public class IdNode : ExprNode{
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
    }

    public class IntNumNode : ExprNode{
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
    }

    public class BoolValNode : ExprNode{
        public bool Val { get; set; }
        public BoolValNode(bool val) { Val = val; }
    }

    public class BinOpNode : ExprNode{
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public OpType Op { get; set; }
        public BinOpNode(ExprNode left, ExprNode right, OpType op){
            Left = left;
            Right = right;
            Op = op;
        }
    }

    public abstract class StatementNode : Node {}

    public class AssignNode : StatementNode{
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignNode(IdNode id, ExprNode expr){
            Id = id;
            Expr = expr;
        }
    }

    public class WhileNode : StatementNode{
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, StatementNode stat){
            Expr = expr;
            Stat = stat;
        }
    }

    public class ForNode : StatementNode{
        public IdNode Id { get; set; }
        public ExprNode From { get; set; }
        public ExprNode To { get; set; }
        public StatementNode Stat { get; set; }
        public ForNode(IdNode id, ExprNode from, ExprNode to, StatementNode stat){
            Id = id;
            From = from;
            To = to;
            Stat = stat;
        }
    }

    public class StListNode : StatementNode{
        public List<StatementNode> StList = new List<StatementNode>();
        public StListNode(StatementNode stat) {
            Add(stat);
        }
        public void Add(StatementNode stat) {
            StList.Add(stat);
        }
    }

    public class IfElseNode : StatementNode{
        public ExprNode Expr { get; set; }
        public StatementNode TrueStat { get; set; }
        public StatementNode FalseStat { get; set; }

        public IfElseNode(ExprNode expr, StatementNode trueSt, StatementNode falseSt = null){
            Expr = expr;
            TrueStat = trueSt;
            FalseStat = falseSt;
        }
    }

    public class PrintNode : StatementNode{
        public ExprListNode exprList { get; set; }
        public PrintNode(ExprListNode list) { exprList = list; }
    }

    public class InputNode : StatementNode{
        public IdNode Ident { get; set; }
        public InputNode(IdNode ident) { Ident = ident; }
    }

    public class ExprListNode : ExprNode{
        public List<ExprNode> exprList = new List<ExprNode>();
        public ExprListNode(ExprNode expr){
            Add(expr);
        }
        public void Add(ExprNode expr){
            exprList.Add(expr);
        }
    }

    public class VarListNode : StatementNode{
        public List<IdNode> vars = new List<IdNode>();
        public VarListNode(IdNode id){
            Add(id);
        }
        public void Add(IdNode id){
            vars.Add(id);
        }
    }

    public class GotoNode : StatementNode {
        public IntNumNode Label { get; set; }
        public GotoNode(int num) { Label = new IntNumNode(num); }
    }

    public class LabelStatementNode : StatementNode {
        public IntNumNode Label { get; set; }
        public StatementNode Stat { get; set; }
        public LabelStatementNode(int num, StatementNode stat) {
            Label = new IntNumNode(num);
            Stat = stat;
        }
    }
	
	public class BlockNode : StatementNode {
        public StListNode List { get; set; }
        public BlockNode(StListNode st) {
            List = st;
        }
    }
}