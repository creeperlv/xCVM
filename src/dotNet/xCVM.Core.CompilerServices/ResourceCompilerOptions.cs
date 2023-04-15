using System;

namespace xCVM.Core.CompilerServices
{
    [Serializable]
    public class ResourceCompilerOptions
    {
        public bool CompileToMemory = false;
        public string? Destination;
    }
}
