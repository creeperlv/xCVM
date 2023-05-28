using LibCLCC.NET.TextProcessing;
using System.Collections.Generic;
using xCVM.Core.CompilerServices;

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
    public class ParserNotFoundError : OperationError
    {
        public ParserNotFoundError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Parser Not Found.";
    }
    public static class VanillaCParsers
    {
        public static ParserProvider GetProvider()
        {
            ParserProvider ParserProvider = new ParserProvider();
            ParserProvider.RegisterParser(ASTNodeType.Root, new RootParser());
            ParserProvider.RegisterParser(ASTNodeType.DeclareFunc, new FunctionParser());
            ParserProvider.RegisterParser(ASTNodeType.DataType, new TypeParser());
            return ParserProvider;
        }
    }
}