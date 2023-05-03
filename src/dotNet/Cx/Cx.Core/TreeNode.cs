namespace Cx.Core
{
    [Serializable]
    public class TreeNode
    {
        public TreeNode? Parent;
        public List<TreeNode> Children = new List<TreeNode>();
    }

}