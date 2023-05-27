using System.Collections.Generic;
using Cx.Core.VCParser;
using xCVM.Core.CompilerServices;

namespace Cx.Core
{
    public abstract class ContextualParser
    {
        public List<int> ConcernedParsers = new List<int>();
        public abstract OperationResult<bool> Parse(ParserProvider provider,SegmentContext context , ASTNode Parent);
    }
}