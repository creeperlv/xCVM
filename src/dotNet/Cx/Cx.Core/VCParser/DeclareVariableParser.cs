using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class DeclareVariableParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> result = false;
            var TypeParser = provider.GetParser(ASTNodeType.DataType);
            if(TypeParser == null )
            {
                result.AddError(new ParserNotFoundError(context.Current));
                return result;
            }
            TreeNode node=new TreeNode();
            node.Type = ASTNodeType.DeclareVar;
            var tr=TypeParser.Parse(provider , context , node);
            if (!tr.Result)
            {
                result.InheritAbnormalities(tr);
                return false;
            }
            node.Segment=context.Current;
            return result;
        }
    }
}