using LibCLCC.NET.TextProcessing;

namespace xCVM.Core.CompilerServices
{
    public class AssemblerError
    {
        public Segment? Segment;

        public AssemblerError(Segment? binded, string? msg = "")
        {
            Segment = binded;
            _msg = msg;
        }

        string? _msg;
        public virtual string? Message { get => _msg; }
        public override string ToString()
        {
            return Message ?? "";
        }
    }
    public class VersionFormatError : AssemblerError
    {
        public VersionFormatError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to System.Version.";
    }
    public class UnexpectedEndOfFileError : AssemblerError
    {
        public UnexpectedEndOfFileError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"End at unexpected location.";
    }
    public class IntParseError : AssemblerError
    {
        public IntParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Int32.";
    }
    public class LongParseError : AssemblerError
    {
        public LongParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Int64.";
    }
    public class FloatParseError : AssemblerError
    {
        public FloatParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Single.";
    }
    public class DoubleParseError : AssemblerError
    {
        public DoubleParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Single.";
    }
    public class RegisterFormatError : AssemblerError
    {
        public RegisterFormatError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Should start with $.";
    }
    public class UnknownInstructionError : AssemblerError
    {
        public UnknownInstructionError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Unknown instruction.";
    }
    public class UnknownParameterError : AssemblerError
    {
        public UnknownParameterError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Parameter cannot be converted.";
    }
    public class MustEndWithSemicolonError : AssemblerError
    {
        public MustEndWithSemicolonError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Must end with semicolon error.";
    }
    public class MustEndWithSpecifiedEndMarkError : AssemblerError
    {
        string end_mark;
        public MustEndWithSpecifiedEndMarkError(Segment? binded, string end_mark) : base(binded, null)
        {
            this.end_mark = end_mark;
        }
        public override string Message => $"Must end with {end_mark}.";
    }
}
