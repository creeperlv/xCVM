using System;
using System.Collections.Generic;
using System.IO;

namespace xCVM.Core.CompilerServices
{
    [Serializable]
    public class ResourceDictionary
    {
        public Dictionary<string, string> Name_File_Mapping = new Dictionary<string, string>();
        /// <summary>
        /// Overwrite.
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="Conflict"></param>
        public static void DefaultMerger(Dictionary<string, string> Target, KeyValuePair<string, string> Conflict)
        {
            Target[Conflict.Key] = Conflict.Value;
        }
        public void Merge(ResourceDictionary R, Action<Dictionary<string, string>, KeyValuePair<string, string>> Merger)
        {
            foreach (var item in R.Name_File_Mapping)
            {
                if (!Name_File_Mapping.ContainsKey(item.Key))
                {
                    Name_File_Mapping.Add(item.Key, item.Value);
                }
                else
                {
                    Merger(Name_File_Mapping, item);
                }
            }
        }
        public static CompileResult<ResourceDictionary> FromTextReader(TextReader reader, DirectoryInfo Parent, string? ID = null)
        {
            ResourceManifestParser parser = new ResourceManifestParser();
            var content = reader.ReadToEnd();
            var segments = parser.Parse(content, false, ID);
            SegmentContext context = new SegmentContext(segments);
            var result = Parse(context,Parent);
            return result;
        }
        static CompileResult<ResourceDictionary> Parse(SegmentContext context, DirectoryInfo Parent)
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            CompileResult<ResourceDictionary> result = new CompileResult<ResourceDictionary>(dictionary);
            while (true)
            {
                if (context.ReachEnd)
                {
                    break;
                }
                if (context.Current == null)
                {
                    break;
                }
                var Key = context.Current.content;
                context.GoNext();
                var match_result = context.MatachNext("=");
                if (match_result == MatchResult.Match)
                {
                    var Value = context.Current.content;
                    dictionary.Name_File_Mapping.Add(Key, Path.Combine(Parent.FullName, Value));
                }
                else if (match_result == MatchResult.Mismatch)
                {
                    result.AddError(new ExpectAMarkError(context.Last, "="));
                    break;
                }
                else if (match_result == MatchResult.ReachEnd)
                {
                    result.AddError(new UnexpectedEndOfFileError(context.Current));
                }
            }
            return result;
        }
    }
}
