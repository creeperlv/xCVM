using Cx.Core;
using Cx.Core.SegmentContextUtilities;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Parsers
{
    public class HLStructParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            var headmatch = context.Match("struct");
            var HEAD = context.Current;
            if (HEAD == null)
            {
                return false;
            }
            if (headmatch == MatchResult.Match)
            {
                context.GoNext();
                var FormedName = Utilities.FormName(context , false);
                if (FormedName.Errors.Count > 0)
                {
                    FinalResult.Errors = FormedName.Errors;
                    return FinalResult;
                }
                else
                {
                    var StructureName = FormedName.Result;
                    if(context.Match("{")== MatchResult.Match)
                    {
                        var ClosedResult=ContextClosure.LRClose(context , "{" , "}");
                        if (ClosedResult.Errors.Count > 0)
                        {
                            FinalResult.Errors = ClosedResult.Errors;
                            return FinalResult;
                        }
                        if (ClosedResult.Result == null)
                        {
                            return FinalResult;
                        }

                    }
                    TreeNode node = new TreeNode();
                    node.Segment = HEAD.Duplicate();
                    node.Segment.content = StructureName;
                }
            }
            return FinalResult;
        }
    }
}