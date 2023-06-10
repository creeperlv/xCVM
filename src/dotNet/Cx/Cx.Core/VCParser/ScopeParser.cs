using Cx.Core.SegmentContextUtilities;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class ScopeParser : ContextualParser
    {
        public ScopeParser()
        {
            ConcernedParsers.Add(ASTNodeType.Return);
            ConcernedParsers.Add(ASTNodeType.If);
            ConcernedParsers.Add(ASTNodeType.While);
            ConcernedParsers.Add(ASTNodeType.For);
            ConcernedParsers.Add(ASTNodeType.Switch);
            ConcernedParsers.Add(ASTNodeType.Statement);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(false);
            var HEAD = context.Current;
            if (HEAD == null) return result;
            if (HEAD.content == "{")
            {
                var BODY = ContextClosure.LRClose(context , "{" , "}");
                if (BODY.Errors.Count > 0)
                {
                    result.Errors = BODY.Errors;
                    return result;
                }
                if (BODY.Result == null)
                {
                    result.AddError(new OperationError(context.Current , "Closure Failed"));
                }
                TreeNode scope_body = new TreeNode();
                scope_body.Type = ASTNodeType.Scope;

                Parent.AddChild(scope_body);
                result.Result = true;
            }
            else
            {

            }
            return result;
        }
    }
}