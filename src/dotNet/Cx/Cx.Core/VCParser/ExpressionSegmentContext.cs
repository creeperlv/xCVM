using System;

namespace Cx.Core.VCParser
{
    public enum ExpressionSegmentType
    {
        PlainContent, TreeNode, ESTreeNode, Closure, NULL
    }
    public enum ESCMatchResult
    {
        Match, MismatchContent, MismatchType, ReachEnd
    }
    public class ExpressionSegmentContext
    {
        public ExpressionSegment HEAD;
        private ExpressionSegment? _Current;
        public ExpressionSegment? Current { get => _Current; }
        private ExpressionSegment? _ep = null;
        public ExpressionSegment? EndPoint { get => _ep; }
        public ExpressionSegment? Next;
        public ExpressionSegment? Prev;
        public void GoNext()
        {
            Prev = Current;
            _Current = _Current?.Next;
            Next = Current?.Next;
        }
        public void GoBack()
        {
            _Current = Prev;
            Prev = Current?.Prev;
            Next = Current?.Next;
        }
        public ExpressionSegmentType CurrentType
        {
            get
            {
                if (Current != null) return Current.SegmentType;
                return ExpressionSegmentType.NULL;
            }
        }
        public ESCMatchResult Match(string content)
        {
            switch (CurrentType)
            {
                case ExpressionSegmentType.PlainContent:
                    if (Current?.Segment?.content == content) return ESCMatchResult.Match;
                    return ESCMatchResult.MismatchContent;
                case ExpressionSegmentType.TreeNode:
                case ExpressionSegmentType.Closure:
                    return ESCMatchResult.MismatchType;
                default:
                    return ESCMatchResult.ReachEnd;

            }
        }
        public ESCMatchResult MatchCollection(params string [ ] contents)
        {
            switch (CurrentType)
            {
                case ExpressionSegmentType.PlainContent:
                    foreach (var content in contents)
                    {
                        if (Current?.Segment?.content == content) return ESCMatchResult.Match;
                    }
                    return ESCMatchResult.MismatchContent;
                case ExpressionSegmentType.TreeNode:
                case ExpressionSegmentType.Closure:
                    return ESCMatchResult.MismatchType;
                default:
                    return ESCMatchResult.ReachEnd;

            }
        }
        public (ESCMatchResult, string?) MatchCollectionWithMatchItem(params string [ ] contents)
        {
            switch (CurrentType)
            {
                case ExpressionSegmentType.PlainContent:
                    foreach (var content in contents)
                    {
                        if (Current?.Segment?.content == content) return (ESCMatchResult.Match, content);
                    }
                    return (ESCMatchResult.MismatchContent, null);
                case ExpressionSegmentType.TreeNode:
                case ExpressionSegmentType.Closure:
                    return (ESCMatchResult.MismatchType, null);
                default:
                    return (ESCMatchResult.ReachEnd, null);

            }
        }
        public void SetCurrent(ExpressionSegment? current)
        {
            _Current = current;
        }
        public void SetEndPoint(ExpressionSegment? expressionSegment)
        {
            _ep = expressionSegment;
        }
        public bool IsReachEnd { get { return Current == EndPoint || Current?.Next == null; } }

        public int Count
        {
            get
            {
                int _Count = 0;
                var current = HEAD;
                while (true)
                {
                    if (current == EndPoint || current?.Next == null)
                    {
                        break;
                    }
                    current=current.Next;
                    _Count++;
                }
                return _Count;
            }
        }

        public bool SubstituteSegment_1(ExpressionSegment? L , ExpressionSegment? R)
        {
            if (L == null && R == null) return false;
            if (L?.Next == R) return false;
            if (L == null)
            {
                if (R == null) return false;
                if (R.Next == null) return false;
                HEAD = R.Next;
                return true;
            }
            if (L.Prev == null)
            {
                if (R == null)
                {
                    return false;
                }
                if (R.Next == null) return false;
                HEAD = R.Next;
                return true;
            }
            if (R == null)
            {
                L.Prev.Next = null;
                return true;
            }
            if (R.Next == null)
            {
                L.Prev.Next = null;
                return true;
            }
            L.Prev.Next = R.Next;
            R.Next.Prev = L.Prev;
            return true;
        }
        public bool IsInside(ExpressionSegment? L, ExpressionSegment? R, ExpressionSegment? Item)
        {
            ExpressionSegment? Start = HEAD;
            if (L != null)
                while (true)
                {
                    if (Start == L) break;
                    if (Start == null) return false;
                    Start = Start.Next;
                }
            ExpressionSegment? Current = Start;
            while (true)
            {
                if (Current == R) break;
                if (Current == null) break;
                if (Current == Item) return true;
                Current = Current.Next;
            }
            return false;
        }
        /// <summary>
        /// Exclusive Substitute, LeftEnd and RightEnd will be preserved.
        /// </summary>
        /// <param name="LeftEnd"></param>
        /// <param name="RightEnd"></param>
        /// <param name="NewSegment"></param>
        /// <returns></returns>
        public bool SubstituteSegment_0(ExpressionSegment? LeftEnd , ExpressionSegment? RightEnd , ExpressionSegment NewSegment)
        {
            var segment_l_end = LeftEnd;
            var segment_r_end = RightEnd;
            if (IsInside(LeftEnd , RightEnd , Current))
            {
                _Current = NewSegment;
            }
            if (segment_l_end == null)
            {
#if DEBUG
                Console.WriteLine("Replaced HEAD.");
#endif
                this.HEAD = NewSegment;
            }
            else
            {
#if DEBUG
                Console.WriteLine($"\tSub0:{segment_l_end.GetString()}");
#endif
                segment_l_end.Next = NewSegment;
                NewSegment.Prev = segment_l_end;
            }
            if (segment_r_end != null)
            {
                var end = NewSegment.GetEnd();
#if DEBUG
                Console.WriteLine($"{end.GetString()} appended with:{segment_r_end.GetString()}");
#endif
                end.Next = segment_r_end;
                segment_r_end.Prev = end;
            }
            else
            {
                if (NewSegment.Next == null)
                {
                    ExpressionSegment Empty = new ExpressionSegment();
                    NewSegment.AttachNext(Empty);
                }
                SetEndPoint(NewSegment.GetEnd().Next);
            }
            return true;
        }
        public ExpressionSegmentContext? SubContext(ExpressionSegment? L , ExpressionSegment? R)
        {
            ExpressionSegment? Start = HEAD;
            if (L != null)
                while (true)
                {
                    if (Start == L) break;
                    if (Start == null) return null;
                    Start = Start.Next;
                }
            ExpressionSegment? Current = Start;
            ExpressionSegment? _Current = null;
            ExpressionSegment? _HEAD = null;
            while (true)
            {
                if (Current == R) break;
                if (Current == null) break;
                if (_Current == null || _HEAD == null)
                {
                    _Current = Start.Duplicate();
                    _HEAD = _Current;
                }
                else
                {
                    _Current.AttachNext(Current.Duplicate());
                    _Current = _Current.Next;
                }
                Current = Current.Next;
            }
            if (_HEAD == null) return null;
            if (_Current != null)
            {
                ExpressionSegment EndPoint = new ExpressionSegment();
                _Current.AttachNext(EndPoint);
            }
            return new ExpressionSegmentContext(_HEAD);

        }
        public ExpressionSegmentContext(ExpressionSegment head)
        {
            HEAD = head;
            _Current = head;
        }
    }
}