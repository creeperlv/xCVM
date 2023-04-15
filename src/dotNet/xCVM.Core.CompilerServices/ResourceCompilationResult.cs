namespace xCVM.Core.CompilerServices
{
    public class ResourceCompilationResult
    {
        public ResourceDevDef Definition;
        public CompiledxCVMResource? resource;

        public ResourceCompilationResult(ResourceDevDef definition, CompiledxCVMResource? resource)
        {
            Definition = definition;
            this.resource = resource;
        }
    }
}
