using System.Collections.Generic;
using Cx.Core;

namespace Cx.Preprocessor
{
    public class Preprocessed
    {
        public Dictionary<string , VirtualFile> ProcessedHeader = new Dictionary<string , VirtualFile>();
        public VirtualFile? CombinedHeader;
        public VirtualFile? ProcessedCFile;
    }
}
