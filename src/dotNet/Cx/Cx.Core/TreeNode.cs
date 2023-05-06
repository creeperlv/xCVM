namespace Cx.Core
{
    [Serializable]
    public class TreeNode
    {
        public TreeNode? Parent;
        public List<TreeNode> Children = new List<TreeNode>();
        public void AddChild(TreeNode node)
        {
            Children.Add(node);
            node.Parent = this;
        }
    }

}