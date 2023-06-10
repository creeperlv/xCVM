using Cx.Core;
using Cx.Core.CodeGen;
using System.IO;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.CodeGen
{
    public class VCScopeGenerator : CodeGenerator
    {
        public override OperationResult<bool> Write(GeneratorProvider provider , TreeNode node , StreamWriter writer)
        {
            writer.Write("{");
            foreach (TreeNode child in node.Children)
            {
                var g = provider.GetGenerator(child.Type);

                if (g == null)
                {
                    OperationResult<bool> operationResult = new OperationResult<bool>(false);
                    operationResult.AddError(new GeneratorNotFoundError(child.Segment));
                    return operationResult;
                }
                var r = g.Write(provider , child , writer);
                if ((r.Errors.Count > 0))
                {
                    return r;
                }
                writer.WriteLine(";");
            }
            writer.Write("}");
            return true;
        }
    }
}
