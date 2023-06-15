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
                TreeNode return_type = new TreeNode();
                FuncDef.AddChild(return_type);
                return_type.Type = ASTNodeType.ReturnType;
                var RetTpeRes=typeParser.Parse(provider , new SegmentContext(HEAD) , return_type);
                if (result.CheckAndInheritAbnormalities(RetTpeRes)) return result;
                if (!RetTpeRes.Result)
                {
                    result.AddError(new ParseFailError(HEAD , ASTNodeType.DataType));
                }
            }
            {
                TreeNode parameters = new TreeNode();
                FuncDef.AddChild(parameters);
                parameters.Type = ASTNodeType.Parameters;
                TreeNode node = new TreeNode();
                context.SetCurrent(FirstLP.Prev);
#if DEBUG
                Console.WriteLine($"Function Parser: Parameters");
#endif
                var ParameterClosure = ContextClosure.LRClose(context , "(" , ")");
                if (ParameterClosure.Errors.Count > 0)
                {
                    result.Result = false;
                    result.Errors = ParameterClosure.Errors;
                    return result;
                }
                else if (ParameterClosure.Result == null)
                {
                    result.Result = false;
                    return result;
                }
                SegmentContext _context = ParameterClosure.Result;
                var DeclareVariableParser = provider.GetParser(ASTNodeType.DeclareVar);
                if (DeclareVariableParser == null)
                {
                    result.Errors.Add(new ParserNotFoundError(_context.Current , ASTNodeType.DeclareVar));
                    return result;
                }
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
                    var par_res = DeclareVariableParser.Parse(provider , _context , parameters);
                    if (result.CheckAndInheritAbnormalities(par_res)) return result;
                    if (par_res.Result == false)
                    {
                        result.AddError(new ParseFailError(_context.Current , ASTNodeType.DeclareVar));
                        return result;
                    }
                    parameters.Children.Last().Type = ASTNodeType.SingleParameter;
                    _context.GoNext();
                    if (_context.Current.content == ")")
                    {
                        break;
                    }
                    else if (_context.Current.content == ",")
                    {
                        _context.GoNext();
                    }
                    else
                    {
                        result.AddError(new IllegalIdentifierError(_context.Current));
                        return result;
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