using Cx.Core.CodeGen;
using Cx.Core.VCParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.CodeGen
{
    public class VCTypeGenerator : CodeGenerator
    {
        public override OperationResult<bool> Write(GeneratorProvider provider , ASTNode node , StreamWriter writer)
        {
            writer.Write(node.Segment);
            return true;
        }
    }
    public class VCFunctionGenerator : CodeGenerator
    {
        public override OperationResult<bool> Write(GeneratorProvider provider , ASTNode node , StreamWriter writer)
        {
            var children = node.Children;
            if (children [ 0 ] is ASTNode c0)
            {
                var g0 = provider.GetGenerator(c0.Type);
                if (g0 == null)
                {
                    OperationResult<bool> operationResult = new OperationResult<bool>(false);
                    operationResult.AddError(new GeneratorNotFound(c0.Segment));
                    return operationResult;
                }
                g0.Write(provider , c0 , writer);
                writer.Write(" ");
                writer.Write(node.Segment!.content);
                writer.Write("(");
                if (children [ 1 ] is ASTNode c1)
                {
                    var count = c1.Children.Count;
                    for (int i = 0 ; i < count ; i++)
                    {
                        var item = c1.Children [ i ] as ASTNode;
                        var g = provider.GetGenerator(item.Type);
                        if (g == null)
                        {
                            OperationResult<bool> operationResult = new OperationResult<bool>(false);
                            operationResult.AddError(new GeneratorNotFound(item.Segment));
                            return operationResult;
                        }
                        var result = g.Write(provider , item , writer);
                        if (result.Errors.Count > 0) return result;
                        if (i != count - 1)
                        {
                            writer.Write(",");
                        }
                    }
                }
                {

                    if (children [ 2 ] is ASTNode item)
                    {
                        var g = provider.GetGenerator(item.Type);
                        if (g == null)
                        {
                            OperationResult<bool> operationResult = new OperationResult<bool>(false);
                            operationResult.AddError(new GeneratorNotFound(item.Segment));
                            return operationResult;
                        }
                        var result = g.Write(provider , item , writer);
                        if (result.Errors.Count > 0) return result;
                    }
                }
            }
            return true;
        }
    }
    public class VCRootGenerator : CodeGenerator
    {
        public override OperationResult<bool> Write(GeneratorProvider provider , ASTNode node , StreamWriter writer)
        {
            foreach (ASTNode child in node.Children)
            {
                var g = provider.GetGenerator(child.Type);
                var r = g.Write(provider , child , writer);
                if ((r.Errors.Count > 0))
                {
                    return r;
                }
                writer.WriteLine();
            }
            return true;
        }
    }
}
