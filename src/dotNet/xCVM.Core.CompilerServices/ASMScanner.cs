using LibCLCC.NET.TextProcessing;

namespace xCVM.Core.CompilerServices
{
    public class ASMScanner : GeneralPurposeScanner
    {
        public ASMScanner()
        {
            PredefinedSegmentCharacters.Add(';');
            PredefinedSegmentCharacters.Add(':');
            lineCommentIdentifiers.Add(new LineCommentIdentifier { StartSequence = "#" });
            lineCommentIdentifiers.Add(new LineCommentIdentifier { StartSequence = "//" });
        }
    }
}
