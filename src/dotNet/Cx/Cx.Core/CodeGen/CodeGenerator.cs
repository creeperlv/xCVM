using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.Core.CodeGen
{
    public class CodeGenerator
    {
        public virtual OperationResult<bool> Write(GeneratorProvider provider, TreeNode node , StreamWriter writer)
        {
            return new OperationResult<bool>(false);
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
