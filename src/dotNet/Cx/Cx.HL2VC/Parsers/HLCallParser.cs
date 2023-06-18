using Cx.Core;
using Cx.Core.DataTools;
using Cx.Core.SegmentContextUtilities;
using Cx.Core.VCParser;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Parsers
{
    public class HLCallParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            if (context.Current == null) { return FinalResult; }
            var dt = DataTypeChecker.DetermineDataType(context.Current.content);
            var HEAD = context.Current;
            if (dt == DataType.String)
            {
                var name_result = Utilities.FormName(context , false);
                if (FinalResult.CheckAndInheritAbnormalities(name_result))
                {
                    return FinalResult;
                }
                var LP = context.Match("(");
                if (LP == MatchResult.Match)
                {
                    TreeNode node = new TreeNode();
                    node.Segment = HEAD.Duplicate();
                    node.Segment.content = name_result.Result;
                    node.Type=ASTNodeType.Call;
                    var ParametersClosure = ContextClosure.LRClose(context , "(" , ")");
                    if (FinalResult.CheckAndInheritAbnormalities(ParametersClosure))
                    {
                        return FinalResult;
                    }
                    if (ParametersClosure.Result == null)
                    {
                        FinalResult.AddError(new ClosureError(context.Current , "(" , ")"));
                        return FinalResult;
                    }
                    var ClosedParametersContext = ParametersClosure.Result;
                    if(ClosedParametersContext.Current?.Next==ClosedParametersContext.EndPoint)
                    {
                        Parent.AddChild(node);
                        return true;
                    }
                    else
                    {

                    }
                    return FinalResult;
                }
                else
                    return FinalResult;
            }
            else
                return FinalResult;
        }
    }
}
