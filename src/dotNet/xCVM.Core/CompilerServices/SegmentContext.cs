﻿using LibCLCC.NET.TextProcessing;
using static System.Net.Mime.MediaTypeNames;

namespace xCVM.Core.CompilerServices
{
    public class SegmentContext
    {
        private Segment? _Last = null;
        private Segment? _Current;

        public SegmentContext(Segment? current)
        {
            _Current = current;
        }

        public Segment? Current => _Current;
        public Segment? Last=> _Last;
        public (MatchResult, string?) MatchCollectionMarchReturnName(params string[] matches)
        {
            if (Current == null) return (MatchResult.ReachEnd, null);
            for (int i = 0; i < matches.Length; i++)
            {
                if (Current.content == matches[i])
                {
                    if (Current.Next != null)
                    {
                        GoNext();
                        return (MatchResult.Match, matches[i]);
                    }
                }
            }
            return (MatchResult.Mismatch, null);
        }
        public (MatchResult, int) MatchCollectionMarch(params string[] matches)
        {
            if (Current == null) return (MatchResult.ReachEnd, -1);
            for (int i = 0; i < matches.Length; i++)
            {
                if (Current.content == matches[i])
                {
                    if (Current.Next != null)
                    {
                        GoNext();
                        return (MatchResult.Match, i);
                    }
                }
            }
            return (MatchResult.Mismatch, -1);
        }
        public (MatchResult, string?) MatchCollectionMarchWithMatchNextReturnName(string Next, params string[] matches)
        {
            if (Current == null) return (MatchResult.ReachEnd, null);
            for (int i = 0; i < matches.Length; i++)
            {
                if (Current.content == matches[i])
                {
                    if (Current.Next != null)
                    {
                        if (Current.Next.content == Next)
                        {
                            GoNext();
                            GoNext();
                            return (MatchResult.Match, matches[i]);
                        }
                    }
                }
            }
            return (MatchResult.Mismatch, null);
        }
        public (MatchResult, int) MatchCollectionMarchWithMatchNext(string Next, params string[] matches)
        {
            if (Current == null) return (MatchResult.ReachEnd, -1);
            for (int i = 0; i < matches.Length; i++)
            {
                if (Current.content == matches[i])
                {
                    if (Current.Next != null)
                    {
                        if (Current.Next.content == Next)
                        {
                            GoNext();
                            GoNext();
                            return (MatchResult.Match, i);
                        }
                    }
                }
            }
            return (MatchResult.Mismatch, -1);
        }
        public MatchResult MatchMarch(string Name, bool WillGoNext = true)
        {
            if (Current != null)
            {
                if (Current.content == Name)
                {
                    if (WillGoNext)
                        GoNext();
                    return MatchResult.Match;

                }
                else
                {
                    return MatchResult.Mismatch;
                }
            }
            return MatchResult.ReachEnd;
        }
        public MatchResult MatachNext(string Name, bool WillGoBack = true)
        {
            if (GoNext())
            {
                if (Current == null) { return MatchResult.ReachEnd; }
                if (Current.content == Name)
                {
                    return MatchResult.Match;
                }
                else
                {
                    if (WillGoBack) GoBack();
                    return MatchResult.Mismatch;
                }
            }
            else
            {
                return MatchResult.ReachEnd;
            }
        }
        public bool GoBack()
        {
            if (_Last == null) return false;
            _Current = _Last;
            _Last = _Current.Prev;
            return true;
        }
        public bool GoNext()
        {
            if (_Current == null) return false;
            _Current = _Current.Next;
            return _Current != null;
        }
        public bool ReachEnd=>_Current==null;
    }
    public enum MatchResult
    {
        ReachEnd, Match, Mismatch
    }
}