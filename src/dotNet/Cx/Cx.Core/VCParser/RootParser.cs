﻿using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class RootParser : ContextualParser
    {
        public RootParser()
        {
            ConcernedParsers.Add(ASTNodeType.DeclareFunc);
        }
        public override OperationResult<bool> Parse(ParserProvider provider, SegmentContext context, ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            while (true)
            {
                if (context.ReachEnd) break;
                if (context.Current == null) break;
                if (context.Current.Next == null) break;
                var Hit = false;
                var __current= context.Current;
                foreach (var id in ConcernedParsers)
                {
                    var item = provider.GetParser(id);
                    if (item == null)
                    {
                        result.AddError(new ParserNotFoundError(context.Current));
                        return result;
                    }
                    context.SetCurrent(__current);
                    var _result = item.Parse(provider, context, Parent);
                    if (_result.Result == true)
                    {
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