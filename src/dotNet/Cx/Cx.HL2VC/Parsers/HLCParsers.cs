﻿using Cx.Core;
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
            ParserProvider.RegisterParser(HLASTNodeType.Namespace , new NamespaceParser());
            ParserProvider.RegisterParser(HLASTNodeType.Using , new UsingParser());
            ParserProvider.RegisterParser(ASTNodeType.Scope , new ScopeParser());
            return ParserProvider;
        }
    }
}
