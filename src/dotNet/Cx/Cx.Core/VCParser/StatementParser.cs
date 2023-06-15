using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class StatementParser : ContextualParser
    {
        public StatementParser()
        {
            ConcernedParsers.Add(ASTNodeType.Assign);
            ConcernedParsers.Add(ASTNodeType.DeclareVar);
            ConcernedParsers.Add(ASTNodeType.Expression);
            ConcernedParsers.Add(ASTNodeType.Call);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            TreeNode root = new TreeNode();
            root.Type = ASTNodeType.Statement;
            foreach (var item in ConcernedParsers)
            {
                var parser = provider.GetParser(item);
                if (parser != null)
                {
                    var pr = parser.Parse(provider , context , root);
                    if (pr == true)
                    {
                        FinalResult.Result = true;
                        return FinalResult;
                    }
                }
                else
                {
                    FinalResult.AddError(new ParserNotFoundError(context.Current , item));
                    return FinalResult;
                }
            }
            return FinalResult;
        }
    }
}