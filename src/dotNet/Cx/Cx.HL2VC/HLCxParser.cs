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
            SubParsers.Add(new RootParser());
        }
        public OperationResult<ASTNode> ParseAST(Segment HEAD)
        {
            SegmentContext segmentContext = new SegmentContext(HEAD);
            ASTNode root = new ASTNode();
            OperationResult<ASTNode> result = new OperationResult<ASTNode>(root);
            Parse(segmentContext , root);
            return result;
        }

        public override OperationResult<bool> Parse(SegmentContext context , ASTNode Parent)
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
    public class NamespaceParser : ContextualParser
    {
        public NamespaceParser()
        {
            SubParsers.Add(new FunctionParser());
        }
        public override OperationResult<bool> Parse(SegmentContext context , ASTNode Parent)
        {
            throw new NotImplementedException();
        }
    }
}
