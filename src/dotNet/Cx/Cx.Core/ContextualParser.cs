using xCVM.Core.CompilerServices;

namespace Cx.Core
{
    public abstract class ContextualParser
    {
        public List<ContextualParser> SubParsers = new List<ContextualParser>();
        public abstract OperationResult<bool> Parse(SegmentContext context , ASTNode Parent);
    }

}