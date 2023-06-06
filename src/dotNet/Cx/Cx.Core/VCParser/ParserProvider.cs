using System.Collections.Generic;

namespace Cx.Core.VCParser
{
    public class ParserProvider
    {
        Dictionary<int, ContextualParser> parsers = new Dictionary<int, ContextualParser>();
        public void RegisterParser(int ID, ContextualParser parser)
        {
            if (parsers.ContainsKey(ID))
            {
                parsers[ID] = parser;
            }
            else parsers.Add(ID, parser);
        }
        public ContextualParser? GetParser(int ID)
        {
            if (parsers.ContainsKey(ID))
            {
                return parsers[ID];
            }
            return null;
        }
    }
    public static class VanillaCParsers
    {
        public static ParserProvider GetProvider()
        {
            ParserProvider ParserProvider = new ParserProvider();
            ParserProvider.RegisterParser(ASTNodeType.Root, new RootParser());
            ParserProvider.RegisterParser(ASTNodeType.DeclareFunc, new FunctionParser());
            ParserProvider.RegisterParser(ASTNodeType.DataType, new TypeParser());
            ParserProvider.RegisterParser(ASTNodeType.Scope, new ScopeParser());
            return ParserProvider;
        }
    }
}