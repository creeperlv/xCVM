using Cx.Core;
using LibCLCC.NET.TextProcessing;
using System;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC
{
    public class HLCxParser : ContextualParser
    {
        public HLCxParser()
        {
        }
        public OperationResult<ASTNode> ParseAST(Segment HEAD)
        {
            ParserProvider provider = VanillaCParsers.GetProvider();
            SegmentContext segmentContext = new SegmentContext(HEAD);
            ASTNode root = new ASTNode();
            OperationResult<ASTNode> result = new OperationResult<ASTNode>(root);
            Parse(provider,segmentContext , root);
            return result;
        }

        public override OperationResult<bool> Parse(ParserProvider provider,SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            return result;
        }
    }
    public static class HLASTNodeType
    {
        public const int Namespace = 100;
        public const int Generic= 101;
    }
}
