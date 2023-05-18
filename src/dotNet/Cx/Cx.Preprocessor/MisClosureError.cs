using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Preprocessor
{
    public class MisClosureError : OperationError
    {
        public MisClosureError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Closure Identifier Mismatch.";
    }
}
