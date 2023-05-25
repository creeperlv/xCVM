using System.Collections.Generic;

namespace Cx.Core
{
    public class ParserProvider
    {
        Dictionary<int , ContextualParser> parsers = new Dictionary<int , ContextualParser>();
        public void RegisterProvider(int ID , ContextualParser parser)
        {
            if (parsers.ContainsKey(ID))
            {
                parsers [ ID ] = parser;
            }
            else parsers.Add(ID , parser);
        }
        public ContextualParser? GetParser(int ID)
        {
            if (parsers.ContainsKey(ID))
            {
                return parsers [ ID ];
            }
            return null;
        }
    }
    public static class VanillaCParsers
    {
        public static ParserProvider GetProvider()
        {
            ParserProvider ParserProvider = new ParserProvider();
            ParserProvider.RegisterProvider(ASTNodeType.Root , new RootParser());
            ParserProvider.RegisterProvider(ASTNodeType.DeclareFunc , new FunctionParser());
            ParserProvider.RegisterProvider(ASTNodeType.DataType , new TypeParser());
            return ParserProvider;
        }
    }
}