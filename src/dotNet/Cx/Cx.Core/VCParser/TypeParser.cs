using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class TypeParser : ContextualParser
    {
        OperationResult<TreeNode> WrapPointer(SegmentContext context, TreeNode PointTo)
        {
            var mr = context.MatchMarch("*");

            if (mr == MatchResult.Match)
            {
                TreeNode node = new TreeNode();
                node.Type = ASTNodeType.Pointer;
                node.AddChild(PointTo);
                var r = WrapPointer(context, node);
                return new OperationResult<TreeNode>(r.Result);
            }
            else
            {
                return new OperationResult<TreeNode>(PointTo);
            }
        }
        public override OperationResult<bool> Parse(ParserProvider provider, SegmentContext context, TreeNode Parent)
        {
            var r = context.MatchMarch("struct");
            if (r == MatchResult.Match)
            {
                TreeNode __node = new TreeNode();
                __node.Type = ASTNodeType.UseStruct;
                __node.Segment = context.Current;
                var res = WrapPointer(context, __node);
                Parent.AddChild(res.Result);
                return new OperationResult<bool>(true);
            }
            else if (r == MatchResult.Mismatch)
            {
                TreeNode __node = new TreeNode();
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