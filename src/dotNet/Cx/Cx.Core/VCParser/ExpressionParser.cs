using Cx.Core.DataTools;
using LibCLCC.NET.TextProcessing;
using System;
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
#if DEBUG
        static void PrintESContext(ExpressionSegmentContext ES_Context)
        {
            while (!ES_Context.IsReachEnd)
            {
                var current = ES_Context.Current;
                if (current != null)
                {
                    switch (current.SegmentType)
                    {
                        case ExpressionSegmentType.PlainContent:
                            Console.Write($"{current?.Segment?.content ?? "null"}");
                            break;
                        case ExpressionSegmentType.TreeNode:
                            Console.Write($"TreeNode");
                            break;
                        case ExpressionSegmentType.ESTreeNode:
                            Console.Write($"ESTreeNode");
                            break;
                        case ExpressionSegmentType.Closure:
                            Console.Write($"CLOSURE");
                            break;
                        case ExpressionSegmentType.NULL:
                            Console.Write($"NULL");
                            break;
                        default:
                            break;
                    }

                }
                else
                    Console.Write($"NULL");
                Console.Write("\t");
                ES_Context.GoNext();
            }
            Console.WriteLine();
            ES_Context.SetCurrent(ES_Context.HEAD);
        }
#endif
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            if (context.ReachEnd)
            {
                return FinalResult;
            }
            if (context.Current == null)
            {
                return FinalResult;
            }
            {
                Segment HEAD = context.Current;
                var TER = TerminateExpression(provider , context);
                if (FinalResult.CheckAndInheritAbnormalities(TER)) return FinalResult;
                var _newContext = TER.Result;

//#if DEBUG
//                Console.WriteLine("Terminated.");
//                while (!_newContext.ReachEnd)
//                {
//                    Console.Write($"{_newContext.Current?.content ?? "null"}");
//                    Console.Write("\t");
//                    _newContext.GoNext();
//                }
//                Console.WriteLine();
//                _newContext.ResetCurrent();
//#endif
                var FPR = FirstPass_SubstituteCallsAndVariables(provider , _newContext);
                if (FinalResult.CheckAndInheritAbnormalities(FPR)) return FinalResult;
                if (FPR.Result == null)
                {
                    FinalResult.AddError(new ParseFailError(_newContext.HEAD , ASTNodeType.Expression));
                    return FinalResult;
                }
                ExpressionSegmentContext ES_Context = new ExpressionSegmentContext(FPR.Result);
#if DEBUG
                Console.WriteLine("Call Processed.");
                PrintESContext(ES_Context);
#endif
                var node_result = ParseExpressionTree(provider , ES_Context);
                if (FinalResult.CheckAndInheritAbnormalities(node_result)) return FinalResult;
                if (node_result.Result == null)
                {
#if DEBUG
                    Console.WriteLine("!!!Null Node.");
#endif
                    FinalResult.AddError(new ParseFailError(_newContext.HEAD , ASTNodeType.Expression));
                    return FinalResult;
                }
                Parent.AddChild(node_result.Result);
                FinalResult.Result = true;
            }
            return FinalResult;
        }
        public OperationResult<TreeNode?> ParseExpressionTree(ParserProvider provider , ExpressionSegmentContext context)
        {
#if DEBUG
            Console.WriteLine("Start process expression tree:");
            if (context.HEAD.Prev != null)
                Console.WriteLine("HEAD has prev!");

            PrintESContext(context);
#endif
            context.SetCurrent(context.HEAD);
            OperationResult<TreeNode?> FinalResult = new OperationResult<TreeNode?>(null);

            var SP_C_R = SecondPass_Closures(provider , context);
            if (FinalResult.CheckAndInheritAbnormalities(SP_C_R)) return FinalResult;
#if DEBUG
            Console.WriteLine("Closure Done.");

            context.SetCurrent(context.HEAD);
            PrintESContext(context);
#endif
            {
                context.SetCurrent(context.HEAD);
                var CPBER = CustomPass_RightHandSideUnaryExpression(provider , context , ExpressionSymbols.RightHand_Unary_0st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
            {
                context.SetCurrent(context.HEAD);
                var CPBER = CustomPass_LeftHandSideUnaryExpression(provider , context , ExpressionSymbols.LeftHand_Unary_0st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
#if DEBUG
            Console.WriteLine("Unary Done!");
            PrintESContext(context);
            context.SetCurrent(context.HEAD);
#endif
            {
                context.SetCurrent(context.HEAD);
                var CPBER = CustomPass_BinaryExpression(provider , context , ExpressionSymbols.Binary_0st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
            {
                context.SetCurrent(context.HEAD);
                var CPBER = CustomPass_BinaryExpression(provider , context , ExpressionSymbols.Binary_1st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
            {
                context.SetCurrent(context.HEAD);
                var CPBER = CustomPass_BinaryExpression(provider , context , ExpressionSymbols.Binary_2st);
                if (FinalResult.CheckAndInheritAbnormalities(CPBER)) return FinalResult;
                if (!CPBER.Result)
                {
                    FinalResult.AddError(new ParseFailError(context.HEAD.Segment , ASTNodeType.BinaryExpression));
                    return FinalResult;
                }
            }
#if DEBUG
            Console.WriteLine($"Final Stage: Reset Context");
#endif
            context.SetCurrent(context.HEAD);
#if DEBUG
            Console.WriteLine($"Final Stage: Context Reset!");
            Console.WriteLine($"Final Stage: Context Count:{context.Count}. HEAD?{context.HEAD}");
            PrintESContext(context);
            context.SetCurrent(context.HEAD);
#endif
            {
                if (context.Count > 1)
                {
                    FinalResult.AddError(new ExpressionRemainsMoreThanOneSegmentError(context.HEAD.Segment));
                    return FinalResult;
                }
            }
            var node = FinalizeContext(context);

            FinalResult.Result = node;
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
                        ENDPOINT = context.Current;
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
                    {
                        PairCount--;
                        if (PairCount < 0)
                        {
                            ENDPOINT = context.Current;
                            break;
                        }
                    }
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
                    ExpressionSegment? prev = context.Current!.Prev;
                    ExpressionSegment? next = context.Current!.Next;
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
#if DEBUG
                    Console.WriteLine($"Binary Expression:{prev.Segment?.content}{context.Current?.Segment?.content ?? "unknown symbol"}{next.Segment?.content}");
#endif
                    if (!prev.IsOkayForExpressionPart || !next.IsOkayForExpressionPart)
                    {
#if DEBUG
                        Console.WriteLine($"Binary Expression: REJECTED, L:{!prev.IsOkayForExpressionPart}{prev.GetString()}," +
                            $" R:{next.IsOkayForExpressionPart}{next.GetString()}");
#endif
                        context.GoNext();
                        continue;
                    }
                    ExpressionSegment NewSegment = new ExpressionSegment();
                    NewSegment.Node = expressionTreeNode;
                    expressionTreeNode.ExpressionChildren.Add(prev);
                    expressionTreeNode.ExpressionChildren.Add(next);
                    context.SubstituteSegment_0(prev?.Prev , next?.Next , NewSegment);
                }
                context.GoNext();
            }
            FinalResult.Result = true;
            return FinalResult;
        }
        public TreeNode? FinalizeContext(ExpressionSegmentContext context)
        {
            var HEAD = context.HEAD;
#if DEBUG
            Console.WriteLine($"???HEAD NULL:{HEAD == null}");
#endif
            if (HEAD == null) return null;
#if DEBUG
            Console.WriteLine($"???HEAD.Type:{HEAD.SegmentType}");
            if (HEAD.SegmentType == ExpressionSegmentType.PlainContent)
            {
                Console.WriteLine($"???HEAD.Seg.Content:{HEAD.Segment?.content ?? "null"}");

            }
#endif
            if (context.HEAD.Node == null) return null;
#if DEBUG
            Console.WriteLine($"???:{context.HEAD.Node.GetType()}");
#endif
            var node = context.HEAD.Node as ExpressionTreeNode;
            if (node == null) return null;
            Finalize(node);
            return node;
        }
        public void Finalize(ExpressionTreeNode Node)
        {
            if (Node.IsFormed) return;
            foreach (var item in Node.ExpressionChildren)
            {
                if (item.SegmentType == ExpressionSegmentType.PlainContent)
                {
                    TreeNode treeNode = new TreeNode();
                    treeNode.Type = ASTNodeType.EndNode;
                    treeNode.Segment = item.Segment;
                    Node.AddChild(treeNode);
                }
                else if (item.SegmentType == ExpressionSegmentType.ESTreeNode)
                {
                    if (item.Node is ExpressionTreeNode esn)
                    {
                        Finalize(esn);
                        Node.AddChild(esn);
                    }
                }
                else if (item.SegmentType == ExpressionSegmentType.TreeNode)
                {
                    if (item.Node != null)
                    {
                        Node.AddChild(item.Node);
                    }
                }
            }
            Node.IsFormed = true;
        }
        public OperationResult<bool> CustomPass_RightHandSideUnaryExpression(ParserProvider provider , ExpressionSegmentContext context , params string [ ] symbols)
        {
            OperationResult<bool> FinalResult = new OperationResult<bool>(false);
#if DEBUG
            Console.WriteLine($">>>>>>Start RHS Unary:{context.Current?.Segment?.content ?? "NULL"}(Is End:{context.IsReachEnd})");
            PrintESContext(context);
#endif
            while (true)
            {
                if (context.IsReachEnd) break;
                var ESMR = context.MatchCollection(symbols);
                if (ESMR == ESCMatchResult.Match)
                {
#if DEBUG
                    Console.WriteLine($">>>>>>RHS UnaryExp:{context.Current?.Segment?.content ?? "NULL"}");
#endif

                    ExpressionTreeNode expressionTreeNode = new ExpressionTreeNode();
                    expressionTreeNode.Segment = context.Current?.Segment;
                    expressionTreeNode.Type = ASTNodeType.UnaryExpression;
                    ExpressionSegment? prev = context.Current?.Prev;
                    ExpressionSegment? next = context.Current?.Next;
                    bool isHit = false;
                    if (prev == null)
                    {
                        isHit = true;
                    }
                    else
                    {
                        if (prev.IsOkayForExpressionPart)
                        {
                            isHit = false;
                        }
                        else
                        {
                            isHit = true;
#if DEBUG
                            Console.WriteLine($">>>>>>RHS UnaryExp:prev not suitable for exp part");
#endif
                        }
                    }

                    if (next == null)
                    {
                        FinalResult.AddError(new UnaryExpressionMissingPartError(context.Current?.Segment));
                        return FinalResult;
                    }
                    if (next.IsOkayForExpressionPart)
                    {
                        isHit &= true;
#if DEBUG
                        Console.WriteLine($">>>>>>RHS UnaryExp:next suitable for exp part, isHit={isHit}");
#endif
                    }
                    else
                    {
                        isHit = false;
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
#if DEBUG
            Console.WriteLine($">>>>>>Start LHS Unary:{context.Current?.Segment?.content ?? "NULL"}(Is End:{context.IsReachEnd})");
            PrintESContext(context);
#endif
            OperationResult<bool> FinalResult = new OperationResult<bool>(false);
            while (true)
            {
                if (context.IsReachEnd) break;
                var ESMR = context.MatchCollection(symbols);
                if (ESMR == ESCMatchResult.Match)
                {

                    ExpressionTreeNode expressionTreeNode = new ExpressionTreeNode();
                    expressionTreeNode.Segment = context.Current?.Segment;
#if DEBUG
                    Console.WriteLine($">>>>>>UnaryExp:{context.Current?.Segment?.content??"NULL"}");
#endif
                    expressionTreeNode.Type = ASTNodeType.UnaryExpression;
                    ExpressionSegment? prev = context.Current?.Prev;
                    ExpressionSegment? next = context.Current?.Next;
                    bool isHit = false;
                    if (prev == null)
                    {
                        FinalResult.AddError(new UnaryExpressionMissingPartError(context.Current?.Segment));
                        return FinalResult;
                    }
                    {
                        if (prev.IsOkayForExpressionPart)
                        {
                            isHit = true;
                        }
                    }
                    if (next != null)
                        if (next.IsOkayForExpressionPart)
                        {
                            isHit = false;
                        }
                    if (isHit)
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
#if DEBUG

        static void PrintDepth(int depth , string content)
        {
            for (int i = 0 ; i < depth ; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine(content);
        }
        static void PrintESTree(TreeNode node , int Depth)
        {
            switch (node.Type)
            {
                case ASTNodeType.Expression:
                    PrintDepth(Depth , "Node: Expression");
                    break;
                case ASTNodeType.BinaryExpression:
                    PrintDepth(Depth , "Node: BinExp");
                    break;
                case ASTNodeType.UnaryExpression:
                    PrintDepth(Depth , "Node: UnExp");
                    break;
                case ASTNodeType.Call:
                    PrintDepth(Depth , "Node: Function Call");
                    break;
                default:
                    PrintDepth(Depth , "Generic Node");
                    break;
            }
            if (node.Segment != null)
            {
                PrintDepth(Depth , $">Content:{node.Segment.content}");
            }
            else
                PrintDepth(Depth , $">Content: NULL");
            foreach (var item in node.Children)
            {
                PrintESTree(item , Depth + 1);
            }
        }
#endif
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
                else
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
                        var R = context.Current!;
                        var L = FirstParentheses_L.Next;
                        ExpressionSegmentContext? Closed = context.SubContext(L , R);
                        if (Closed == null)
                        {
                            FinalResult.AddError(new ClosureError(context.HEAD.Segment , "(" , ")"));
                            return FinalResult;
                        }
                        context.GoNext();
                        //Recursive Parse.
                        var CResult = ParseExpressionTree(provider , Closed);
#if DEBUG
                        Console.WriteLine($"Sub Expression Tree done:");
                        PrintESTree(CResult.Result ?? new TreeNode() , 0);
#endif
                        if (FinalResult.CheckAndInheritAbnormalities(CResult)) return FinalResult;
                        ExpressionSegment Closure = new ExpressionSegment();
                        //Closure.Context = Closed;
                        var nt = CResult.Result;
                        Closure.Node = nt;
#if DEBUG

                        Console.WriteLine($"L:{FirstParentheses_L?.Prev?.GetString()??"<NULL>"},R:{R?.Next?.GetString()}");
#endif
                        context.SubstituteSegment_0(FirstParentheses_L?.Prev , R?.Next , Closure);
                        FirstParentheses_L = null;
                    }
                }
                context.GoNext();
            }
            FinalResult.Result = true;
            return FinalResult;
        }
        public OperationResult<ExpressionSegment?> FirstPass_SubstituteCallsAndVariables(ParserProvider provider , SegmentContext context)
        {
            OperationResult<ExpressionSegment?> FinalResult = new OperationResult<ExpressionSegment?>(null);
            var HEAD = context.Current;

            var CallParser = provider.GetParser(ASTNodeType.Call);
            var VariableParser = provider.GetParser(ASTNodeType.Variable);
			if (CallParser == null)
			{
				FinalResult.AddError(new ParserNotFoundError(HEAD , ASTNodeType.Call));
				return FinalResult;
			}

			if (VariableParser == null)
			{
				FinalResult.AddError(new ParserNotFoundError(HEAD , ASTNodeType.Variable));
				return FinalResult;
			}
			TreeNode Reciver = new TreeNode();
            ExpressionSegment? ES_HEAD = null;
            ExpressionSegment? Current = null;
            int ID = 0;
            while (true)
            {
                if (context.ReachEnd) break;
                var CUR = context.Current;
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
                    context.SetCurrent(CUR);
                    var VPR = VariableParser.Parse(provider , context , Reciver);

					if (FinalResult.CheckAndInheritAbnormalities(VPR)) { return FinalResult; }
                    if (VPR.Result)
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
                }
                ID++;
            }
            {

                ExpressionSegment new_segment = new ExpressionSegment();

                Current?.AttachNext(new_segment);
            }
            return ES_HEAD;
        }
    }
    public class ExpressionRemainsMoreThanOneSegmentError : OperationError
    {
        public ExpressionRemainsMoreThanOneSegmentError(Segment? binded) : base(binded , null)
        {
        }
        public override string? Message => "Expression remains more than one segment after parsing.";
    }
}