using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class ExternParser : ContextualParser
    {
        public ExternParser()
        {
            ConcernedParsers.Add(ASTNodeType.DeclareFunc);
            ConcernedParsers.Add(ASTNodeType.DeclareStruct);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            if (context.Match("extern") == MatchResult.Match)
            {
                bool Hit = false;
                var result = new OperationResult<bool>(false);
                context.GoNext();
                var __current = context.Current;
                foreach (var id in ConcernedParsers)
                {
                    var item = provider.GetParser(id);
                    if (item == null)
                    {
                        result.AddError(new ParserNotFoundError(context.Current , id));
                        return result;
                    }
                    context.SetCurrent(__current);
                    var _result = item.Parse(provider , context , Parent);
                    if (_result.Result == true)
                    {
                        __current = context.Current;
                        Hit = true;
                        break;
                    }
                }
                if (Hit == false)
                {
                    result.Result = false;
                    foreach (var item in ConcernedParsers)
                    {
                        result.AddError(new ParseFailError(context.Current , item));
                    }
                    return result;
                }
                result.Result = true;
                return result;
            }
            return false;
        }
    }
}