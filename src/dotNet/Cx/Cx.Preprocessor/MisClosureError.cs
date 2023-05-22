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
    public class ElifWithoutIfError : OperationError
    {
        public ElifWithoutIfError(Segment? binded) : base(binded , null) { }
        public override string Message => $"#elif without #if";
    }
    public class ElseWithoutIfError : OperationError
    {
        public ElseWithoutIfError(Segment? binded) : base(binded , null) { }
        public override string Message => $"#else without #if";
    }
    public class EndIfWithoutIfError : OperationError
    {
        public EndIfWithoutIfError(Segment? binded) : base(binded , null) { }
        public override string Message => $"#endif without #if";
    }
}
