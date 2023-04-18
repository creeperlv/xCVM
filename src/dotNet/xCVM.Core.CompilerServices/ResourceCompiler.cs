using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using xCVM.Core.Utilities;

namespace xCVM.Core.CompilerServices
{

    public class ResourceCompiler
    {
        public CompileResult<ResourceDevDef> CompileDevDefinition(ResourceCompilerOptions options, params FileInfo[] MapFiles)
        {
            ResourceDevDef DevDef = new ResourceDevDef();
            var result = new CompileResult<ResourceDevDef>(DevDef);
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            foreach (var item in MapFiles)
            {
                var compileResult = ResourceDictionary.FromTextReader(item.OpenText(), item.Directory, item.Name);
                if (compileResult.Errors.Count > 0)
                {
                    result.Errors.ConnectAfterEnd(compileResult.Errors);
                    break;
                }
                resourceDictionary.Merge(compileResult.Result, ResourceDictionary.DefaultMerger);
            }
            int ID = 0;
            foreach (var item in resourceDictionary.Name_File_Mapping)
            {
                DevDef.Mapping.Add(item.Key, ID);
                ID++;
            }
            return result;
        }
        public CompileResult<ResourceCompilationResult> Compile(ResourceCompilerOptions options, params FileInfo[] MapFiles)
        {
            ResourceDevDef Definition = new ResourceDevDef();
            CompiledxCVMResource compiled_res = new CompiledxCVMResource();
            ResourceDictionary resourceDictionary = new ResourceDictionary();

            var cresult = new ResourceCompilationResult(Definition, compiled_res);
            var result = new CompileResult<ResourceCompilationResult>(cresult);
            foreach (var item in MapFiles)
            {
                var compileResult = ResourceDictionary.FromTextReader(item.OpenText(), item.Directory, item.Name);
                if (compileResult.Errors.Count > 0)
                {
                    result.Errors.ConnectAfterEnd(compileResult.Errors);
                    break;
                }
                resourceDictionary.Merge(compileResult.Result, ResourceDictionary.DefaultMerger);
            }
            ResourceDevDef resourceDevDef = new ResourceDevDef();
            cresult.Definition = resourceDevDef;
            if (options.CompileToMemory == false && options.Destination != null)
            {
                List<int> Headers = new List<int>();
                int ID = 0;
                foreach (var item in resourceDictionary.Name_File_Mapping)
                {
                    Headers.Add(ID);
                    resourceDevDef.Mapping.Add(item.Key, ID);
                    ID++;
                }
                using (var stream = File.OpenWrite(options.Destination))
                {

                    byte[] Header;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        foreach (var item in Headers)
                        {
                            memoryStream.Write(BitConverter.GetBytes(item));
                        }
                        Header = memoryStream.GetBuffer();
                    }
                    stream.WriteBytes(Header);
                    foreach (var item in resourceDictionary.Name_File_Mapping)
                    {
                        var bytes = File.ReadAllBytes(item.Value);
                        stream.WriteBytes(bytes);
                    }
                }
            }
            else
            {
                xCVMResource resource = new xCVMResource();
                int ID = 0;
                foreach (var item in resourceDictionary.Name_File_Mapping)
                {
                    var bytes = File.ReadAllBytes(item.Value);
                    resourceDevDef.Mapping.Add(item.Key, ID);
                    resource.Datas.Add(ID, bytes);
                    ID++;
                }
                result.Result.resource=new CompiledxCVMResource();
                result.Result.resource.xCVMResource = resource;
            }

            return result;
        }
    }
    [Serializable]
    public class ResourceDevDef
    {
        public Dictionary<string, int> Mapping = new Dictionary<string, int>();
        public void WriteWriter(TextWriter writer)
        {
            foreach (var item in Mapping)
            {
                writer.Write("\"");
                writer.Write(item.Key);
                writer.Write("\"");
                writer.Write(item.Value + "");
                writer.Write("\t");
            }
        }
        public void WriteToFile(string path)
        {
            using (var fs = File.OpenWrite(path))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    WriteWriter(sw);
                }
            }
        }
        public static CompileResult<ResourceDevDef> FromStream(TextReader stream, string ID)
        {
            ResourceDevDef def = new ResourceDevDef();
            CompileResult<ResourceDevDef> result = new CompileResult<ResourceDevDef>(def);
            ResourceManifestParser parser = new ResourceManifestParser();
            SegmentContext context = new SegmentContext(parser.Parse(stream.ReadToEnd(), false, ID));
            while (true)
            {
                if (context.ReachEnd)
                {
                    break;
                }
                if (context.Current == null) break;
                if (context.Current.content == "")
                {
                    context.GoNext();
                    continue;
                }
                var K = context.Current.content;
                context.GoNext();
                var V = context.Current.content;
                if (int.TryParse(V, out var value))
                {
                    def.Mapping.Add(K, value);
                    context.GoNext();
                }
                else
                {
                    result.AddError(new IntParseError(context.Current));
                    break;
                }

            }
            return result;
        }
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
