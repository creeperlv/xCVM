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
        public SegmentContext TerminateExpression(SegmentContext context) {
            var HEAD = context.Current;
            var ENDPOINT = context.EndPoint;
            while (true)
            {

            }
            SegmentContext Result=new SegmentContext(HEAD);
            Result.SetEndPoint(ENDPOINT);
            
        }
        public OperationResult<ExpressionSegment?> FirstPass_SubstituteCalls(SegmentContext context)
        {
            OperationResult<ExpressionSegment?> FinalResult = new OperationResult<ExpressionSegment?>(null);

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