using Cx.Core;
using Cx.Core.DataValidation;
using Cx.Core.VCParser;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Parsers
{
    public class HLTypeParser : Parser
    {
        public override OperationResult<bool> Parse(ParserProvider provider, SegmentContext context, ASTNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            var Current = context.Current;
            string FormedType = "";
            ASTNode _node = new ASTNode();
            _node.Type = ASTNodeType.DataType;
            while (true)
            {
                if (context.ReachEnd)
                {
                    break;
                }
                if (Current == null)
                {
                    break;
                }
                var DT = DataTypeChecker.DetermineDataType(Current.content);
                switch (DT)
                {
                    case DataType.String:
                        {
                            if (_node.Type == ASTNodeType.Pointer)
                            {
                                Parent.AddChild(_node);
                                FinalResult.Result = true;
                                return FinalResult;
                            }
                            FormedType += Current.content;
                        }
                        break;
                    case DataType.Symbol:
                        {
                            if (Current.content == ".")
                            {
                                if (_node.Type == ASTNodeType.Pointer)
                                {
                                    Parent.AddChild(_node);
                                    FinalResult.Result = true;
                                    return FinalResult;
                                }
                                FormedType += "_";
                            }
                            else if (Current.content == "*")
                            {
                                if (_node.Type == ASTNodeType.DataType)
                                {
                                    _node.Segment = Current.Duplicate();
                                    _node.Segment.content = FormedType;
                                }
                                ASTNode Pointer = new ASTNode();
                                Pointer.Segment = Current;
                                Pointer.Type = ASTNodeType.Pointer;
                                Pointer.AddChild(_node);
                                _node = Pointer;
                            }
                            else
                            {

                                Parent.AddChild(_node);
                                FinalResult.Result = true;
                                return FinalResult;
                            }
                        }
                        break;
                    default:
                        {
                            Parent.AddChild(_node);
                            FinalResult.Result = true;
                            return FinalResult;
                        }
                }
                context.GoNext();
            }
            {

                Parent.AddChild(_node);
                FinalResult.Result = true;
                return FinalResult;
            }
        }
    }
}
