using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    /// <summary>
    /// Proxy Parser
    /// </summary>
    public class AllStatementParser : ContextualParser
    {
        public AllStatementParser()
        {
            ConcernedParsers.Add(IntermediateASTNodeType.Intermediate_NonSemicolonStatement);
            //ConcernedParsers.Add(ASTNodeType.Statement);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            var Current = context.Current;
            foreach (var id in ConcernedParsers)
            {
                var Parser = provider.GetParser(id);
                if (Parser == null)
                {
                    OperationResult<bool> or = false;
                    or.AddError(new ParserNotFoundError(context.Current , id));
                    return or;
                }
                else
                {
                    context.SetCurrent(Current);
                    var PR = Parser.Parse(provider , context , Parent);
                    if (PR.Result) { return PR; }

                    if (PR.Errors.Count > 0)
                    {
                        return PR;
                    }
                }
            }
            {
                return false;
            }
        }
    }
    /// <summary>
    /// Proxy Parser
    /// </summary>
    public class AllStatementAndScopeParser : ContextualParser
    {
        public AllStatementAndScopeParser()
        {
            ConcernedParsers.Add(ASTNodeType.Scope);
            ConcernedParsers.Add(IntermediateASTNodeType.Intermediate_AllStatement);
            //ConcernedParsers.Add(ASTNodeType.Statement);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            var Current = context.Current;
            foreach (var id in ConcernedParsers)
            {
                var Parser = provider.GetParser(id);
                if (Parser == null)
                {
                    OperationResult<bool> or = false;
                    or.AddError(new ParserNotFoundError(context.Current , id));
                    return or;
                }
                else
                {
                    context.SetCurrent(Current);
                    var PR = Parser.Parse(provider , context , Parent);
                    if (PR.Result) { return PR; }

                    if (PR.Errors.Count > 0)
                    {
                        return PR;
                    }
                }
            }
            {
                return false;
            }
        }
    }
}