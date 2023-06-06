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
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            ASTNode root = new ASTNode();
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
                    FinalResult.AddError(new ParserNotFoundError(context.Current));
                    return FinalResult;
                }
            }
            return FinalResult;
        }
    }
}