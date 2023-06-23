using Cx.Core.DataTools;
using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    //public enum ExpressionSegmentType
    //{
    //    NA,Segment,TreeNode,ESTreeNode,ESContext
    //}
    public class ExpressionTreeNode : TreeNode
    {
        public List<ExpressionSegment> ExpressionChildren = new List<ExpressionSegment>();
        public bool IsFormed = false;
    }
    public class ExpressionParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            {
                var TER = TerminateExpression(provider , context);
                if (FinalResult.CheckAndInheritAbnormalities(TER)) return FinalResult;
                var _newContext = TER.Result;

                var FPR = FirstPass_SubstituteCalls(provider , _newContext);
                if (FinalResult.CheckAndInheritAbnormalities(FPR)) return FinalResult;
                if (FPR.Result == null)
                {
                    FinalResult.AddError(new ParseFailError(_newContext.HEAD , ASTNodeType.Expression));
                    return FinalResult;
                }
                ExpressionSegmentContext ES_Context = new ExpressionSegmentContext(FPR.Result);
                ParseExpressionTree(provider , ES_Context);
            }
            return FinalResult;
        }
        public OperationResult<TreeNode> ParseExpressionTree(ParserProvider provider , ExpressionSegmentContext context)
        {
            TreeNode Node = new ExpressionTreeNode();
            Node.Type = ASTNodeType.Expression;
            OperationResult<TreeNode> FinalResult = new OperationResult<TreeNode>(Node);
            var SP_C_R = SecondPass_Closures(provider , context);
            if (FinalResult.CheckAndInheritAbnormalities(SP_C_R)) return FinalResult;
            {
                var CPBER = CustomPass_RightHandSideUnaryExpression(provider , context , ExpressionSymbols.RightHand_Unary_0st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
            {
                var CPBER = CustomPass_BinaryExpression(provider , context , ExpressionSymbols.Binary_0st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
            {
                var CPBER = CustomPass_BinaryExpression(provider , context , ExpressionSymbols.Binary_1st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
            {
                var CPBER = CustomPass_BinaryExpression(provider , context , ExpressionSymbols.Binary_2st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
            return FinalResult;
        }
        public OperationResult<SegmentContext> TerminateExpression(ParserProvider provider , SegmentContext context)
        {
            var HEAD = context.Current;
            var ENDPOINT = context.EndPoint;
            int PairCount = 0;
            var Terminator = ExpressionSymbols.Termination;
            while (true)
            {
                if (context.ReachEnd)
                {
                    break;
                }
                var match = context.MatchCollection(true , Terminator);
                if (match.Item1 == MatchResult.Match)
                {
                    if (PairCount <= 0)
                    {
                        break;
                    }
                }
                var MR = context.Match("(");
                if (MR == MatchResult.Match)
                {
                    PairCount++;
                }
                else
                {
                    MR = context.Match(")");
                    if (MR == MatchResult.Match)
                        PairCount--;
                }
                context.GoNext();
            }
            SegmentContext Result = new SegmentContext(HEAD);
            Result.SetEndPoint(ENDPOINT);
            return Result;
        }

        public OperationResult<bool> CustomPass_BinaryExpression(ParserProvider provider , ExpressionSegmentContext context , params string [ ] symbols)
        {
            OperationResult<bool> FinalResult = new OperationResult<bool>(false);
            while (true)
            {
                if (context.IsReachEnd) break;
                var ESMR = context.MatchCollection(symbols);
                if (ESMR == ESCMatchResult.Match)
                {

                    ExpressionTreeNode expressionTreeNode = new ExpressionTreeNode();
                    expressionTreeNode.Segment = context.Current?.Segment;
                    expressionTreeNode.Type = ASTNodeType.BinaryExpression;
                    ExpressionSegment? prev = context.Prev;
                    ExpressionSegment? next = context.Next;
                    if (prev == null)
                    {
                        FinalResult.AddError(new BinaryExpressionMissingPartError(context.Current?.Segment , true));
                        return FinalResult;
                    }

                    if (next == null)
                    {
                        FinalResult.AddError(new BinaryExpressionMissingPartError(context.Current?.Segment , false));
                        return FinalResult;
                    }
                    if(!prev.IsOkayForExpressionPart|| !next.IsOkayForExpressionPart)
                    {
                        context.GoNext();
                        continue;
                    }
                    ExpressionSegment NewSegment = new ExpressionSegment();
                    NewSegment.Node = expressionTreeNode;
                    expressionTreeNode.ExpressionChildren.Add(prev);
                    expressionTreeNode.ExpressionChildren.Add(next);
                    context.SubstituteSegment_0(prev.Prev , next.Next , NewSegment);
                }
                context.GoNext();
            }
            FinalResult.Result = true;
            return FinalResult;
        }

        public OperationResult<bool> CustomPass_RightHandSideUnaryExpression(ParserProvider provider , ExpressionSegmentContext context , params string [ ] symbols)
        {
            OperationResult<bool> FinalResult = new OperationResult<bool>(false);
            while (true)
            {
                if (context.IsReachEnd) break;
                var ESMR = context.MatchCollection(symbols);
                if (ESMR == ESCMatchResult.Match)
                {

                    ExpressionTreeNode expressionTreeNode = new ExpressionTreeNode();
                    expressionTreeNode.Segment = context.Current?.Segment;
                    expressionTreeNode.Type = ASTNodeType.BinaryExpression;
                    ExpressionSegment? prev = context.Prev;
                    ExpressionSegment? next = context.Next;
                    bool isHit = false;
                    if (prev == null)
                    {
                        isHit = true;
                    }
                    else
                    {
                        if (prev.Segment != null)
                        {
                            var dt = DataTypeChecker.DetermineDataType(prev.Segment.content);
                            if (dt == DataType.String || dt == DataType.IntegerAny || dt == DataType.DecimalAny)
                            {
                                isHit = true;
                            }
                        }
                    }

                    if (next == null)
                    {
                        FinalResult.AddError(new UnaryExpressionMissingPartError(context.Current?.Segment));
                        return FinalResult;
                    }
                    if (isHit)
                    {
                        ExpressionSegment NewSegment = new ExpressionSegment();
                        NewSegment.Node = expressionTreeNode;
                        expressionTreeNode.ExpressionChildren.Add(next);
                        context.SubstituteSegment_0(prev , next.Next , NewSegment);
                    }
                }
                context.GoNext();
            }
            FinalResult.Result = true;
            return FinalResult;
        }

        public OperationResult<bool> CustomPass_LeftHandSideUnaryExpression(ParserProvider provider , ExpressionSegmentContext context , params string [ ] symbols)
        {
            OperationResult<bool> FinalResult = new OperationResult<bool>(false);
            while (true)
            {
                if (context.IsReachEnd) break;
                var ESMR = context.MatchCollection(symbols);
                if (ESMR == ESCMatchResult.Match)
                {

                    ExpressionTreeNode expressionTreeNode = new ExpressionTreeNode();
                    expressionTreeNode.Segment = context.Current?.Segment;
                    expressionTreeNode.Type = ASTNodeType.BinaryExpression;
                    ExpressionSegment? prev = context.Prev;
                    ExpressionSegment? next = context.Next;
                    bool isHit = false;
                    if (prev == null)
                    {
                        FinalResult.AddError(new UnaryExpressionMissingPartError(context.Current?.Segment));
                        return FinalResult;
                    }
                    {
                        string content = prev.Segment.content;
                        var dt = DataTypeChecker.DetermineDataType(content);
                        if (dt == DataType.String || dt == DataType.IntegerAny || dt == DataType.DecimalAny)
                        {
                            isHit = true;
                        }
                    }

                    if (next == null)
                    {
                    }
                    //if (isHit)
                    {
                        ExpressionSegment NewSegment = new ExpressionSegment();
                        NewSegment.Node = expressionTreeNode;
                        expressionTreeNode.ExpressionChildren.Add(prev);
                        context.SubstituteSegment_0(prev.Prev , next , NewSegment);
                    }
                }
                context.GoNext();
            }
            FinalResult.Result = true;
            return FinalResult;
        }
        public OperationResult<bool> SecondPass_Closures(ParserProvider provider , ExpressionSegmentContext context)
        {
            OperationResult<bool> FinalResult = new OperationResult<bool>(false);
            ExpressionSegment? FirstParentheses_L = null;
            int ParenthesesDepth = 0;
            while (true)
            {
                if (context.IsReachEnd) break;
                if (context.Match("(") == ESCMatchResult.Match)
                {
                    if (FirstParentheses_L == null)
                    {
                        FirstParentheses_L = context.Current;
                    }
                    ParenthesesDepth++;
                }
                if (context.Match(")") == ESCMatchResult.Match)
                {
                    if (FirstParentheses_L == null)
                    {
                        FinalResult.AddError(new ClosureError(context.Current?.Segment , "(" , ")"));
                        return FinalResult;
                    }

                    ParenthesesDepth--;
                    if (ParenthesesDepth == 0)
                    {
                        ExpressionSegment Closure = new ExpressionSegment();
                        ExpressionSegmentContext Closed = new ExpressionSegmentContext(FirstParentheses_L.Next!);
                        Closure.Context = Closed;
                        var R = context.Current;
                        context.GoNext();
                        context.SubstituteSegment_0(FirstParentheses_L?.Prev , R?.Next , Closure);
                        //Recursive Parse.
                        var CResult = ParseExpressionTree(provider , Closed);
                        if (FinalResult.CheckAndInheritAbnormalities(CResult)) return FinalResult;
                        var nt = CResult.Result;
                        Closure.Node = nt;
                        FirstParentheses_L = null;
                    }
                }
            }
            FinalResult.Result = true;
            return FinalResult;
        }
        public OperationResult<ExpressionSegment?> FirstPass_SubstituteCalls(ParserProvider provider , SegmentContext context)
        {
            OperationResult<ExpressionSegment?> FinalResult = new OperationResult<ExpressionSegment?>(null);
            var HEAD = context.Current;

            var CallParser = provider.GetParser(ASTNodeType.Call);
            if (CallParser == null)
            {
                FinalResult.AddError(new ParserNotFoundError(HEAD , ASTNodeType.Call));
                return FinalResult;
            }
            TreeNode Reciver = new TreeNode();
            ExpressionSegment? ES_HEAD = null;
            ExpressionSegment? Current = null;
            int ID = 0;
            while (true)
            {
                if (context.ReachEnd) break;
                var CPR = CallParser.Parse(provider , context , Reciver);
                if (FinalResult.CheckAndInheritAbnormalities(CPR)) { return FinalResult; }
                ExpressionSegment new_segment = new ExpressionSegment();
                new_segment.ID = ID;
                if (CPR.Result)
                {
                    var NewNode = Reciver.Children [ 0 ];
                    Reciver.Children.Clear();
                    new_segment.Node = NewNode;
                    if (ES_HEAD == null || Current == null)
                    {
                        ES_HEAD = new_segment;
                        Current = new_segment;
                    }
                    else
                    {
                        Current.AttachNext(new_segment);
                        Current = new_segment;
                    }
                }
                else
                {
                    new_segment.Segment = context.Current;
                    if (ES_HEAD == null || Current == null)
                    {
                        ES_HEAD = new_segment;
                        Current = new_segment;
                    }
                    else
                    {
                        Current.AttachNext(new_segment);
                        Current = new_segment;
                    }
                    context.GoNext();
                }
                ID++;
            }
            return ES_HEAD;
        }
    }
    public static class ExpressionSymbols
    {
        public static readonly string [ ] Termination = new string [ ] { "," , ")" , ";" };
        public static readonly string [ ] RightHand_Unary_0st = new string [ ] { "!" , "++" , "--" };
        public static readonly string [ ] LeftHand_Unary_0st = new string [ ] { "++" , "--" };
        public static readonly string [ ] Binary_0st = new string [ ] { "&&" , "||" , "&" , "|" };
        public static readonly string [ ] Binary_1st = new string [ ] { "*" , "/" , "%" };
        public static readonly string [ ] Binary_2st = new string [ ] { "+" , "-" };

    }
}