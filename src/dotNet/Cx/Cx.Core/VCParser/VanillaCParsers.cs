namespace Cx.Core.VCParser
{
    public static class VanillaCParsers
    {
        public static ParserProvider GetProvider()
        {
            ParserProvider ParserProvider = new ParserProvider();
            ParserProvider.RegisterParser(ASTNodeType.Root, new RootParser());
            ParserProvider.RegisterParser(ASTNodeType.DeclareFunc, new FunctionParser());
            ParserProvider.RegisterParser(ASTNodeType.DeclareVar, new DeclareVariableParser());
            ParserProvider.RegisterParser(ASTNodeType.DataType, new TypeParser());
            ParserProvider.RegisterParser(ASTNodeType.Scope, new ScopeParser());
            ParserProvider.RegisterParser(ASTNodeType.Call, new CallParser());
            ParserProvider.RegisterParser(ASTNodeType.Statement, new StatementParser());
            ParserProvider.RegisterParser(ASTNodeType.TypeDef, new TypeDefParser());
            ParserProvider.RegisterParser(ASTNodeType.Extern, new ExternParser());
            ParserProvider.RegisterParser(ASTNodeType.If, new IfParser());
            ParserProvider.RegisterParser(ASTNodeType.While, new WhileParser());
            ParserProvider.RegisterParser(IntermediateASTNodeType.Intermediate_NonSemicolonStatement, new NonSemiColonStatementParser());
            ParserProvider.RegisterParser(IntermediateASTNodeType.Intermediate_AllStatement, new AllStatementParser());
            ParserProvider.RegisterParser(IntermediateASTNodeType.Intermediate_AllStatementAndAScope , new AllStatementAndScopeParser());
            return ParserProvider;
        }
    }
}