using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Preprocessor
{
    public class NotABoolError : OperationError
    {

        public NotABoolError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Not a bool.";
    }
}
