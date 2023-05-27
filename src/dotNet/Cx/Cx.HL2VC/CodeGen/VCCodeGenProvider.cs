using Cx.Core;
using Cx.Core.CodeGen;

namespace Cx.HL2VC.CodeGen
{
    public static class VCCodeGenProvider
    {
        public static GeneratorProvider GetProvider()
        {
            GeneratorProvider provider = new GeneratorProvider();
            provider.RegisterGenerator(ASTNodeType.Root , new VCRootGenerator());
            provider.RegisterGenerator(ASTNodeType.Pointer, new VCPointerGenerator());
            provider.RegisterGenerator(ASTNodeType.DataType, new VCTypeGenerator());
            provider.RegisterGenerator(ASTNodeType.ReturnType, new VCTypeGenerator());
            return provider;
        }
    }
}
