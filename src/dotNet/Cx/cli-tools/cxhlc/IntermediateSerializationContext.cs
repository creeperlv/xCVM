using Cx.Core.VCParser;
using LibCLCC.NET.TextProcessing;
using System.Text.Json.Serialization;

namespace cxhlc
{
    [JsonSourceGenerationOptions(WriteIndented = true ,
        GenerationMode = JsonSourceGenerationMode.Metadata ,
        IncludeFields = true)]
    [JsonSerializable(typeof(ASTNode))]
    [JsonSerializable(typeof(Segment))]
    internal partial class IntermediateSerializationContext : JsonSerializerContext
    { }
}