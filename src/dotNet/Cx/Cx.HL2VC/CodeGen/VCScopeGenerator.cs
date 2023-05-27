using Cx.Core.CodeGen;
using Cx.Core.VCParser;
using System.IO;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.CodeGen
{
    public class VCScopeGenerator : CodeGenerator
    {
        public override OperationResult<bool> Write(GeneratorProvider provider , ASTNode node , StreamWriter writer)
        {
            writer.Write("{");
            foreach (ASTNode child in node.Children)
            {
                var g = provider.GetGenerator(child.Type);
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
