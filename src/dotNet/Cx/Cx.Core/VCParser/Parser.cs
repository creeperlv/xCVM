using LibCLCC.NET.TextProcessing;
using System;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
    public class Parser : ContextualParser
    {
        public Parser()
        {
            ConcernedParsers.Add(ASTNodeType.Root);
        }
        public OperationResult<ASTNode> ParseAST(Segment HEAD)
        {
            SegmentContext segmentContext = new SegmentContext(HEAD);
            ASTNode root = new ASTNode();
            OperationResult<ASTNode> result = new OperationResult<ASTNode>(root);
            Parse(VanillaCParsers.GetProvider(), segmentContext, root);
            return result;
        }

        public override OperationResult<bool> Parse(ParserProvider provider, SegmentContext context, ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            return result;
        }
    }
    public class FunctionParser : ContextualParser
    {
        TypeParser typeParser;
        public FunctionParser()
        {
            typeParser = new TypeParser();
        }
        public override OperationResult<bool> Parse(ParserProvider provider, SegmentContext context, ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            ASTNode FuncDef = new ASTNode();
            FuncDef.Type = ASTNodeType.DeclareFunc;
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
                return new OperationResult<bool>(false);
            if (FirstLB == null)
                return new OperationResult<bool>(false);
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
            {
                ASTNode return_type = new ASTNode();
                FuncDef.AddChild(return_type);
                return_type.Type = ASTNodeType.ReturnType;
                typeParser.Parse(provider, new SegmentContext(HEAD), return_type);
            }
            {
                ASTNode node = new ASTNode();
                while (true)
                {
                    //
                    if (context.Current > FirstRP)
                    {
                        break;
                    }
                    var tr = typeParser.Parse(provider, context, node);
                    if (tr.Result == true)
                    {

                    }
                }
            }
            ASTNode parameters = new ASTNode();
            FuncDef.AddChild(parameters);
            parameters.Type= ASTNodeType.Parameters;
            while (true)
            {
                if (context.ReachEnd) break;
                if (context.Current == null) break;
                if (context.Current.Next == null) break;
                var Hit = false;
                foreach (var id in ConcernedParsers)
                {
                    var item = provider.GetParser(id);

                    var _result = item.Parse(provider, context, Parent);
                    if (_result.Result == true)
                    {
                        Hit = true;
                        break;
                    }
                }
                if (Hit == false)
                {
                    result.Result = false;
                }

            }
            return result;
        }
    }
    [Serializable]
    public class Token
    {
        public int TokenType;
    }
    public class ASTNode : TreeNode
    {
        public int Type;
        public Segment? Segment;
    }

}