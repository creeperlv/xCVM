using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    /// <summary>
    /// Ends at where the name is.
    /// int a = ...
    ///     ^ This is where the Current is when parsed successfully.
    /// </summary>
    public class DeclareVariableParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> result = false;
            var TypeParser = provider.GetParser(ASTNodeType.DataType);
            if (TypeParser == null)
            {
                result.AddError(new ParserNotFoundError(context.Current , ASTNodeType.DataType));
                return result;
            }
            TreeNode node = new TreeNode();
            node.Type = ASTNodeType.DeclareVar;
            var tr = TypeParser.Parse(provider , context , node);
            if (!tr.Result)
            {
                result.InheritAbnormalities(tr);
                return false;
            }
            result.Result = true;
            node.Segment = context.Current;
            Parent.AddChild(node);
            return result;
        }
    }
}