using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Core.CodeGen
{
    public class GeneratorNotFoundError : OperationError
    {
        public GeneratorNotFoundError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Generator Not Found.";
    }
}
