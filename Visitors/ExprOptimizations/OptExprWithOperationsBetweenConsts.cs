using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class OptExprWithOperationsBetweenConsts : ChangeVisitor
    {
        public override void PostVisit(Node n)
        {
            if (n is BinOpNode binop)
            {
                switch (binop.Op)
                {
                    case OpType.LESS:
                        if (binop.Left is IntNumNode lbn && binop.Right is IntNumNode rbn)
                        {
                            ReplaceExpr(binop, new BoolValNode(lbn.Num < rbn.Num));
                            break;
                        }
                        break;

                    case OpType.GREATER:
                        if (binop.Left is IntNumNode lbn1 && binop.Right is IntNumNode rbn1)
                        {
                            ReplaceExpr(binop, new BoolValNode(lbn1.Num > rbn1.Num));
                            break;
                        }
                        break;

                    case OpType.EQGREATER:
                        if (binop.Left is IntNumNode lbn2 && binop.Right is IntNumNode rbn2)
                        {
                            ReplaceExpr(binop, new BoolValNode(lbn2.Num >= rbn2.Num));
                            break;
                        }
                        break;

                    case OpType.EQLESS:
                        if (binop.Left is IntNumNode lbn3 && binop.Right is IntNumNode rbn3)
                        {
                            ReplaceExpr(binop, new BoolValNode(lbn3.Num <= rbn3.Num));
                            break;
                        }
                        break;

                    case OpType.NOTEQUAL:
                        if (binop.Left is IntNumNode lbn4 && binop.Right is IntNumNode rbn4)
                        {
                            ReplaceExpr(binop, new BoolValNode(lbn4.Num != rbn4.Num));
                            break;
                        }

                        if (binop.Left is BoolValNode lbn5 && binop.Right is BoolValNode rbn5)
                        {
                            ReplaceExpr(binop, new BoolValNode(lbn5.Val != rbn5.Val));
                        }
                        break;
                } 
            }
        }
    }
}
