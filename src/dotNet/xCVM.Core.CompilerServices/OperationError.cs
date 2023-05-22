using LibCLCC.NET.TextProcessing;

namespace xCVM.Core.CompilerServices
{
    public class OperationError
    {
        public Segment? Segment;

        public OperationError(Segment? binded, string? msg = "")
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
    public class VersionFormatError : OperationError
    {
        public VersionFormatError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to System.Version.";
    }
    public class UnexpectedEndOfFileError : OperationError
    {
        public UnexpectedEndOfFileError(Segment? binded) : base(binded , null)
        {
        }
        public override string Message => $"End at unexpected location.";
    }
    public class UnexpectedEndError : OperationError
    {
        public UnexpectedEndError(Segment? binded) : base(binded , null)
        {
        }
        public override string Message => $"End at unexpected location.";
    }
    public class IntParseError : OperationError
    {
        public IntParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Int32.";
    }
    public class UIntParseError : OperationError
    {
        public UIntParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to unsigned int32.";
    }
    public class LongParseError : OperationError
    {
        public LongParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Int64.";
    }
    public class FloatParseError : OperationError
    {
        public FloatParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Single.";
    }
    public class DoubleParseError : OperationError
    {
        public DoubleParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Single.";
    }
    public class RegisterFormatError : OperationError
    {
        public RegisterFormatError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Should start with $.";
    }
    public class UnknownInstructionError : OperationError
    {
        public UnknownInstructionError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Unknown instruction.";
    }
    public class UnknownParameterError : OperationError
    {
        public UnknownParameterError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Parameter cannot be converted.";
    }
    public class MustEndWithSemicolonError : OperationError
    {
        public MustEndWithSemicolonError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Must end with semicolon error.";
    }
    public class MustEndWithSpecifiedEndMarkError : OperationError
    {
        string end_mark;
        public MustEndWithSpecifiedEndMarkError(Segment? binded, string end_mark) : base(binded, null)
        {
            this.end_mark = end_mark;
        }
        public override string Message => $"Must end with {end_mark}.";
    }
    public class AssemblerNotDefinedError : OperationError
    {
        public AssemblerNotDefinedError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Assembler Not Defined.";
    }
    public class ExpectAMarkError : OperationError
    { 
        string __mark;
        public ExpectAMarkError(Segment? binded,string mark) : base(binded, null)
        {
            __mark=mark;
        }
        public override string? Message => $"Except \"{__mark}\".";
    }
}
