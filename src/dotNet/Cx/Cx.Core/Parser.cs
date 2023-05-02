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
            OperationResult<ASTNode> result=new OperationResult<ASTNode>(root);
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
    [Serializable]
    public class TreeNode
    {
        public TreeNode? Parent;
        public List<TreeNode> Children = new List<TreeNode>();
    }
    public class ASTNode : TreeNode
    {
        public ASTNodeType Type;
        public Segment? Segment;
    }

}