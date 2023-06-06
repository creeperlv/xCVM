using Cx.Core.DataValidation;
using Cx.Core.SegmentContextUtilities;
using LibCLCC.NET.TextProcessing;
using System;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class FunctionParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , ASTNode Parent)
        {
            ContextualParser? typeParser = provider.GetParser(ASTNodeType.DataType);
            if (typeParser == null)
            {
                OperationResult<bool> _result = new OperationResult<bool>(false);
                return _result;
            }
            OperationResult<bool> result = new OperationResult<bool>(true);
            ASTNode FuncDef = new ASTNode
            {
                Type = ASTNodeType.DeclareFunc
            };
            var HEAD = context.Current;
            Segment? FirstLP = null;
            Segment? FirstRP = null;
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
            context.SetCurrent(HEAD);
            while (true)
            {
                var res = context.MatchMarch("(");
                if (res == MatchResult.Match)
                {
                    FirstLP = context.Last;
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
            if (FirstLP == null)
            {
#if DEBUG
                Console.WriteLine("Function Parser:Missing First LP.");
#endif
                return new OperationResult<bool>(false);
            }

            if (FirstLB == null)
            {

#if DEBUG
                Console.WriteLine("Function Parser:Missing First LB.");
#endif
                return new OperationResult<bool>(false);
            }
            context.SetCurrent(FirstLP);
            while (true)
            {
                var res = context.MatchMarch(")");
                if (res == MatchResult.Match)
                {
                    FirstRP = context.Last;
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
            if (FirstLP > FirstLB)
            {
                return new OperationResult<bool>(false);
            }
            if (FirstLP == null)
                return new OperationResult<bool>(false);
            if (FirstRP == null)
            {
#if DEBUG
                Console.WriteLine("Function Parser:Missing First RP.");
#endif
                return new OperationResult<bool>(false);
            }
            {
                ASTNode return_type = new ASTNode();
                FuncDef.AddChild(return_type);
                return_type.Type = ASTNodeType.ReturnType;
                typeParser.Parse(provider , new SegmentContext(HEAD) , return_type);
            }
            {
                ASTNode parameters = new ASTNode();
                FuncDef.AddChild(parameters);
                parameters.Type = ASTNodeType.Parameters;
                ASTNode node = new ASTNode();
                context.SetCurrent(FirstLP.Prev);
#if DEBUG
                Console.WriteLine($"Function Parser: Parameters");
#endif
                var Parameters = ContextClosure.LRClose(context , "(" , ")");
                if (Parameters.Errors.Count > 0)
                {
                    result.Result = false;
                    result.Errors = Parameters.Errors;
                    return result;
                }
                else if (Parameters.Result == null)
                {
                    result.Result = false;
                    return result;
                }
                SegmentContext _context = Parameters.Result;
                while (true)
                {
                    //Parser Parameter
                    if (_context.ReachEnd)
                    {
                        break;
                    }
                    if (_context.Current == null)
                    {
#if DEBUG
                        Console.WriteLine("Function Parser: Unexpected end.");
#endif
                        var OR = new OperationResult<bool>(false);
                        OR.AddError(new UnexpectedEndError(_context.Current));
                        return OR;
                    }
                    //if (_context.Current > FirstRP)
                    //{
                    //    break;
                    //}
                    //if (_context.Current.Index >= FirstRP.Index)
                    //{
                    //    break;
                    //}
                    var content = _context.Current.content;
                    switch (DataTypeChecker.DetermineDataType(content))
                    {
                        case DataType.String:
                            {

                            }
                            break;
                        case DataType.Symbol:
                            {
                                if (content == ",")
                                {
                                    _context.GoNext();
                                    continue;
                                }
                                else
                                if (content == "(")
                                {
                                    _context.GoNext();
                                    continue;
                                }
                                else
                                if (content == ")")
                                {
                                    break;
                                }
                                else
                                {
#if DEBUG
                                    Console.WriteLine($"Function Parser: Illegal Identifier, Current:{_context.Current?.content ?? "null"}");
#endif
                                    var res = new OperationResult<bool>(false);
                                    res.AddError(new IllegalIdentifierError(_context.Current));
                                    return res;
                                }
                            }
                        default:
                            {
                                var res = new OperationResult<bool>(false);
                                res.AddError(new IllegalIdentifierError(_context.Current));
                                return res;
                            }
                    }
#if DEBUG
                    Console.WriteLine($"Function Parser: Will Type, Current:{_context.Current?.content ?? "null"}");
#endif
                    var tr = typeParser.Parse(provider , _context , node);
                    if (tr.Result == true)
                    {
                        _context.GoNext();
#if DEBUG
                        Console.WriteLine($"Function Parser: Type Done, Current:{_context.Current?.content ?? "null"}");
#endif
                        node.Type = ASTNodeType.SingleParameter;
                        node.Segment = _context.Current;
                        _context.GoNext();
#if DEBUG
                        Console.WriteLine($"Function Parser: Name Done, Current:{_context.Current?.content ?? "null"}");
#endif
                        parameters.AddChild(node);
                        node = new ASTNode();
                    }
                }
            }
            {
                context.GoNext();
#if DEBUG
                Console.WriteLine($"Function Parser: Parameters Done, Current:{context.Current?.content ?? "null"}");
#endif
                ContextualParser? ScopeParser = provider.GetParser(ASTNodeType.Scope);
                if (ScopeParser == null)
                {
                    result.AddError(new ParserNotFoundError(context.Current));
                    return result;
                }
                if (context.ReachEnd)
                {
                    result.AddError(new UnexpectedEndError(context.Current));
                    return result;
                }
                if (context.Current == null)
                {
                    result.AddError(new UnexpectedEndError(context.Current));
                    return result;
                }
                if (context.Current.content == ";")
                {
                    Parent.AddChild(FuncDef);
                    result.Result = true;
                }
                else
                {
                    var _result = ScopeParser.Parse(provider , context , FuncDef);
                    if (_result == true)
                    {

                        Parent.AddChild(FuncDef);
                        result.Result = true;
                    }
                    else
                    {

                    }
                }
            }

            return result;
        }
    }
}