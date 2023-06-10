using Cx.Core.DataTools;
using Cx.Core.SegmentContextUtilities;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class EnumParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            var FinalResult = new OperationResult<bool>(false);
            var em = context.Match("enum");
            if (em == MatchResult.Match)
            {
                context.GoNext();
                if (context.Current == null)
                {
                    FinalResult.AddError(new UnexpectedEndError(context.Current));
                    return FinalResult;
                }
                if (context.Current.content == "" && context.Current.Next == null)
                {
                    FinalResult.AddError(new UnexpectedEndError(context.Current));
                    return FinalResult;
                }
                TreeNode EnumNode = new TreeNode();
                var name = context.Current.content;
                if (DataTypeChecker.DetermineDataType(name) != DataType.String)
                {
                    FinalResult.AddError(new IllegalIdentifierError(context.Current));
                }
                EnumNode.Segment = context.Current;
                var ClosureResult=ContextClosure.LRClose(context , "{" , "}");
                if(ClosureResult.Errors.Count > 0)
                {
                    FinalResult.Errors=ClosureResult.Errors;
                    return FinalResult;
                }
                if (ClosureResult.Result == null)
                {
                    return FinalResult;
                }
                context.SetCurrent(ClosureResult.Result.EndPoint);
            }
            return FinalResult;
        }
    }
}