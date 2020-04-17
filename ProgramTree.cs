using System.Collections.Generic;
using SimpleLang.Visitors;

namespace ProgramTree{
    public enum OpType { OR, AND, EQUAL, NOTEQUAL, GREATER, LESS, EQGREATER, EQLESS, PLUS, MINUS, MULT, DIV };
    public abstract class Node{
        public abstract Node Parent { get; set; }
        public abstract void Visit(Visitor v);
    }

    public abstract class ExprNode : Node {}

    public class IdNode : ExprNode{
        public override Node Parent { get; set; }
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
        public override void Visit(Visitor v){
            v.VisitIdNode(this);
        }
    }

    public class IntNumNode : ExprNode{
        public override Node Parent { get; set; }
        public int Val { get; set; }
        public IntNumNode(int num) { Val = num; }
        public override void Visit(Visitor v){
            v.VisitIntNumNode(this);
        }
    }

    public class BoolValNode : ExprNode{
        public override Node Parent { get; set; }
        public bool Val { get; set; }
        public BoolValNode(bool val) {
            Val = val;
        }
        public override void Visit(Visitor v){
            v.VisitBoolValNode(this);
        }
    }

    public class BinOpNode : ExprNode{
        public override Node Parent { get; set; }
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public OpType Op { get; set; }
        public BinOpNode(ExprNode left, ExprNode right, OpType op){
            Left = left;
            Right = right;
            Op = op;
        }
        public override void Visit(Visitor v){
            v.VisitBinOpNode(this);
        }
    }

    public abstract class StatementNode : Node {}

    public class AssignNode : StatementNode{
        public override Node Parent { get; set; }
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignNode(IdNode id, ExprNode expr){
            Id = id;
            Expr = expr;
        }
        public override void Visit(Visitor v){
            v.VisitAssignNode(this);
        }
    }

    public class WhileNode : StatementNode{
        public override Node Parent { get; set; }
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, StatementNode stat){
            Expr = expr;
            Stat = stat;
        }
        public override void Visit(Visitor v){
            v.VisitWhileNode(this);
        }
    }

    public class ForNode : StatementNode{
        public override Node Parent { get; set; }
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
        public override void Visit(Visitor v){
            v.VisitForNode(this);
        }
    }

    public class StListNode : StatementNode{
        public override Node Parent { get; set; }
        public List<StatementNode> StList = new List<StatementNode>();
        public StListNode(StatementNode stat) {
            Add(stat);
        }
        public void Add(StatementNode stat) {
            StList.Add(stat);
        }
        public override void Visit(Visitor v){
            v.VisitStListNode(this);
        }
    }

    public class IfElseNode : StatementNode{
        public override Node Parent { get; set; }
        public ExprNode Expr { get; set; }
        public StatementNode TrueStat { get; set; }
        public StatementNode FalseStat { get; set; }

        public IfElseNode(ExprNode expr, StatementNode trueSt, StatementNode falseSt = null){
            Expr = expr;
            TrueStat = trueSt;
            FalseStat = falseSt;
        }
        public override void Visit(Visitor v){
            v.VisitIfElseNode(this);
        }
    }

    public class PrintNode : StatementNode{
        public override Node Parent { get; set; }
        public ExprListNode exprList { get; set; }
        public PrintNode(ExprListNode list) { exprList = list; }
        public override void Visit(Visitor v){
            v.VisitPrintNode(this);
        }
    }

    public class InputNode : StatementNode{
        public override Node Parent { get; set; }
        public IdNode Ident { get; set; }
        public InputNode(IdNode ident) { Ident = ident; }
        public override void Visit(Visitor v){
           v.VisitInputNode(this);
        }
    }

    public class ExprListNode : ExprNode{
        public override Node Parent { get; set; }
        public List<ExprNode> exprList = new List<ExprNode>();
        public ExprListNode(ExprNode expr){
            Add(expr);
        }
        public void Add(ExprNode expr){
            exprList.Add(expr);
        }
        public override void Visit(Visitor v){
            v.VisitExprListNode(this);
        }
    }

    public class VarListNode : StatementNode{
        public override Node Parent { get; set; }
        public List<IdNode> vars = new List<IdNode>();
        public VarListNode(IdNode id){
            Add(id);
        }
        public void Add(IdNode id){
            vars.Add(id);
        }
        public override void Visit(Visitor v){
            v.VisitVarListNode(this);
        }
    }

    public class GotoNode : StatementNode {
        public override Node Parent { get; set; }
        public IntNumNode Label { get; set; }
        public GotoNode(int num) { Label = new IntNumNode(num); }
        public override void Visit(Visitor v){
            v.VisitGotoNode(this);
        }
    }

    public class LabelStatementNode : StatementNode {
        public override Node Parent { get; set; }
        public IntNumNode Label { get; set; }
        public StatementNode Stat { get; set; }
        public LabelStatementNode(int num, StatementNode stat) {
            Label = new IntNumNode(num);
            Stat = stat;
        }
        public override void Visit(Visitor v){
            v.VisitLabelstatementNode(this);
        }
    }

    public class BlockNode : StatementNode {
        public override Node Parent { get; set; }
        public StListNode List { get; set; }
        public BlockNode(StListNode st) {
            List = st;
        }
        public override void Visit(Visitor v){
            v.VisitBlockNode(this);
        }
    }
}