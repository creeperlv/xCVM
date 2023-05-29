using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class ParserNotFoundError : OperationError
    {
        public ParserNotFoundError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Parser Not Found.";
    }
    public class IllegalIdentifierError : OperationError
    {
        public IllegalIdentifierError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Illegal Identifier Error.";
    }
}