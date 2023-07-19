using Cx.Core.Analyzer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.Core.CodeGen
{
    public class CodeGenerator
    {
        public virtual OperationResult<bool> Write(GeneratorProvider provider, AnalyzedTreeNode node , StreamWriter writer)
        {
            return new OperationResult<bool>(false);
        }
    }
    public class ToASM_Scope_Generator : CodeGenerator
    {

    }
    public class ToASM_CallGenerator : CodeGenerator
    {
		public override OperationResult<bool> Write(GeneratorProvider provider , AnalyzedTreeNode node , StreamWriter writer)
		{
            if(node.Type!= ASTNodeType.Call)
            {
                switch (node.Segment?.content??"")
                {
                    case "ASM_0":
                        {

                        }
                        break;
                    default:
                        break;
                }
            }
            return false;
		}
	}
    public class GeneratorProvider
    {

        internal Dictionary<int , CodeGenerator> generators = new Dictionary<int , CodeGenerator>();
        public void RegisterGenerator(int ID , CodeGenerator generator)
        {
            if (generators.ContainsKey(ID))
            {
                generators [ ID ] = generator;
            }
            else generators.Add(ID , generator);
        }
        public CodeGenerator? GetGenerator(int ID)
        {
            if (generators.ContainsKey(ID))
            {
                return generators [ ID ];
            }
            return null;
        }
    }
}
