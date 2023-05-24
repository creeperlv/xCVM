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
            var rp = new RootParser();
            rp.SubParsers.Add(new NamespaceParser());
            SubParsers.Add(rp);
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
            var rp = new RootParser();
            SubParsers.Add(rp);
        }
        public override OperationResult<bool> Parse(SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> result=new OperationResult<bool>(false);
            var nr=context.Match("namespace");
            var BRKPNT=context.Current;
            if(nr== MatchResult.Match)
            {
                while (true)
                {
                    var seg_match = context.MatchCollection(true , "." , "{");
                }
            }
            else
            {
                context.SetCurrent(BRKPNT);
            }
            return result;
        }
    }
}
