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
        public virtual void Init()
        {

        }
        public virtual OperationResult<TreeNode?> Transform(TreeNode node, ASTTransformer? ParentTransformer) {
            return new OperationResult<TreeNode?>(null);
        }
    }
}
