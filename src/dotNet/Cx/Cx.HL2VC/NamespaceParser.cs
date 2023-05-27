using Cx.Core;
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
