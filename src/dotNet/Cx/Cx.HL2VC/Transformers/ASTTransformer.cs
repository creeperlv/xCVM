using Cx.Core;
using System;
using System.Collections.Generic;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Transformers
{
    public class ASTReferences
    {
        public List<TreeNode> nodes=new List<TreeNode>();
    }
    public class ASTTransformer
    {
        public ASTReferences ASTReferences=new ASTReferences();
        public TreeNode? RootNode;
        Dictionary<string , string> Usings = new Dictionary<string , string>();
        public virtual void Init()
        {

        }
        public void FindUsing(TreeNode root)
        {
            foreach (var item in root.Children)
            {
                if (item.Type == HLASTNodeType.Using)
                {
                    Usings.Add(item.Segment?.ID??"Default" , item.Segment?.content??"");
                }
            }
        }
        public virtual OperationResult<TreeNode?> Transform(TreeNode node, ASTTransformer? ParentTransformer) {
            return new OperationResult<TreeNode?>(null);
        }
    }
}
