using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    class FillParentVisitor : AutoVisitor
    {
        Stack<Node> stack = new Stack<Node>(); 
        public FillParentVisitor()
        {
            stack.Push(null);
        }

        public override void VisitExprNode(ExprNode exprNode) 
        {
            exprNode.Parent = stack.Peek();
            stack.Push(exprNode);
            base.VisitExprNode(exprNode);
            stack.Pop();
        }
        public override void VisitBinOpNode(BinOpNode binop) 
        {
            binop.Parent = stack.Peek();
            stack.Push(binop);
            base.VisitBinOpNode(binop);
            stack.Pop();
        }
        public override void VisitIdNode(IdNode id) 
        {
            id.Parent = stack.Peek();
            stack.Push(id);
            base.VisitIdNode(id);
            stack.Pop();
        }
        public override void VisitIntNumNode(IntNumNode num) 
        {
            num.Parent = stack.Peek();
            stack.Push(num);
            base.VisitIntNumNode(num);
            stack.Pop();
        }
        public override void VisitBoolNode(BoolNode b) 
        {
            b.Parent = stack.Peek();
            stack.Push(b);
            base.VisitBoolNode(b);
            stack.Pop();
        }
        public override void VisitStatementNode(StatementNode stat) 
        {
            stat.Parent = stack.Peek();
            stack.Push(stat);
            base.VisitStatementNode(stat);
            stack.Pop();
        }
        public override void VisitBlockNode(BlockNode block) 
        {
            block.Parent = stack.Peek();
            stack.Push(block);
            base.VisitBlockNode(block);
            stack.Pop();
        }
        public override void VisitAssignNode(AssignNode a) 
        {
            a.Parent = stack.Peek();            
            stack.Push(a);
            base.VisitAssignNode(a);
            stack.Pop();

        }
        public override void VisitForNode(ForNode forNode)
        {
            forNode.Parent = stack.Peek();
            stack.Push(forNode);
            base.VisitForNode(forNode);
            stack.Pop();
        }
        public override void VisitWhileNode(WhileNode whileNode)
        {
            whileNode.Parent = stack.Peek();
            stack.Push(whileNode);
            base.VisitWhileNode(whileNode);
            stack.Pop();
        }
        public override void VisitIfNode(IfNode ifNode) 
        {
            ifNode.Parent = stack.Peek();
            stack.Push(ifNode);
            base.VisitIfNode(ifNode);
            stack.Pop();
        }
        public override void VisitInputNode(InputNode inputNode)
        {
            inputNode.Parent = stack.Peek();
            stack.Push(inputNode);
            base.VisitInputNode(inputNode);
            stack.Pop();
        }
        public override void VisitExprListNode(ExprListNode exprListNode) 
        {
            exprListNode.Parent = stack.Peek();
            stack.Push(exprListNode);
            base.VisitExprListNode(exprListNode);
            stack.Pop();
        }
        public override void VisitPrintNode(PrintNode printNode)
        {
            printNode.Parent = stack.Peek();
            stack.Push(printNode);
            base.VisitPrintNode(printNode);
            stack.Pop();
        }
        public override void VisitVarNode(VarNode varNode)
        {
            varNode.Parent = stack.Peek();
            stack.Push(varNode);
            base.VisitVarNode(varNode);
            stack.Pop();
        }
        public override void VisitVarListNode(VarListNode varListNode) 
        {
            varListNode.Parent = stack.Peek();
            stack.Push(varListNode);
            base.VisitVarListNode(varListNode);
            stack.Pop();
        }
        public override void VisitGotoNode(GotoNode gotoNode) 
        {
            gotoNode.Parent = stack.Peek();
            stack.Push(gotoNode);
            base.VisitGotoNode(gotoNode);
            stack.Pop();
        }
        public override void VisitLabelStatementNode(LabelStatementNode labelStatementNode) 
        {
            labelStatementNode.Parent = stack.Peek();
            stack.Push(labelStatementNode);
            base.VisitLabelStatementNode(labelStatementNode);
            stack.Pop();
        }
    }
}
