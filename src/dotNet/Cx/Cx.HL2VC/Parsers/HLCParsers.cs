using Cx.Core;
using Cx.Core.VCParser;

namespace Cx.HL2VC.Parsers
{
    public static class HLCParsers
    {
        public static ParserProvider GetProvider()
        {
            ParserProvider ParserProvider = new ParserProvider();
            ParserProvider.RegisterParser(ASTNodeType.Root, new RootParser());
            ParserProvider.RegisterParser(ASTNodeType.DeclareFunc, new FunctionParser());
            ParserProvider.RegisterParser(ASTNodeType.DataType, new TypeParser());
            ParserProvider.RegisterParser(HLASTNodeType.Namespace, new NamespaceParser());
            return ParserProvider;
        }
    }
    public class HLRootParser : RootParser
    {
        public HLRootParser()
        {
            ConcernedParsers.Add(HLASTNodeType.Namespace);
        }
    }
}
