using LibCLCC.NET.TextProcessing;

namespace xCVM.Core
{
    public class ASMParser : GeneralPurposeParser {
        public ASMParser() {
            this.PredefinedSegmentCharacters.Add(';');
            this.PredefinedSegmentCharacters.Add(':');
        }
    }
}
