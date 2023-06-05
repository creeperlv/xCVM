using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class Parser : ContextualParser
    {
        public Parser()
        {
            ConcernedParsers.Add(ASTNodeType.Root);
        }
        public OperationResult<ASTNode> ParseAST(Segment HEAD)
        {
            SegmentContext segmentContext = new SegmentContext(HEAD);
            ASTNode root = new ASTNode();
            OperationResult<ASTNode> result = new OperationResult<ASTNode>(root);
            Parse(VanillaCParsers.GetProvider(), segmentContext, root);
            return result;
        }

        public override OperationResult<bool> Parse(ParserProvider provider, SegmentContext context, ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            return result;
        }
    }

}