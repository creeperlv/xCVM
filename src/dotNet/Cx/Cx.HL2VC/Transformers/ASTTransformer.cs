using Cx.Core.VCParser;
using System;
using System.Collections.Generic;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Transformers
{
    public class ASTReferences
    {
        public List<ASTNode> nodes=new List<ASTNode>();
    }
    public class ASTTransformer
    {
        public ASTReferences ASTReferences=new ASTReferences();
        public ASTNode? RootNode;
        public virtual void Init()
        {

        }
        public virtual OperationResult<ASTNode?> Transform(ASTNode node, ASTTransformer? ParentTransformer) {
            return new OperationResult<ASTNode?>(null);
        }
    }
}
