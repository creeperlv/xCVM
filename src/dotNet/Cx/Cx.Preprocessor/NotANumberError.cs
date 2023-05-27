using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Preprocessor
{
    public class NotANumberError : OperationError
    {

        public NotANumberError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Not a number.";
    }
    public class UnableToParseExpressionError : OperationError
    {

        public UnableToParseExpressionError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Unable to parse the expression.";
    }
}
