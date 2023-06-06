namespace Cx.Core.VCParser
{
    public static class VanillaCParsers
    {
        public static ParserProvider GetProvider()
        {
            ParserProvider ParserProvider = new ParserProvider();
            ParserProvider.RegisterParser(ASTNodeType.Root, new RootParser());
            ParserProvider.RegisterParser(ASTNodeType.DeclareFunc, new FunctionParser());
            ParserProvider.RegisterParser(ASTNodeType.DataType, new TypeParser());
            ParserProvider.RegisterParser(ASTNodeType.Scope, new ScopeParser());
            ParserProvider.RegisterParser(ASTNodeType.Call, new CallParser());
            ParserProvider.RegisterParser(ASTNodeType.Statement, new StatementParser());
            return ParserProvider;
        }
    }
}