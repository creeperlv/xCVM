using LibCLCC.NET.TextProcessing;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class ParserNotFoundError : OperationError
    {
        int _TargetType;
        public ParserNotFoundError(Segment? binded , int TargetType) : base(binded , null)
        {
            _TargetType = TargetType;
        }

        public override string Message => $"Parser Not Found:{_TargetType}";
    }
    public class StatementRequiredError : OperationError
    {
        public StatementRequiredError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Require a statement.";
    }
    public class ParseFailError : OperationError
    {
        int _TargetType;
        public ParseFailError(Segment? binded , int TargetType) : base(binded , null)
        {
            _TargetType = TargetType;
        }

        public override string Message => $"Parser Not Found:{_TargetType}";
    }
    public class IllegalIdentifierError : OperationError
    {
        public IllegalIdentifierError(Segment? binded) : base(binded , null)
        {
        }

        public override string Message => $"Illegal Identifier Error.";
    }
    public class CannotFoundNodeError : OperationError
    {
        int TargetNodeType;
        public CannotFoundNodeError(Segment? binded , int TargetNodeType) : base(binded , null)
        {
            this.TargetNodeType = TargetNodeType;
        }

        public override string Message => $"Cannot Found Target Node({TargetNodeType})";
    }
    public class ClosureError : OperationError
    {
        string L;
        string R;
        public ClosureError(Segment? binded , string L , string R) : base(binded , null)
        {
            this.L = L;
            this.R = R;
        }

        public override string Message => $"Cannot perform correct LRClosure:{L},{R}";
    }
    public class WrongSubASTNode : OperationError
    {
        int [ ] ASTNodeIDs;
        public WrongSubASTNode(Segment? binded , params int [ ] ASTNodeIDs) : base(binded , null)
        {
            this.ASTNodeIDs = ASTNodeIDs;
        }
        string? _message = null;
        public override string Message
        {
            get
            {
                if (_message == null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("Mismatch ASTNodes:");
                    stringBuilder.Append(string.Join(", " , ASTNodeIDs));
                    _message = stringBuilder.ToString();
                }
                return _message;
            }
        }
    }
}