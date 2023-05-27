using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Core.CodeGen
{
    public class GeneratorNotFound : OperationError
    {
        public GeneratorNotFound(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Generator Not Found.";
    }
}
