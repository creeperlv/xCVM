using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class RootParser : ContextualParser
    {
        public RootParser()
        {
            ConcernedParsers.Add(ASTNodeType.Extern);
            ConcernedParsers.Add(ASTNodeType.DeclareStruct);
            ConcernedParsers.Add(ASTNodeType.DeclareFunc);
            //ConcernedParsers.Add(ASTNodeType.TypeDef);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            while (true)
            {
                if (context.ReachEnd) break;
                if (context.Current == null) break;
                if (context.Current.Next == null) break;
                var Hit = false;
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
                }

            }
            return result;
        }
    }

}