using System.Text.Json.Serialization;
using xCVM.Core;
using xCVM.Core.CompilerServices;

namespace xCVM.Compiler
{
    [JsonSourceGenerationOptions(WriteIndented = true ,
        GenerationMode = JsonSourceGenerationMode.Metadata , 
        IncludeFields = true)]
    [JsonSerializable(typeof(AssemblerDefinition))]
    [JsonSerializable(typeof(xCVMModule))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}