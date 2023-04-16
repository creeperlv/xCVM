using LibCLCC.NET.TextProcessing;

namespace xCVM.Core.CompilerServices
{
    public class ASMParser : GeneralPurposeParser
    {
        public ASMParser()
        {
            PredefinedSegmentCharacters.Add(';');
            PredefinedSegmentCharacters.Add(':');
            lineCommentIdentifiers.Add(new LineCommentIdentifier { StartSequence = "#" });
            lineCommentIdentifiers.Add(new LineCommentIdentifier { StartSequence = "//" });
        }
    }
}
