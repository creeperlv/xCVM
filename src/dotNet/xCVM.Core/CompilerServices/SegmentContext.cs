using LibCLCC.NET.TextProcessing;

namespace xCVM.Core.CompilerServices
{
    public class SegmentContext
    {
        private Segment? Last = null;
        public Segment? Current;
        public bool GoBack()
        {
            if (Last == null) return false;
            Current = Last;
            Last = Current.Prev;
            return true;
        }
        public bool GoNext()
        {
            if (Current == null) return false;
            Current = Current.Next;
            return Current != null;
        }
    }
}
