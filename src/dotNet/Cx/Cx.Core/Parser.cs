using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Core
{
    public class Parser : ContextualParser
    {
        public Parser()
        {
            SubParsers.Add(new RootParser());
        }
        public OperationResult<ASTNode> ParseAST(Segment HEAD)
        {
            SegmentContext segmentContext = new SegmentContext(HEAD);
            ASTNode root = new ASTNode();
            OperationResult<ASTNode> result = new OperationResult<ASTNode>(root);
            Parse(segmentContext , root);
            return result;
        }

        public override OperationResult<bool> Parse(SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            return result;
        }
    }
    public class FunctionParser : ContextualParser
    {
        public override OperationResult<bool> Parse(SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            var isStruct = context.MatchMarch("struct");
            ASTNode FuncDef = new ASTNode();
            FuncDef.Type = ASTNodeType.DeclareFunc;
            if (isStruct == MatchResult.Match)
            {
                var isStructSymbol = context.MatchNext("{");
                if (isStructSymbol == MatchResult.Match)
                {
                    //It is a struct definition at first glances.
                    return new OperationResult<bool>(false);
                }
                else if (isStructSymbol == MatchResult.ReachEnd)
                {
                    result.AddError(new UnexpectedEndOfFileError(context.Current));
                }
                else
                {
                    var n0 = new ASTNode { Type = ASTNodeType.ReturnType , Segment = null };
                    var n1 = new ASTNode { Type = ASTNodeType.UseStruct , Segment = context.Current };
                    n0.Children.Add(n1);
                    FuncDef.Children.Add(n0);
                }
            }
            while (true)
            {
                if (context.ReachEnd) break;
                if (context.Current == null) break;
                if (context.Current.Next == null) break;
                var Hit = false;
                foreach (var item in SubParsers)
                {
                    var _result = item.Parse(context , Parent);
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
    public class RootParser : ContextualParser
    {
        public RootParser()
        {
            SubParsers.Add(new FunctionParser());
        }
        public override OperationResult<bool> Parse(SegmentContext context , ASTNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(true);
            while (true)
            {
                if (context.ReachEnd) break;
                if (context.Current == null) break;
                if (context.Current.Next == null) break;
                var Hit = false;
                foreach (var item in SubParsers)
                {
                    var _result = item.Parse(context , Parent);
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
        public ASTNodeType Type;
        public Segment? Segment;
    }

}