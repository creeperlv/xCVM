using Cx.Core.DataTools;
using Cx.Core.SegmentContextUtilities;
using Cx.Core.VCParser;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Parsers
{
    public class UsingParser : RootParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> __result = false;
            {
                var r = context.Match("using");
                if (r == MatchResult.Match)
                {
                    context.GoNext();
                    var cr = ContextClosure.RClose(context , ";");
                    if (cr.Errors.Count > 0)
                    {
                        __result.Errors = cr.Errors;
                        return __result;
                    }
                    if(cr.Result!=null)
                    {
                        var namespace_name = cr.Result;
                        var FormedPrefix = "";
                        bool Continue = true;
                        while (Continue)
                        {
                            if (namespace_name.Current == namespace_name.EndPoint)
                            {
                                break;
                            }
                            var dt = DataTypeChecker.DetermineDataType(namespace_name.Current?.content ?? "");
                            switch (dt)
                            {
                                case DataType.String:
                                    {
                                        var seg = (namespace_name.Current?.content ?? "");
                                        FormedPrefix += seg;
                                    }
                                    break;
                                case DataType.Symbol:
                                    {
                                        var seg = (namespace_name.Current?.content ?? "");
                                        if (seg == ".")
                                        {
                                            FormedPrefix += "_";
                                        }
                                        else if (seg == "{")
                                        {
                                            FormedPrefix += "_";
                                            Continue = false;
                                            break;
                                        }
                                        else
                                        {
                                            var __r = new OperationResult<bool>(false);
                                            __r.AddError(new IllegalIdentifierError(namespace_name.Current));
                                            return __r;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        var __r = new OperationResult<bool>(false);
                                        __r.AddError(new IllegalIdentifierError(namespace_name.Current));
                                        return __r;
                                    }
                            }
                            namespace_name.GoNext();
                        }
                        {
                            ASTNode node = new ASTNode();
                            node.Type = HLASTNodeType.Using;
                            if (namespace_name.HEAD == null)
                            {
                                return __result;
                            }
                            node.Segment = namespace_name.HEAD.Duplicate();
                            node.Segment.content = FormedPrefix;
                        }
                    }
                }
            }
            return __result;
        }
    }
}
