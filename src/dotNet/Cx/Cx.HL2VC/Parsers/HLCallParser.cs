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
                    node.Type = ASTNodeType.Call;
                    TreeNode parameters = new TreeNode();
                    parameters.Type = ASTNodeType.Arguments;
                    node.AddChild(parameters);
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
                    if (ClosedParametersContext.Current?.Next == ClosedParametersContext.EndPoint)
                    {
                        Parent.AddChild(node);
                        context.SetCurrent(ClosedParametersContext.EndPoint);
                        context.GoNext();
                        return true;
                    }
                    else
                    {
                        var ExpressionParser = provider.GetParser(ASTNodeType.Expression);
                        if (ExpressionParser == null)
                        {
                            FinalResult.AddError(new ParserNotFoundError(context.Current , ASTNodeType.Expression));
                            return FinalResult;
                        }
                        while (true)
                        {
                            if (ClosedParametersContext.ReachEnd) break;
                            if (ClosedParametersContext.Match(")") == MatchResult.Match) break;
                            var ValueResult = ExpressionParser.Parse(provider , ClosedParametersContext , parameters);
                            if (FinalResult.CheckAndInheritAbnormalities(ValueResult)) return FinalResult;
                            if (FinalResult.Result == false)
                            {
                                FinalResult.AddError(new ParseFailError(context.Current , ASTNodeType.Expression));
                                return FinalResult;
                            }
                            if (ClosedParametersContext.Match(",") == MatchResult.Match) { ClosedParametersContext.GoNext(); }
                            else
                            if (ClosedParametersContext.Match(")") == MatchResult.Match) { break; }
                            else
                            {
                                if (ClosedParametersContext.ReachEnd) break;
                                else
                                {
                                    FinalResult.AddError(new IllegalIdentifierError(ClosedParametersContext.Current));
                                    return FinalResult;
                                }
                            }
                        }
                    }
                    context.SetCurrent(ClosedParametersContext.EndPoint);
                    context.GoNext();
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
