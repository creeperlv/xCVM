using Cx.Core;
using Cx.Core.VCParser;

namespace Cx.HL2VC
{
    public static class HLCParsers
    {
        public static ParserProvider GetProvider()
        {
            ParserProvider ParserProvider = new ParserProvider();
            ParserProvider.RegisterProvider(ASTNodeType.Root , new RootParser());
            ParserProvider.RegisterProvider(ASTNodeType.DeclareFunc , new FunctionParser());
            ParserProvider.RegisterProvider(ASTNodeType.DataType , new TypeParser());
            ParserProvider.RegisterProvider(HLASTNodeType.Namespace, new NamespaceParser());
            return ParserProvider;
        }
    }
    public class HLRootParser : RootParser
    {
        public HLRootParser() {
            ConcernedParsers.Add(HLASTNodeType.Namespace);
        }
    }
}
