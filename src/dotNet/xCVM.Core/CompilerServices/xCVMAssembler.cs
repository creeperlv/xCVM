using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xCVM.Core.CompilerServices
{
    public class xCVMAssembler
    {
        public xCVMAssembler() { }
        public AssembleResult Assemble(FileInfo file)
        {
            using var sr = file.OpenText();
            var content = sr.ReadToEnd();
            return Assemble(content);
        }
        public AssembleResult Assemble(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                return Assemble(sr.ReadToEnd());
            }
        }
        ASMParser parser = new ASMParser();
        public AssembleResult Assemble(Segment segments)
        {
            xCVMModule module = new xCVMModule();
            AssembleResult assembleResult = new AssembleResult(module);
            var current = segments;
            SegmentContext context = new SegmentContext(current);
            int Sections = 0;
            while (true)
            {

                if (context.Current == null) break;
                {
                    (var mr, var section) = context.MatchCollectionMarchWithMatchNext(":", ".module", ".text", ".ids", ".codes");
                    if (mr == MatchResult.Match)
                    {
                        Sections = section;
                    }
                    else if (mr == MatchResult.Mismatch)
                    {
                        switch (Sections)
                        {
                            case 0:
                                {
                                    (var _mr, var _selection) = context.MatchCollectionMarchReturnName("ModuleName", "Author", "Copyright", "ModuleVersion", "TargetVersion");
                                    if (_mr == MatchResult.Match)
                                    {
                                        switch (_selection)
                                        {
                                            case "ModuleName":
                                                {
                                                    module.ModuleMetadata.ModuleName = context.Current.content;
                                                    context.GoNext();
                                                }
                                                break;
                                            case "Author":
                                                {
                                                    module.ModuleMetadata.Author = context.Current.content;
                                                    context.GoNext();
                                                }
                                                break;
                                            case "Copyright":
                                                {
                                                    module.ModuleMetadata.Copyright = context.Current.content;
                                                    context.GoNext();
                                                }
                                                break;
                                            case "ModuleVersion":
                                                {
                                                    if (Version.TryParse(context.Current.content, out var result))
                                                    {
                                                        module.ModuleMetadata.ModuleVersion = result;
                                                    }
                                                    else assembleResult.AddError(new VersionFormatError(context.Current));
                                                    context.GoNext();
                                                }
                                                break;
                                            case "TargetVersion":
                                                {
                                                    if (Version.TryParse(context.Current.content, out var result))
                                                    {
                                                        module.ModuleMetadata.TargetVersion = result;
                                                    }
                                                    else assembleResult.AddError(new VersionFormatError(context.Current));
                                                    context.GoNext();
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (mr == MatchResult.ReachEnd)
                    {

                    }
                }

            }
            return assembleResult;

        }
        public AssembleResult Assemble(string content)
        {
            var segments = parser.Parse(content, false);
            return Assemble(segments);
        }
    }
}
