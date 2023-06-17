using Cx.Core.SegmentContextUtilities;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class IfParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> FinalResult = new OperationResult<bool>(false);
            if (context.Match("if") == MatchResult.Match)
            {
                context.GoNext();
                var MR_LP = context.Match("(");
                if (MR_LP == MatchResult.Match)
                {
                    TreeNode node = new TreeNode();
                    node.Type = ASTNodeType.If;
                    var ConditionClosure = ContextClosure.LRClose(context , "(" , ")");
                    if (FinalResult.CheckAndInheritAbnormalities(ConditionClosure)) { return FinalResult; }
                    if (ConditionClosure.Result == null)
                    {
                        FinalResult.AddError(new ClosureError(context.Current , "(" , ")"));
                        return FinalResult;
                    }
                    var ClosedContext = ConditionClosure.Result;
                    var ExpressionParser = provider.GetParser(ASTNodeType.Expression);
                    if (ExpressionParser == null)
                    {
                        FinalResult.AddError(new ParserNotFoundError(context.Current , ASTNodeType.Expression));
                        return FinalResult;
                    }
                    var EPResult = ExpressionParser.Parse(provider , ClosedContext , node);
                    if (FinalResult.CheckAndInheritAbnormalities(EPResult)) return FinalResult;
                    if (FinalResult.Result == false)
                    {
                        FinalResult.AddError(new ParseFailError(context.Current , ASTNodeType.Expression));
                        return FinalResult;
                    }
                    context.SetCurrent(ClosedContext.EndPoint);
                    context.GoNext();
                    var AllStatement = provider.GetParser(IntermediateASTNodeType.Intermediate_AllStatement);
                    if(AllStatement==null)
                    {
                        FinalResult.AddError(new ParserNotFoundError(context.Current , IntermediateASTNodeType.Intermediate_AllStatement));
                        return FinalResult;
                    }
                    var ASResult=AllStatement.Parse(provider , context , node);
                    if (FinalResult.CheckAndInheritAbnormalities(ASResult)) return FinalResult;
                    if(FinalResult.Result == false)
                    {
                        FinalResult.AddError(new StatementRequiredError(context.Current));
                        return FinalResult;
                    }
                    if (context.Match("else") == MatchResult.Match)
                    {
                        context.GoBack();
                    }
                }
                else if (MR_LP == MatchResult.Mismatch)
                {

                    FinalResult.AddError(new IllegalIdentifierError(context.Current));
                    return FinalResult;
                }
                else
                {
                    FinalResult.AddError(new UnexpectedEndError(context.Current));
                    return FinalResult;
                }
            }
            return FinalResult;
        }
    }
}