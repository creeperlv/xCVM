﻿using Cx.Core;
using Cx.Core.DataValidation;
using Cx.Core.VCParser;
using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC
{
    public class NamespaceParser : ContextualParser
    {
        public NamespaceParser()
        {
            ConcernedParsers.Add(ASTNodeType.DeclareFunc);
            ConcernedParsers.Add(ASTNodeType.DeclareStruct);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(false);
            var nr = context.Match("namespace");
            var BRKPNT = context.Current;
            if (nr == MatchResult.Match)
            {
                var HEAD = context.Current;
                Segment? FirstLB = null;
                while (true)
                {
                    var res = context.MatchMarch("{");
                    if (res == MatchResult.Match)
                    {
                        FirstLB = context.Last;
                    }
                    else if (res == MatchResult.Mismatch)
                    {
                        context.GoNext();
                    }
                    else
                    {
                        break;
                    }
                }
                if(FirstLB == null)
                {
                    result.AddError(new ExpectAMarkError(HEAD , "{"));
                    return result;
                }
                SegmentContext namespace_name = new SegmentContext(HEAD);
                namespace_name.SetEndPoint(FirstLB);
                string FormedPrefix = "";
                bool Continue = true;
                ASTNode root = new ASTNode();
                root.Type = HLASTNodeType.Namespace;
                var __head = context.Current;
                if (__head == null)
                {
                    result.AddError(new UnexpectedEndError(__head));
                    return result;
                }
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
                                }else if (seg == "{")
                                {
                                    FormedPrefix += "_";
                                    Continue = false;
                                    break;
                                }
                                else
                                {

                                    var r = new OperationResult<bool>(false);
                                    r.AddError(new IllegalIdentifierError(namespace_name.Current));
                                    return r;
                                }
                            }
                            break;
                        default:
                            {
                                var r = new OperationResult<bool>(false);
                                r.AddError(new IllegalIdentifierError(namespace_name.Current)) ;
                                return r;
                            }
                            break;
                    }
                    namespace_name.GoNext();
                }
                var __seg = __head.Duplicate();
                __seg.content = __seg.content = FormedPrefix;
                root.Segment = __seg;
            }
            else
            {
                context.SetCurrent(BRKPNT);
            }
            return result;
        }
    }
}
