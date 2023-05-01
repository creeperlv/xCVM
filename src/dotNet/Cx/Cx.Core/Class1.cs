namespace Cx.Core
{
    public class Analyzer
    {

    }
    [Serializable]
    public class Token
    {
        public int TokenType;
    }
    public enum ASTNodeType
    {
        Declare,Assign,Experssion,BinaryExpression,UnaryExpression
    }
    [Serializable]
    public class TreeNode
    {
        public TreeNode? Parent;
        public List<TreeNode> Children = new List<TreeNode>();
    }
    public class ASTNode : TreeNode {
        public ASTNodeType Type;
    }

}