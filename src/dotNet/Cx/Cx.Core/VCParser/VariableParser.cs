using Cx.Core.DataTools;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class VariableParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider, SegmentContext context, TreeNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            TreeNode treeNode = new TreeNode();
            if (context.ReachEnd) return FinalResult;
            if (DataTypeChecker.DetermineDataType(context.Current?.content ?? "") == DataType.String)
            {
                treeNode.Type = ASTNodeType.Variable;
                treeNode.Segment = context.Current;
            }
            return FinalResult;
        }
    }
}