using Cx.Core.DataTools;
using Cx.Core.SegmentContextUtilities;
using LibCLCC.NET.TextProcessing;
using System;
using System.Linq;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class FunctionParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            ContextualParser? typeParser = provider.GetParser(ASTNodeType.DataType);
            if (typeParser == null)
            {
                OperationResult<bool> _result = new OperationResult<bool>(false);
                return _result;
            }
            OperationResult<bool> result = new OperationResult<bool>(true);
            TreeNode FuncDef = new TreeNode
            {
                Type = ASTNodeType.DeclareFunc
            };
#if DEBUG
            Console.WriteLine($"FuncParser: {context.Current?.content ?? "null"}");
#endif
            var HEAD = context.Current;
            {
                TreeNode return_type = new TreeNode();
                FuncDef.AddChild(return_type);
                return_type.Type = ASTNodeType.ReturnType;
                var RetTpeRes=typeParser.Parse(provider , context, return_type);
                if (result.CheckAndInheritAbnormalities(RetTpeRes)) return result;
                if (!RetTpeRes.Result)
                {
                    result.AddError(new ParseFailError(HEAD , ASTNodeType.DataType));
                }
            }
#if DEBUG
            Console.WriteLine($"FuncParser: ReturnType Done, At: {context.Current?.content ?? "null"}");
#endif
            {
                FuncDef.Segment = context.Current;
                context.GoNext();
            }
            {
                TreeNode parameters = new TreeNode();
                FuncDef.AddChild(parameters);
                parameters.Type = ASTNodeType.Parameters;
                TreeNode node = new TreeNode();
                //context.SetCurrent(FirstLP.Prev);
#if DEBUG
                Console.WriteLine($"Function Parser: Parameters");
#endif
                var ParameterClosure = ContextClosure.LRClose(context , "(" , ")");
                if (result.CheckAndInheritAbnormalities(ParameterClosure))
                {
                    return result;
                }
                else if (ParameterClosure.Result == null)
                {
#if DEBUG
                    Console.WriteLine($"Function Parser: Parameters Closure failed");
#endif
                    result.Result = false;
                    return result;
                }
#if DEBUG
                Console.WriteLine($"Function Parser: LRClosed.");
#endif
                SegmentContext _context = ParameterClosure.Result;
                var DeclareVariableParser = provider.GetParser(ASTNodeType.DeclareVar);
                if (DeclareVariableParser == null)
                {
                    result.Errors.Add(new ParserNotFoundError(_context.Current , ASTNodeType.DeclareVar));
                    return result;
                }
                _context.GoNext();
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

#if DEBUG
                    Console.WriteLine($"FuncParser: Paras: \x1b[33mReady Try\x1b[0m, at: {_context.Current?.content ?? "null"}");
#endif
                    var par_res = DeclareVariableParser.Parse(provider , _context , parameters);
                    if (result.CheckAndInheritAbnormalities(par_res))
                    {
#if DEBUG
                        Console.WriteLine($"FuncParser: Paras: \x1b[31mFailed\x1b[0m, at: {_context.Current?.content ?? "null"}");
#endif
                        return result;
                    }

                    if (par_res.Result == false)
                    {
#if DEBUG
                        Console.WriteLine($"FuncParser: Paras: \x1b[31mFailed\x1b[0m, at: {_context.Current?.content ?? "null"}");
#endif
                        result.AddError(new ParseFailError(_context.Current , ASTNodeType.DeclareVar));
                        return result;
                    }
                    parameters.Children.Last().Type = ASTNodeType.SingleParameter;
                    _context.GoNext();
#if DEBUG
                    Console.WriteLine($"FuncParser: Paras: Done one 0, at: {_context.Current?.content ?? "null"}");
#endif
                    if (_context.Match(")") == MatchResult.Match)
                    {
#if DEBUG
                        Console.WriteLine($"FuncParser: Stop at }}.");
#endif
                        break;
                    }
                    else if (_context.Match(",") == MatchResult.Match)
                    {
                        _context.GoNext();
                    }
                    else if(!_context.ReachEnd)
                    {
                        result.AddError(new IllegalIdentifierError(_context.Current));
                        return result;
                    }
#if DEBUG
                    Console.WriteLine($"FuncParser: Paras: Done one 1, at: {_context.Current?.content??"null"}");
#endif
                  
                }
#if DEBUG
                Console.WriteLine($"Function Parser: Parameters Done, EndPoint:{_context.EndPoint?.content ?? "null"}");
#endif
                context.SetCurrent(_context.EndPoint!.Next);
            }
            {
#if DEBUG
                Console.WriteLine($"Function Parser: Parameters Done, Current:{context.Current?.content ?? "null"}");
#endif
                ContextualParser? ScopeParser = provider.GetParser(ASTNodeType.Scope);
                if (ScopeParser == null)
                {
                    result.AddError(new ParserNotFoundError(context.Current , ASTNodeType.Scope));
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
#if DEBUG
                Console.WriteLine($"FuncParse::{context.Current.content}");
#endif
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