using Cx.Core;
using Cx.Core.VCParser;
using System.Collections.Generic;

namespace Cx.HL2VC.Parsers
{
    public static class HLCParsers
    {
        public static ParserProvider GetProvider()
        {
            ParserProvider ParserProvider = new ParserProvider();
            ParserProvider.RegisterParser(ASTNodeType.Root , new HLRootParser());
            ParserProvider.RegisterParser(ASTNodeType.DeclareFunc , new FunctionParser());
            ParserProvider.RegisterParser(ASTNodeType.DataType , new HLTypeParser());
            ParserProvider.RegisterParser(HLASTNodeType.Namespace , new NamespaceParser());
            return ParserProvider;
        }
    }
    public class HLRootParser : RootParser
    {
        public HLRootParser()
        {
            var old_list = ConcernedParsers;
            ConcernedParsers = new List<int>
            {
                HLASTNodeType.Namespace ,
                HLASTNodeType.Using
            };
            foreach (var item in old_list)
            {
                ConcernedParsers.Add(item);
            }
        }
    }
}
