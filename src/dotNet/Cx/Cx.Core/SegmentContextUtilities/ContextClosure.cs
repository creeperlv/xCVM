using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.Core.SegmentContextUtilities
{
    public class ContextClosure
    {
        public static OperationResult<SegmentContext?> RClose(SegmentContext context , string R)
        {
            var __head = context.Current;
            while (true)
            {
                if (context.ReachEnd)
                {

                    if (context.ReachEnd)
                    {
                        var result = new OperationResult<SegmentContext?>(null);
                        result.AddError(new UnexpectedEndError(context.Current));
                        return result;
                    }
                    if (context.Current == null)
                    {
                        var result = new OperationResult<SegmentContext?>(null);
                        result.AddError(new UnexpectedEndError(context.Current));
                        return result;
                    }
                    if (context.Current.content == R)
                    {
                        SegmentContext segmentContext = new SegmentContext(__head);
                        segmentContext.SetEndPoint(context.Current);
                        return segmentContext;
                    }
                    context.GoNext();
                }

            }
        }
        public static OperationResult<SegmentContext?> LRClose(SegmentContext context , string L , string R)
        {
            int ClosureLevel = 0;
            Segment? HEAD = null;
            while (true)
            {
                if (context.ReachEnd)
                {
                    var result = new OperationResult<SegmentContext?>(null);
                    result.AddError(new UnexpectedEndError(context.Current));
                    return result;
                }
                if (context.Current == null)
                {
                    var result = new OperationResult<SegmentContext?>(null);
                    result.AddError(new UnexpectedEndError(context.Current));
                    return result;
                }
                if (context.Current.content == L)
                {
                    if (ClosureLevel == 0)
                    {
                        HEAD = context.Current;
                    }
                    ClosureLevel++;
                }
                if (context.Current.content == R)
                {
                    ClosureLevel--;
                    if (ClosureLevel == 0)
                    {
                        SegmentContext segmentContext = new SegmentContext(HEAD);
                        segmentContext.SetEndPoint(context.Current);
                        return segmentContext;
                    }
                    else if (ClosureLevel < 0)
                    {
                        var result = new OperationResult<SegmentContext?>(null);
                        result.AddError(new UnexpectedEndMarkError(context.Current));
                        return result;
                    }

                }
                context.GoNext();
            }
        }
    }
}
