using Cx.Core;
using Cx.Core.VCParser;
using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC
{
    public class NamespaceParser : ContextualParser
    {
        public NamespaceParser()
        {
            ConcernedParsers.Add(ASTNodeType.DeclareFunc);
            ConcernedParsers.Add(ASTNodeType.DeclareStruct);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> result=new OperationResult<bool>(false);
            var nr=context.Match("namespace");
            var BRKPNT=context.Current;
            if(nr== MatchResult.Match)
            {
                var HEAD = context.Current;
                Segment? FirstLB = null;
                while (true)
                {
                    var res = context.MatchMarch("{");
                    if (res == MatchResult.Match)
                    {
                        FirstLB = context.Last;
                    }
                    else if (res == MatchResult.Mismatch)
                    {
                        context.GoNext();
                    }
                    else
                    {
                        break;
                    }
                }
                SegmentContext namespace_name = new SegmentContext(HEAD);
                namespace_name.SetEndPoint(FirstLB);

            }
            else
            {
                context.SetCurrent(BRKPNT);
            }
            return result;
        }
    }
}
