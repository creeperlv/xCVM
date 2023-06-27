using Cx.Core.DataTools;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class CombinedAssignParser : ContextualParser
    {
        static string [ ] symbols = { "+" , "-" , "*" , "/" , "%" , "&" , "|" };
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
            node.Type = ASTNodeType.CombinedAssign;
            var c = context.Current;
            if (c == null)
            {
                result.AddError(new UnexpectedEndError(c));
                return result;
            }
            if (DataTypeChecker.DetermineDataType(c.content) == DataType.String)
            {
                context.GoNext();
                var s = context.Current;

                if (s == null)
                {
                    result.AddError(new UnexpectedEndError(s));
                    return result;
                }
                var SMR = context.MatchCollectionMarchReturnName(symbols);
                if(SMR.Item1== MatchResult.Match)
                {
                    if (context.Match("=") == MatchResult.Match)
                    {
                        TreeNode treeNode = new TreeNode();
                        treeNode.Type = ASTNodeType.CombinedSymbol;
                        treeNode.Segment = s;
                        node.AddChild(treeNode);
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
                    return false;
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