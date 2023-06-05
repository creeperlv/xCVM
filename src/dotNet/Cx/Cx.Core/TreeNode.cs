using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;

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
        public void AddChild(TreeNode node)
        {
#if DEBUG
            Console.WriteLine("Added node.");
#endif
            Children.Add(node);
            node.Parent = this;
        }
    }

}