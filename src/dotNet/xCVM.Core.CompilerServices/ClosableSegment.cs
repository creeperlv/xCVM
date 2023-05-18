using LibCLCC.NET.TextProcessing;
using System.Collections.Generic;

namespace xCVM.Core.CompilerServices
{
    public class ClosableSegment : Segment
    {
        public bool isClosure;
        public List<Segment>? ClosedSegments;
        public Segment? LeftClosureIdentifer;
        public Segment? RightClosureIdentifer;
    }
}
