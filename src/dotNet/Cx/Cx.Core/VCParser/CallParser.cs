using Cx.Core.SegmentContextUtilities;
using System;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class CallParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            var HEAD = context.Current;
            if (HEAD == null)
            {
                FinalResult.Result = false;
                FinalResult.AddError(new UnexpectedEndOfFileError(HEAD));
                return FinalResult;
            }
            context.GoNext();
            var LR = context.Current;
            if (LR == null)
            {
                FinalResult.Result = false;
                FinalResult.AddError(new UnexpectedEndOfFileError(HEAD));
                return FinalResult;
            }
            if (LR.content == "(")
            {
                var Arguments = ContextClosure.LRClose(context , "(" , ")");
                if (Arguments.Errors.Count > 0)
                {
                    FinalResult.Errors = Arguments.Errors;
                    return FinalResult;
                }
                if (Arguments.Result == null)
                {
                    return FinalResult;
                }
                ASTNode node = new ASTNode();
                node.Type = ASTNodeType.Call;
                node.Segment = HEAD;
                var _context = Arguments.Result;
                while (true)
                {
                    if (_context.ReachEnd)
                    {
                        break;
                    }
                    if (_context.Current == _context.EndPoint)
                    {
                        break;
                    }
                    var Current = _context.Current;
                    if (Current == null)
                    {
                        break;
                    }

                }
                context.SetCurrent(Arguments.Result.EndPoint);
            }
            else
            {

            }
            context.SetCurrent(HEAD);
            return FinalResult;

        }
    }
}