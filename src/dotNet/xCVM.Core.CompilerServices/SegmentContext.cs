﻿using LibCLCC.NET.TextProcessing;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace xCVM.Core.CompilerServices
{
    public interface IContentable
    {
        string Content { get; }
    }
    public class SegmentContext
    {
        private Segment? _Last = null;
        private Segment? _Current;
        private Segment? _EndPoint = null;
        public Segment? EndPoint { get => _EndPoint; }
        public void SetEndPoint(Segment? End)
        {
            _EndPoint = End;
        }
        public SegmentContext(Segment? current)
        {
            _head = current;
            _Current = current;
        }
        private Segment? _head = null;
        public Segment? HEAD => _head;
        public void SetHead(Segment? segment)
        {
            _head = segment;
        }
        /// <summary>
        /// Reset the current to the head.
        /// </summary>
        public void ResetCurrent()
        {
            _Current = HEAD;
        }
        public Segment? Current => _Current;
        /// <summary>
        /// Use with care.
        /// </summary>
        /// <param name="current"></param>
        public void SetCurrent(Segment? current)
        {
            _Current = current;
            _Last = current?.Prev;
        }
        public Segment? Last => _Last;
        public (MatchResult, IContentable?) MatchCollectionMarchReturnContentable(IEnumerable<IContentable> matches , bool CaseSensitive = true)
        {
            if (CaseSensitive)
            {
                if (Current == null) return (MatchResult.ReachEnd, null);
                foreach (var item in matches)
                {
                    if (Current.content == item.Content)
                    {
                        return (MatchResult.Match, item);
                    }
                }
                return (MatchResult.Mismatch, null);
            }
            else
            {
                if (Current == null) return (MatchResult.ReachEnd, null);
                var concern = Current.content.ToUpper();
                foreach (var item in matches)
                {
                    if (concern == item.Content.ToUpper())
                    {
                        return (MatchResult.Match, item);
                    }
                }
                return (MatchResult.Mismatch, null);
            }

        }
        public (MatchResult, string?) MatchCollectionMarchReturnName(params string [ ] matches)
        {
            if (Current == null) return (MatchResult.ReachEnd, null);
            for (int i = 0 ; i < matches.Length ; i++)
            {
                if (Current.content == matches [ i ])
                {
                    if (Current.Next != null)
                    {
                        GoNext();
                    }
                    return (MatchResult.Match, matches [ i ]);
                }
            }
            return (MatchResult.Mismatch, null);
        }
        public (MatchResult, string?) MatchCollectionReturnName(params string [ ] matches)
        {
            if (Current == null) return (MatchResult.ReachEnd, null);
            for (int i = 0 ; i < matches.Length ; i++)
            {
                if (Current.content == matches [ i ])
                {
                    return (MatchResult.Match, matches [ i ]);
                }
            }
            return (MatchResult.Mismatch, null);
        }
        public (MatchResult, int) MatchCollection(bool CaseSensitive = true , params string [ ] matches)
        {

            if (Current == null) return (MatchResult.ReachEnd, -1);
            if (CaseSensitive)
            {

                for (int i = 0 ; i < matches.Length ; i++)
                {
                    if (Current.content == matches [ i ])
                    {
                        return (MatchResult.Match, i);
                    }
                }
            }
            else
            {
                var curr = Current.content.ToUpper();
                for (int i = 0 ; i < matches.Length ; i++)
                {
                    if (curr == matches [ i ].ToUpper())
                    {
                        return (MatchResult.Match, i);
                    }
                }
                return (MatchResult.Mismatch, -1);
            }
            return (MatchResult.Mismatch, -1);
        }
        public (MatchResult, int) MatchCollectionMarch(bool CaseSensitive = true , params string [ ] matches)
        {

            if (Current == null) return (MatchResult.ReachEnd, -1);
            if (CaseSensitive)
            {

                for (int i = 0 ; i < matches.Length ; i++)
                {
                    if (Current.content == matches [ i ])
                    {
                        if (Current.Next != null)
                        {
                            GoNext();
                        }
                        return (MatchResult.Match, i);
                    }
                }
            }
            else
            {
                var curr = Current.content.ToUpper();
                for (int i = 0 ; i < matches.Length ; i++)
                {
                    if (curr == matches [ i ].ToUpper())
                    {
                        if (Current.Next != null)
                        {
                            GoNext();
                        }
                        return (MatchResult.Match, i);
                    }
                }
                return (MatchResult.Mismatch, -1);
            }
            return (MatchResult.Mismatch, -1);
        }
        public (MatchResult, int) MatchCollectionMarch(params string [ ] matches)
        {
            if (Current == null) return (MatchResult.ReachEnd, -1);
            for (int i = 0 ; i < matches.Length ; i++)
            {
                if (Current.content == matches [ i ])
                {
                    if (Current.Next != null)
                    {
                        GoNext();
                    }
                    return (MatchResult.Match, i);
                }
            }
            return (MatchResult.Mismatch, -1);
        }
        public (MatchResult, string?) MatchCollectionMarchWithMatchNextReturnName(string Next , params string [ ] matches)
        {
            if (Current == null) return (MatchResult.ReachEnd, null);
            for (int i = 0 ; i < matches.Length ; i++)
            {
                if (Current.content == matches [ i ])
                {
                    if (Current.Next != null)
                    {
                        if (Current.Next.content == Next)
                        {
                            GoNext();
                            GoNext();
                            return (MatchResult.Match, matches [ i ]);
                        }
                    }
                }
            }
            return (MatchResult.Mismatch, null);
        }
        public (MatchResult, int) MatchCollectionMarchWithMatchNext(string Next , bool CaseSensitive = true , params string [ ] matches)
        {
            if (CaseSensitive)
            {
                if (Current == null) return (MatchResult.ReachEnd, -1);
                for (int i = 0 ; i < matches.Length ; i++)
                {
                    if (Current.content == matches [ i ])
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
            else
            {
                if (Current == null) return (MatchResult.ReachEnd, -1);
                var focus = Current.content.ToUpper();
                for (int i = 0 ; i < matches.Length ; i++)
                {
                    if (focus == matches [ i ].ToUpper())
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

        }
        public MatchResult MatchMarch(string Name , bool WillGoNext = true , bool CaseSensitive = true)
        {
            if (CaseSensitive)
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
            else
            {
                if (Current != null)
                {
                    if (Current.content.ToUpper() == Name.ToUpper())
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

        }
        public MatchResult MatchNext(string Name , bool WillGoBack = true)
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
        public MatchResult Match(IContentable content)
        {
            if (ReachEnd) return MatchResult.ReachEnd;
            if (Current!.content == content.Content)
            {
                return MatchResult.Match;
            }
            return MatchResult.Mismatch;
        }
        public MatchResult Match(string content)
        {
            if (ReachEnd) return MatchResult.ReachEnd;
            if (Current!.content == content)
            {
                return MatchResult.Match;
            }
            return MatchResult.Mismatch;
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
            _Last = _Current;
            _Current = _Current.Next;
            return _Current != null;
        }
        /// <summary>
        /// If it is **currently** end.
        /// </summary>
        public bool ReachEnd
        {
            get
            {
                if (_EndPoint == _Current) return true;
                if (_Current == null) return true;
                return _Current.content == "" && _Current.Next == null;
            }
        }
        public string FormTillEnd(bool IncludeCurrent , char sep)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Segment? Current;
            if (IncludeCurrent)
            {
                Current = this.Current;
            }
            else
            {
                Current = this.Current?.Next ?? null;
            }
            while (true)
            {
                if (Current == null) break;
                //if (Current.Next == null) break;
                if (Current.isEncapsulated)
                {
                    stringBuilder.Append(Current.EncapsulationIdentifier);
                    stringBuilder.Append(Current.content);
                    stringBuilder.Append(Current.EncapsulationIdentifier);
                }
                else
                    stringBuilder.Append(Current.content);
                if (sep == 0)
                    stringBuilder.Append(sep);
                Current = Current.Next;
            }
            return stringBuilder.ToString();
        }
    }
    public enum MatchResult
    {
        ReachEnd, Match, Mismatch
    }
}
