using LibCLCC.NET.TextProcessing;

namespace xCVM.Core.CompilerServices
{
    public class CompileError
    {
        public Segment? Segment;

        public CompileError(Segment? binded, string? msg = "")
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
    public class VersionFormatError : CompileError
    {
        public VersionFormatError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to System.Version.";
    }
    public class UnexpectedEndOfFileError : CompileError
    {
        public UnexpectedEndOfFileError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"End at unexpected location.";
    }
    public class IntParseError : CompileError
    {
        public IntParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Int32.";
    }
    public class UIntParseError : CompileError
    {
        public UIntParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to unsigned int32.";
    }
    public class LongParseError : CompileError
    {
        public LongParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Int64.";
    }
    public class FloatParseError : CompileError
    {
        public FloatParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Single.";
    }
    public class DoubleParseError : CompileError
    {
        public DoubleParseError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Cannot convert content to Single.";
    }
    public class RegisterFormatError : CompileError
    {
        public RegisterFormatError(Segment? binded) : base(binded, null)
        {
        }

        public override string Message => $"Should start with $.";
    }
    public class UnknownInstructionError : CompileError
    {
        public UnknownInstructionError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Unknown instruction.";
    }
    public class UnknownParameterError : CompileError
    {
        public UnknownParameterError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Parameter cannot be converted.";
    }
    public class MustEndWithSemicolonError : CompileError
    {
        public MustEndWithSemicolonError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Must end with semicolon error.";
    }
    public class MustEndWithSpecifiedEndMarkError : CompileError
    {
        string end_mark;
        public MustEndWithSpecifiedEndMarkError(Segment? binded, string end_mark) : base(binded, null)
        {
            this.end_mark = end_mark;
        }
        public override string Message => $"Must end with {end_mark}.";
    }
    public class AssemblerNotDefinedError : CompileError
    {
        public AssemblerNotDefinedError(Segment? binded) : base(binded, null)
        {
        }
        public override string Message => $"Assembler Not Defined.";
    }
    public class ExpectAMarkError : CompileError
    { 
        string __mark;
        public ExpectAMarkError(Segment? binded,string mark) : base(binded, null)
        {
            __mark=mark;
        }
        public override string? Message => $"Except \"{__mark}\".";
    }
}
