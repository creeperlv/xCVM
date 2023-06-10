using Cx.Core;
using Cx.Core.CodeGen;
using System.IO;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.CodeGen
{
    public class VCPointerGenerator : CodeGenerator
    {
        public override OperationResult<bool> Write(GeneratorProvider provider , TreeNode node , StreamWriter writer)
        {
            if (node.Children.Count > 0)
            {
                if (node.Children [ 0 ] is TreeNode n)
                {
                    var g = provider.GetGenerator(n.Type);
                    if (g == null)
                    {
                        OperationResult<bool> operationResult = new OperationResult<bool>(false);
                        operationResult.AddError(new GeneratorNotFoundError(n.Segment));
                        return operationResult;
                    }
                    var result = g.Write(provider , n , writer);
                    if (result.Errors.Count > 0) return result;
                    writer.Write("* ");
                }
            }
            return true;
        }
    }
}
