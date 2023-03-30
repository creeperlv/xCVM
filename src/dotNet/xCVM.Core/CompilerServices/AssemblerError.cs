using LibCLCC.NET.TextProcessing;

namespace xCVM.Core.CompilerServices
{
    public class AssemblerError
    {
        public Segment? Segment;

        public AssemblerError(Segment? binded, string? msg="")
        {
            Segment = binded;
            _msg = msg;
        }

        string? _msg;
        public virtual string? Message { get=>_msg; }
        public override string ToString()
        {
            return Message??"";
        }
    }
    public class VersionFormatError:AssemblerError
    {
        public VersionFormatError(Segment? binded) : base(binded,null)
        {
        }

        public override string Message => $"Cannot convert content to System.Version.";
    }
}
