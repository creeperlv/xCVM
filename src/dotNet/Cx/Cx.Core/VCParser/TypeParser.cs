using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class TypeParser : ContextualParser
    {
        OperationResult<ASTNode> WrapPointer(SegmentContext context, ASTNode PointTo)
        {
            var mr = context.MatchMarch("*");

            if (mr == MatchResult.Match)
            {
                ASTNode node = new ASTNode();
                node.Type = ASTNodeType.Pointer;
                node.AddChild(PointTo);
                var r = WrapPointer(context, node);
                return new OperationResult<ASTNode>(r.Result);
            }
            else
            {
                return new OperationResult<ASTNode>(PointTo);
            }
        }
        public override OperationResult<bool> Parse(ParserProvider provider, SegmentContext context, ASTNode Parent)
        {
            var r = context.MatchMarch("struct");
            if (r == MatchResult.Match)
            {
                ASTNode __node = new ASTNode();
                __node.Type = ASTNodeType.UseStruct;
                __node.Segment = context.Current;
                var res = WrapPointer(context, __node);
                Parent.AddChild(res.Result);
                return new OperationResult<bool>(true);
            }
            else if (r == MatchResult.Mismatch)
            {
                ASTNode __node = new ASTNode();
                __node.Type = ASTNodeType.DataType;
                __node.Segment = context.Current;
                var res = WrapPointer(context, __node);
                Parent.AddChild(res.Result);
                return new OperationResult<bool>(true);
            }
            else
            {
                OperationResult<bool> result = new OperationResult<bool>(false);
                result.AddError(new UnexpectedEndOfFileError(context.Current));
                return result;
            }
        }
    }
}