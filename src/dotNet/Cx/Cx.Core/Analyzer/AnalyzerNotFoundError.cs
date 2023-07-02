using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
    public class AnalyzerNotFoundError : OperationError
    {
        int Type;
        public AnalyzerNotFoundError(Segment? binded , int Type) : base(binded , null)
        {
           this.Type = Type;
        }
        public override string? Message => $"Analyzer not found:{Type}";
    }
}
