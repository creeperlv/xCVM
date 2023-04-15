using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xCVM.Core.CompilerServices
{

    public class ResourceCompiler
    {
        public ResourceCompilationResult Compile(ResourceCompilerOptions options,params FileInfo[] MapFiles)
        {
            ResourceDevDef Definition = new ResourceDevDef();
            CompiledxCVMResource compiled_res = new CompiledxCVMResource();

            return new ResourceCompilationResult(Definition, compiled_res);
        }
    }
    [Serializable]
    public class ResourceDevDef
    {
        public Dictionary<string, int> Mapping = new Dictionary<string, int>();
    }
    [Serializable]
    public class ResourceDictionary
    {
        public Dictionary<string, string> Name_File_Mapping = new Dictionary<string, string>();
    }
}
