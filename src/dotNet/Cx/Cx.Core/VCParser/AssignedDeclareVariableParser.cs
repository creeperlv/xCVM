using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class AssignedDeclareVariableParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> result = false;
            var TypeParser = provider.GetParser(ASTNodeType.DataType);
            if (TypeParser == null)
            {
                result.AddError(new ParserNotFoundError(context.Current , ASTNodeType.DataType));
                return result;
            }
            var ExpParser = provider.GetParser(ASTNodeType.Expression);
            if (ExpParser == null)
            {
                result.AddError(new ParserNotFoundError(context.Current , ASTNodeType.Expression));
                return result;
            }
            TreeNode node = new TreeNode();
            node.Type = ASTNodeType.AssignedDeclareVariable;
            var tr = TypeParser.Parse(provider , context , node);
            if (!tr.Result)
            {
                result.InheritAbnormalities(tr);
                return false;
            }
            var c = context.Current;
            context.GoNext();
            if (c == null)
            {
                result.AddError(new UnexpectedEndError(c));
                return result;
            }
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
            return result;
        }
    }
}