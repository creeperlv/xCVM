using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xCVM.Core.CompilerServices
{

    public class ResourceCompiler
    {
        public CompileResult<ResourceCompilationResult> Compile(ResourceCompilerOptions options, params FileInfo[] MapFiles)
        {
            ResourceDevDef Definition = new ResourceDevDef();
            CompiledxCVMResource compiled_res = new CompiledxCVMResource();
            ResourceDictionary resourceDictionary = new ResourceDictionary();

            var cresult = new ResourceCompilationResult(Definition, compiled_res);
            var result = new CompileResult<ResourceCompilationResult>(cresult);
            foreach (var item in MapFiles)
            {
                var compileResult = ResourceDictionary.FromTextReader(item.OpenText(), item.Name);
                if (compileResult.Errors.Count > 0)
                {
                    result.Errors.ConnectAfterEnd(compileResult.Errors);
                    break;
                }
                resourceDictionary.Merge(compileResult.Result, ResourceDictionary.DefaultMerger);
            }

            return result;
        }
    }
    [Serializable]
    public class ResourceDevDef
    {
        public Dictionary<string, int> Mapping = new Dictionary<string, int>();
    }
    public class ResourceManifestParser : GeneralPurposeParser
    {
        public ResourceManifestParser()
        {
            PredefinedSegmentCharacters.Add('=');
            lineCommentIdentifiers.Add(new LineCommentIdentifier { StartSequence = "#" });
            lineCommentIdentifiers.Add(new LineCommentIdentifier { StartSequence = "//" });
            lineCommentIdentifiers.Add(new LineCommentIdentifier { StartSequence = ";" });
        }

    }
}
