using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class TypeDefParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> operationResult = false;
            ContextualParser? TypeParser = provider.GetParser(ASTNodeType.DataType);
            if (TypeParser == null)
            {

            }
            return operationResult;
        }
    }
}