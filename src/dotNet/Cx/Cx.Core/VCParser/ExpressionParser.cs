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
    }
    public class ExpressionSegmentContext
    {
        public ExpressionSegment HEAD;
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
            return FinalResult;
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