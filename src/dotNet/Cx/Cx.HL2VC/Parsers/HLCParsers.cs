using Cx.Core;
using Cx.Core.VCParser;

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
            ParserProvider.RegisterParser(ASTNodeType.DeclareStruct , new HLDeclareStructParser());
            ParserProvider.RegisterParser(ASTNodeType.DeclareVar , new DeclareVariableParser());
            ParserProvider.RegisterParser(ASTNodeType.Extern , new ExternParser());
            ParserProvider.RegisterParser(HLASTNodeType.Namespace , new NamespaceParser());
            ParserProvider.RegisterParser(HLASTNodeType.Using , new UsingParser());
            ParserProvider.RegisterParser(ASTNodeType.Scope , new ScopeParser());
            ParserProvider.RegisterParser(ASTNodeType.If , new IfParser());
            ParserProvider.RegisterParser(ASTNodeType.While , new WhileParser());
            ParserProvider.RegisterParser(ASTNodeType.Expression , new ExpressionParser());
            ParserProvider.RegisterParser(ASTNodeType.Call , new HLCallParser());
            ParserProvider.RegisterParser(ASTNodeType.AssignedDeclareVariable , new AssignedDeclareVariableParser());
            ParserProvider.RegisterParser(ASTNodeType.Assign , new AssignParser());
            ParserProvider.RegisterParser(ASTNodeType.CombinedAssign , new CombinedAssignParser());
            ParserProvider.RegisterParser(IntermediateASTNodeType.Intermediate_NonSemicolonStatement , new NonSemiColonStatementParser());
            ParserProvider.RegisterParser(IntermediateASTNodeType.Intermediate_AllStatement , new AllStatementParser());
            ParserProvider.RegisterParser(IntermediateASTNodeType.Intermediate_AllStatementAndAScope , new AllStatementAndScopeParser());
            return ParserProvider;
        }
    }
}
