using Cx.Core.DataTools;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class AssignParser : ContextualParser
    {

        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> result = false;

            var ExpParser = provider.GetParser(ASTNodeType.Expression);
            if (ExpParser == null)
            {
                result.AddError(new ParserNotFoundError(context.Current , ASTNodeType.Expression));
                return result;
            }
            TreeNode node = new TreeNode();
            node.Type = ASTNodeType.Assign;
            var c = context.Current;
            if (c == null)
            {
                result.AddError(new UnexpectedEndError(c));
                return result;
            }
            if (DataTypeChecker.DetermineDataType(c.content) == DataType.String)
            {
                context.GoNext();
                if (context.Match("=") == MatchResult.Match)
                {
                    var EXPR = ExpParser.Parse(provider , context , node);
                    if (result.CheckAndInheritAbnormalities(EXPR))
                    {
                        return result;
                    }
                    result.Result = true;
                    node.Segment = c;
                    Parent.AddChild(node);
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return result;
            }

            return result;
        }
    }
}