using LibCLCC.NET.TextProcessing;
using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class ExpressionSegment
    {
        public ExpressionSegment? Prev;
        public ExpressionSegment? Next;
        public Segment? Segment;
        public TreeNode? Node;
        public ExpressionSegmentContext? Context;
        public void AttachNext(ExpressionSegment next)
        {
            Next = next;
            next.Prev = this;
        }
    }
    public class ExpressionSegmentContext
    {
        public ExpressionSegment HEAD;
        public int Count
        {
            get
            {
                ExpressionSegment Current = HEAD;
                int Count = 1;
                while (true)
                {
                    if (Current.Next == null)
                    {
                        return Count;
                    }
                }
            }
        }
        public ExpressionSegment? this [ int index ]
        {
            get
            {
                ExpressionSegment? current = HEAD;
                for (int i = 0 ; i <= index ; i++)
                {
                    if (current == null) return null;
                    current = current.Next;
                }
                return current;
            }
        }
        public ExpressionSegmentContext(ExpressionSegment head)
        {
            HEAD = head;
        }
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
            while (true)
            {
                if (context.ReachEnd) break;
                var CPR = CallParser.Parse(provider , context , Reciver);
                if (FinalResult.CheckAndInheritAbnormalities(CPR)) { return FinalResult; }
                if (CPR.Result)
                {
                    var NewNode = Reciver.Children [ 0 ];
                    Reciver.Children.Clear();
                    ExpressionSegment new_segment = new ExpressionSegment();
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
                    ExpressionSegment new_segment = new ExpressionSegment();
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
            return ES_HEAD;
        }
    }
    public static class ExpressionSymbols
    {
        public static readonly string [ ] Termination = new string [ ] { "," , ")" , ";" };
        public static readonly string [ ] Binary_0st = new string [ ] { "&&" , "||" , "&" , "|" };
        public static readonly string [ ] Binary_1st = new string [ ] { "*" , "/" , "%" };
        public static readonly string [ ] Binary_2st = new string [ ] { "+" , "-" };

    }
}