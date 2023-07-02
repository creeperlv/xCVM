using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cx.Core
{
    [Serializable]
    public class TreeNode
    {
        public int Type;
        public Segment? Segment;
        [NonSerialized]
        public TreeNode? Parent;
        public List<TreeNode> Children = new List<TreeNode>();
        public void ReplaceChild(TreeNode OriginalChild , TreeNode NewNode)
        {
            var ch = Children.IndexOf(OriginalChild);
            NewNode.Parent = this;
            foreach (var item in OriginalChild.Children)
            {
                NewNode.AddChild(item);
            }
            Children [ ch ] = NewNode;
            OriginalChild.Children.Clear();
            OriginalChild.Parent = null;
        }
        public void AddChild(TreeNode node)
        {
            Children.Add(node);
            node.Parent = this;
        }
    }

}