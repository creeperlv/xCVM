using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cx.Core;
using Cx.Core.VCParser;
using LibCLCC.NET.Delegates;
using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Preprocessor
{
    public class Preprocessor
    {
        public bool CloseSWOnProcessEnd = false;
        public bool CloseSROnProcessEnd = false;
        FilesProvider FilesProvider;
        public ChainAction<VirtualFile> OnSingleFileProcessComplete = new ChainAction<VirtualFile>();
        public Preprocessor(FilesProvider filesProvider)
        {
            FilesProvider = filesProvider;
        }
        CStyleScanner CStyleParser = new CStyleScanner();
        public Dictionary<string , string> Symbols = new Dictionary<string , string>();
        public void Define(string symbol , string value)
        {
#if DEBUG
            Console.WriteLine($"Add:{symbol},{value}");
#endif
            if (Symbols.ContainsKey(symbol)) { Symbols [ symbol ] = value; }
            else { Symbols.Add(symbol , value); }
        }
        public void define(SegmentContext context)
        {
            var key = context.Current?.content ?? "";
            var value = context.FormTillEnd(false , ' ').Trim();
            Define(key , value);
        }
        public void Undefine(SegmentContext context)
        {
            var key = context.Current?.content ?? "";
            if (Symbols.ContainsKey(key)) Symbols.Remove(key);
        }
        public OperationResult<bool> PerformClosure(SegmentContext segmentContext , string ClosureLeft , string ClosureRight)
        {
            int ClosureLeftFound = 0;
            List<Segment> Closed = new List<Segment>();
            Segment? ClosureLeftPoint = null;
            bool Hit = false;
            while (true)
            {
                if (segmentContext.ReachEnd)
                {
                    break;
                }
                {
                    var ML = segmentContext.Match(ClosureLeft);
                    if (ML == MatchResult.Match)
                    {
                        ClosureLeftFound++;
                        ClosureLeftPoint = segmentContext.Current;
                        segmentContext.GoNext();
                        continue;
                    }
                }
                {
                    var MR = segmentContext.Match(ClosureRight);
                    if (MR == MatchResult.Match)
                    {
                        ClosureLeftFound--;
                        if (ClosureLeftFound == 0)
                        {
                            ClosableSegment closableSegment = new ClosableSegment();
                            closableSegment.ClosedSegments = Closed;
                            closableSegment.LeftClosureIdentifer = ClosureLeftPoint;
                            var LL = ClosureLeftPoint!.Prev;
                            if (LL != null)
                            {
                                LL.Next = closableSegment;
                            }
                            closableSegment.Prev = LL;
                            var RR = segmentContext.Current!.Next;
                            if (RR != null)
                                RR.Prev = segmentContext.Current;
                            closableSegment.Next = RR;
                            Closed = new List<Segment>();
                            closableSegment.RightClosureIdentifer = segmentContext.Current;
                            if (ClosureLeftPoint == segmentContext.HEAD)
                            {
                                segmentContext.SetHead(closableSegment);
                            }
                            Hit = true;
                            segmentContext.GoNext();
                            continue;
                        }
                        else if (ClosureLeftFound < 0)
                        {
                            var r = new OperationResult<bool>(false);
                            r.AddError(new MisClosureError(segmentContext.Current));
                            return r;
                        }
                    }
                }
                if (ClosureLeftFound > 0)
                {
                    Closed.Add(segmentContext.Current!);
                }
                segmentContext.GoNext();
            }
            return Hit;
        }
        public OperationResult<ASTNode?> SearchUnaryExpression(SegmentContext context , params string [ ] operators)
        {

            while (true)
            {
                if (context.ReachEnd) break;
                if (context.Current is ClosableSegment)
                {
                    context.GoNext();
                }
                else
                {

                    var mr = context.MatchCollection(true , operators);
                    if (mr.Item1 == MatchResult.Match)
                    {
                        ASTNode node = new ASTNode();
                        node.Segment = context.Current;
                        node.Type = ASTNodeType.BinaryExpression;
                        if (context.Current == null)
                        {
                            var or = new OperationResult<ASTNode?>(null);
                            or.AddError(new UnexpectedEndOfFileError(context.Current));
                            return or;
                        }
                        {
                            var RC = new SegmentContext(context.Current.Next);
                            var RR = ParseEval(RC);
                            if (RR.Errors.Count == 0)
                            {
                                if (RR.Result == null)
                                {

                                }
                                else
                                {
                                    node.AddChild(RR.Result);
                                }
                            }
                            else
                            {
                                return new OperationResult<ASTNode?>(null) { Errors = RR.Errors };
                            }
                        }
                        return node;
                    }
                    else
                    {
                        context.GoNext();
                    }
                }
            }
            return new OperationResult<ASTNode?>(null);
        }
        public OperationResult<ASTNode?> SearchBinaryExpression(SegmentContext context , params string [ ] operators)
        {
            while (true)
            {
                if (context.ReachEnd) break;
                var mr = context.MatchCollection(true , operators);
                if (mr.Item1 == MatchResult.Match)
                {
                    ASTNode node = new ASTNode();
                    node.Segment = context.Current;
                    node.Type = ASTNodeType.BinaryExpression;
                    {
                        var LC = new SegmentContext(context.HEAD);
                        LC.SetEndPoint(context.Current!.Prev);
                        var LR = ParseEval(LC);
                        if (LR.Result == null)
                        {
                            OperationResult<ASTNode?> result = new OperationResult<ASTNode?>(null);
                            result.Errors = LR.Errors;
                            result.AddError(new UnableToParseExpressionError(context.Current));
                            return result;
                        }
                        if (LR.Errors.Count == 0)
                        {
                            node.AddChild(LR.Result);
                        }
                    }
                    if (context.Current == null)
                    {
                        var or = new OperationResult<ASTNode?>(null);
                        or.AddError(new UnexpectedEndOfFileError(context.Current));
                        return or;
                    }
                    {
                        var RC = new SegmentContext(context.Current.Next);
                        var RR = ParseEval(RC);
                        if (RR.Errors.Count == 0)
                        {
                            if (RR.Result == null)
                            {

                            }
                            else
                            {
                                node.AddChild(RR.Result);

                            }
                        }
                        else
                        {
                            return new OperationResult<ASTNode?>(null) { Errors = RR.Errors };
                        }
                    }
                    return node;
                }
                else
                {
                    context.GoNext();
                }
            }
            return new OperationResult<ASTNode?>(null);
        }
        public OperationResult<ASTNode?> ParseEval(SegmentContext context)
        {
            SegmentContext segmentContext = new SegmentContext(context.Current);
            segmentContext.SetEndPoint(context.EndPoint);
            if (segmentContext.Current == null)
            {
                return new OperationResult<ASTNode?>(null);
            }
            if (segmentContext.Current.Next == null)
            {
                return new OperationResult<ASTNode?>(new ASTNode { Segment = segmentContext.Current , Type = ASTNodeType.EndNode });
            }
            if (segmentContext.Current.Next.content == "" && segmentContext.Current.Next.Next == null)
            {
                return new OperationResult<ASTNode?>(new ASTNode { Segment = segmentContext.Current , Type = ASTNodeType.EndNode });
            }
            if (segmentContext.Current == segmentContext.EndPoint)
            {
                return new OperationResult<ASTNode?>(new ASTNode { Segment = segmentContext.Current , Type = ASTNodeType.EndNode });
            }
            if (context.Current!.Prev == null)
            {
                bool Hit = false;
                if (context.Current.Next != null)
                {
                    if (context.Current.Next.content == "")
                    {
                        if (context.Current.Next.Next == null)
                        {
                            Hit = true;
                        }
                    }
                }
                else
                {
                    Hit = true;
                }
                if (Hit)
                {
                    if (segmentContext.Current is ClosableSegment cs)
                    {
                        var head = cs.ClosedSegments.First();
                        var tail = cs.ClosedSegments.Last();
                        var sc = new SegmentContext(head);
                        sc.SetEndPoint(tail);
                        var opres = ParseEval(sc);
                        if (opres.Errors.Count == 0)
                        {
                            return opres;
                        }
                        else
                        {
                            return new OperationResult<ASTNode?>(null) { Errors = opres.Errors };
                        }
                    }
                    else
                    {
                        return new OperationResult<ASTNode?>(new ASTNode { Segment = segmentContext.Current , Type = ASTNodeType.EndNode });
                    }
                }
            }
            var or = PerformClosure(segmentContext , "(" , ")");
            if (or.Errors.Count > 0)
            {
                return new OperationResult<ASTNode?>(null) { Errors = or.Errors };
            }
            segmentContext.ResetCurrent();
            {
                var result = SearchBinaryExpression(segmentContext , "&&" , "||");
                if (result.Result != null)
                {
                    return result.Result;
                }
            }
            segmentContext.ResetCurrent();
            {
                var result = SearchBinaryExpression(segmentContext , "==" , ">=" , "<=" , "!=" , ">" , "<");
                if (result.Result != null)
                {
                    return result.Result;
                }
            }
            segmentContext.ResetCurrent();
            {
                var result = SearchBinaryExpression(segmentContext , "*" , "/");
                if (result.Result != null)
                {
                    return result.Result;
                }
            }
            segmentContext.ResetCurrent();
            {
                var result = SearchBinaryExpression(segmentContext , "+" , "-");
                if (result.Result != null)
                {
                    return result.Result;
                }
            }
            segmentContext.ResetCurrent();
            {
                var result = SearchUnaryExpression(segmentContext , "!" , "defined");
                if (result.Result != null)
                {
                    return result.Result;
                }
            }
            return new OperationResult<ASTNode?>(null);
        }
        public OperationResult<object?> Eval(ASTNode node)
        {
#if DEBUG
            if (node == null)
                Console.WriteLine($"Node not exist!");
            else
            {
                Console.WriteLine($"Node Type:{node.Type}");
                Console.WriteLine($"Node Segment:{node.Segment?.content}");

            }
#endif
            if (node == null)
            {
                return new OperationResult<object?>(null);
            }
            if (node.Type == ASTNodeType.EndNode)
            {
                if (node.Segment != null)
                {
                    var toConvert = node.Segment.content;
                    if (Symbols.ContainsKey(toConvert))
                    {
                        toConvert = Symbols [ toConvert ];
#if DEBUG
                        Console.WriteLine($"\tMacro Replacement: {toConvert}");
#endif
                    }
                    if (bool.TryParse(toConvert , out var bres))
                    {
                        return bres;
                    }
                    else
                    if (int.TryParse(toConvert , out var ires))
                    {
                        return ires;
                    }
                    else
                    if (long.TryParse(toConvert , out var lres))
                    {
                        return lres;
                    }
                    return node.Segment.content;
                }
            }
            if (node.Type == ASTNodeType.BinaryExpression)
            {
                if (node.Segment != null)
                {
                    var Operator = node.Segment.content;
                    switch (Operator)
                    {
                        case "==":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                var cmp = Comparer.Default.Compare(LR.Result , RR.Result);
                                return cmp == 0;
                            }
                        case "&&":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                if (LR.Result is bool LB)
                                {
                                    if (RR.Result is bool RB)
                                    {
                                        return LB && RB;
                                    }
                                    else
                                    {
                                        var result = new OperationResult<object?>(null);
                                        result.AddError(new NotABoolError(RN?.Segment));
                                        return result;
                                    }
                                }
                                else
                                {
                                    var result = new OperationResult<object?>(null);
                                    result.AddError(new NotABoolError(LN?.Segment));
                                    return result;
                                }
                            }
                        case "||":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                if (LR.Result is bool LB)
                                {
                                    if (RR.Result is bool RB)
                                    {
                                        return LB || RB;
                                    }
                                    else
                                    {
                                        var result = new OperationResult<object?>(null);
                                        result.AddError(new NotABoolError(RN?.Segment));
                                        return result;
                                    }
                                }
                                else
                                {
                                    var result = new OperationResult<object?>(null);
                                    result.AddError(new NotABoolError(LN?.Segment));
                                    return result;
                                }
                            }
                        case "!=":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                var cmp = Comparer.Default.Compare(LR.Result , RR.Result);
                                return cmp != 0;
                            }
                        case "*":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                if (LR.Result is int LI)
                                {
                                    if (RR.Result is int RI)
                                    {
                                        return LI * RI;
                                    }
                                    else if (RR.Result is long RL)
                                    {
                                        return LI * RL;
                                    }
                                    else
                                    {
                                        var r = new OperationResult<object?>(null);
                                        r.AddError(new NotANumberError(RN?.Segment));
                                        return r;
                                    }
                                }
                                if (LR.Result is long LL)
                                {
                                    if (RR.Result is int RI)
                                    {
                                        return LL * RI;
                                    }
                                    else if (RR.Result is long RL)
                                    {
                                        return LL * RL;
                                    }
                                    else
                                    {
                                        var r = new OperationResult<object?>(null);
                                        r.AddError(new NotANumberError(RN?.Segment));
                                        return r;
                                    }
                                }
                                else
                                {
                                    var r = new OperationResult<object?>(null);
                                    r.AddError(new NotANumberError(LN?.Segment));
                                    return r;
                                }
                            }
                        case "/":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                if (LR.Result is int LI)
                                {
                                    if (RR.Result is int RI)
                                    {
                                        return LI / RI;
                                    }
                                    else if (RR.Result is long RL)
                                    {
                                        return LI / RL;
                                    }
                                    else
                                    {
                                        var r = new OperationResult<object?>(null);
                                        r.AddError(new NotANumberError(RN?.Segment));
                                        return r;
                                    }
                                }
                                if (LR.Result is long LL)
                                {
                                    if (RR.Result is int RI)
                                    {
                                        return LL / RI;
                                    }
                                    else if (RR.Result is long RL)
                                    {
                                        return LL / RL;
                                    }
                                    else
                                    {
                                        var r = new OperationResult<object?>(null);
                                        r.AddError(new NotANumberError(RN?.Segment));
                                        return r;
                                    }
                                }
                                else
                                {
                                    var r = new OperationResult<object?>(null);
                                    r.AddError(new NotANumberError(LN?.Segment));
                                    return r;
                                }
                            }
                        case "+":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                if (LR.Result is int LI)
                                {
                                    if (RR.Result is int RI)
                                    {
                                        return LI / RI;
                                    }
                                    else if (RR.Result is long RL)
                                    {
                                        return LI / RL;
                                    }
                                    else
                                    {
                                        var r = new OperationResult<object?>(null);
                                        r.AddError(new NotANumberError(RN?.Segment));
                                        return r;
                                    }
                                }
                                if (LR.Result is long LL)
                                {
                                    if (RR.Result is int RI)
                                    {
                                        return LL + RI;
                                    }
                                    else if (RR.Result is long RL)
                                    {
                                        return LL + RL;
                                    }
                                    else
                                    {
                                        var r = new OperationResult<object?>(null);
                                        r.AddError(new NotANumberError(RN?.Segment));
                                        return r;
                                    }
                                }
                                else
                                {
                                    var r = new OperationResult<object?>(null);
                                    r.AddError(new NotANumberError(LN?.Segment));
                                    return r;
                                }
                            }
                        case "-":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                if (LR.Result is int LI)
                                {
                                    if (RR.Result is int RI)
                                    {
                                        return LI - RI;
                                    }
                                    else if (RR.Result is long RL)
                                    {
                                        return LI - RL;
                                    }
                                    else
                                    {
                                        var r = new OperationResult<object?>(null);
                                        r.AddError(new NotANumberError(RN?.Segment));
                                        return r;
                                    }
                                }
                                if (LR.Result is long LL)
                                {
                                    if (RR.Result is int RI)
                                    {
                                        return LL / RI;
                                    }
                                    else if (RR.Result is long RL)
                                    {
                                        return LL / RL;
                                    }
                                    else
                                    {
                                        var r = new OperationResult<object?>(null);
                                        r.AddError(new NotANumberError(RN?.Segment));
                                        return r;
                                    }
                                }
                                else
                                {
                                    var r = new OperationResult<object?>(null);
                                    r.AddError(new NotANumberError(LN?.Segment));
                                    return r;
                                }
                            }
                        case ">=":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;
                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                var cmp = Comparer.Default.Compare(LR.Result , RR.Result);
                                return cmp >= 0;
                            }
                        case ">":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                var cmp = Comparer.Default.Compare(LR.Result , RR.Result);
                                return cmp > 0;
                            }
                        case "<=":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                var cmp = Comparer.Default.Compare(LR.Result , RR.Result);
                                return cmp <= 0;
                            }
                        case "<":
                            {
                                var LN = node.Children [ 0 ] as ASTNode;
                                var RN = node.Children [ 1 ] as ASTNode;

                                var LR = Eval(LN!);
                                if (LR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = LR.Errors };
                                var RR = Eval(RN!);
                                if (RR.Errors.Count > 0) return new OperationResult<object?>(null) { Errors = RR.Errors };
                                var cmp = Comparer.Default.Compare(LR.Result , RR.Result);
                                return cmp < 0;
                            }
                    }
                }
            }
            else
            if (node.Type == ASTNodeType.UnaryExpression)
            {
                if (node.Segment != null)
                    if (node.Segment.content == "defined")
                    {
                        var n = node.Children.First() as ASTNode;
                        var r = Eval(n!);
                        if (r.Errors.Count > 0) return new OperationResult<object?>(false) { Errors = r.Errors };
                        return defined(r.Result as string ?? "");
                    }
                    else if (node.Segment.content == "!")
                    {
                        var n = node.Children.First() as ASTNode;
                        var r = Eval(n!);
                        if (r.Errors.Count > 0) return new OperationResult<object?>(false) { Errors = r.Errors };
                        switch (r.Result)
                        {
                            case bool b:
                                {
                                    return new OperationResult<object?>(!b);
                                }
                            case int i:
                                {
                                    return new OperationResult<object?>(i == 0);
                                }
                            case long l:
                                {
                                    return new OperationResult<object?>(l == 0);
                                }
                            default:
                                break;
                        }
                    }
            }
            return new OperationResult<object?>(true);
        }
        public OperationResult<bool> _ifdef(SegmentContext context)
        {
            var c = context.Current?.content ?? "";
            return defined(c);
        }
        public OperationResult<bool> _ifndef(SegmentContext context)
        {
            var c = context.Current?.content ?? "";
            return !defined(c);
        }
        public OperationResult<bool> _if(SegmentContext context)
        {
            var AST = ParseEval(context);
            if (AST.Errors.Count == 0)
            {
                if (AST.Result == null)
                {
                    return new OperationResult<bool>(false);
                }
                var r = Eval(AST.Result);
                if (r.Errors.Count == 0)
                {
                    var er = r.Result;
                    if (er is bool b)
                    {
                        return b;
                    }
                    else if (er is int i)
                    {
                        return i > 0;
                    }
                }
                else
                {
                    return new OperationResult<bool>(false) { Errors = r.Errors };
                }
                return false;
            }
            return new OperationResult<bool>(false) { Errors = AST.Errors };
        }
        public bool ifdef(SegmentContext context)
        {
            return defined(context.Current?.content ?? "");
        }
        public bool elif(SegmentContext context)
        {
            return _if(context);
        }
        public bool defined(string name)
        {
            return Symbols.ContainsKey(name);
        }
        public OperationResult<Preprocessed?> Process(VirtualFile InputCFile , Preprocessed? predprepro = null)
        {
            Preprocessed preprocessed;
            if (predprepro != null)
            {
                preprocessed = predprepro;
            }
            else { preprocessed = new Preprocessed(); }
            Process(InputCFile , preprocessed , false);
            return new OperationResult<Preprocessed?>(null);
        }
        public (string, bool) process_line(string Line)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var LineParse = CStyleParser.Scan(Line , false);
            FloatPointScanner.ScanFloatPoint(ref LineParse);
            SegmentContext segmentContext = new SegmentContext(LineParse);
            bool MacroReplaced = false;
            while (true)
            {
                if (segmentContext.ReachEnd)
                {
                    break;
                }
                if (segmentContext.Current == null)
                {
                    break;
                }
                if (segmentContext.Current.content == "" && segmentContext.Current.Next == null)
                {
                    break;
                }
                var item = segmentContext.Current.content;
                stringBuilder.Append(" ");
                if (segmentContext.Current.isEncapsulated)
                {
                    stringBuilder.Append(segmentContext.Current.EncapsulationIdentifier.L);
                    stringBuilder.Append(item);
                    stringBuilder.Append(segmentContext.Current.EncapsulationIdentifier.R);
                }
                else
                {

                    if (Symbols.ContainsKey(item))
                    {
                        MacroReplaced = true;
                        stringBuilder.Append(Symbols [ item ]);
                    }
                    else
                    {
                        stringBuilder.Append(item);
                    }
                }
                segmentContext.GoNext();
            }
            return (stringBuilder.ToString(), MacroReplaced);
        }
        public OperationResult<string?> ProcessMacroLine(VirtualFile CurrentFile ,
                                                           string Line ,
                                                           ref bool willskip ,
                                                           ref bool SkipFile ,
                                                           ref Preprocessed preprocessed ,
                                                           ref int IFSCOPE ,
                                                           ref int SKIP_POINT_IF_LAYER)
        {
            var LineParse = CStyleParser.Scan(Line [ 1.. ] , false);
            FloatPointScanner.ScanFloatPoint(ref LineParse);
            SegmentContext segmentContext = new SegmentContext(LineParse);
            var macro = segmentContext.MatchCollectionMarch(false , "include" , "define" , "if" , "undefine"
                , "ifndef" , "ifdef" , "elif" , "endif" , "else" , "pragma");
            if (macro.Item1 == MatchResult.Match)
            {
                switch (macro.Item2)
                {
                    case 0:
                        {
                            if (!willskip)
                            {
                                var m = segmentContext.MatchNext("<" , true);
                                if (segmentContext.Current == null)
                                {

                                    var result = new OperationResult<string?>(null);
                                    result.AddError(new UnexpectedEndError(segmentContext.Current));
                                    return result;
                                }
                                if (m == MatchResult.Match)
                                {
                                    var remain = segmentContext.FormTillEnd(true , (char)0).Trim();
                                    if (remain.EndsWith(">"))
                                    {

                                        var f = FilesProvider.Find(remain [ ..^1 ]);
                                        if (f != null)
                                        {
                                            Process(f , preprocessed , true);
                                        }
                                    }
                                    else
                                    {
                                        var result = new OperationResult<string?>(null);
                                        result.AddError(new MisClosureError(segmentContext.Current));
                                        return result;
                                    }
                                }
                                else if (segmentContext.Current.isEncapsulated)
                                {
                                    var f = FilesProvider.Find(segmentContext.Current?.content ?? "" , CurrentFile);
                                    if (f != null)
                                    {
                                        Process(f , preprocessed , true);
#if DEBUG
                                        Console.WriteLine($"Preprocess include:{segmentContext.Current?.content}");
#endif
                                    }
                                }

                            }
                        }
                        break;
                    case 1:
                        {
                            if (willskip)
                            {
                                return new OperationResult<string?>(null);
                            }
                            define(segmentContext);
                            return new OperationResult<string?>(null);
                        }
                    case 2:
                        {
                            if (!willskip)
                            {
                                willskip = !_if(segmentContext);
                                if (willskip) SKIP_POINT_IF_LAYER = IFSCOPE;
                            }
                            IFSCOPE++;
                        }
                        break;
                    case 3:
                        {
                            if (willskip)
                            {
                                return new OperationResult<string?>(null);
                            }
                            Undefine(segmentContext);
                            return Line;
                        }
                    case 4:
                        {
                            if (!willskip)
                            {
                                willskip = !_ifndef(segmentContext);
                                if (willskip) SKIP_POINT_IF_LAYER = IFSCOPE;
                            }
                            IFSCOPE++;
                        }
                        break;
                    case 5:
                        {
                            if (!willskip)
                            {
                                willskip = !_ifdef(segmentContext);
                                if (willskip) SKIP_POINT_IF_LAYER = IFSCOPE;
                            }
                            IFSCOPE++;
                        }
                        break;
                    case 6:
                        {
                            if (IFSCOPE <= 0)
                            {

                                var result = new OperationResult<string?>(null);
                                result.AddError(new ElifWithoutIfError(segmentContext.Current));
                                return result;
                            }
                            if (willskip)
                            {
                                willskip = !_if(segmentContext);
                                if (willskip) SKIP_POINT_IF_LAYER = IFSCOPE;
                            }
                        }
                        break;
                    case 7:
                        {
                            IFSCOPE--;
                            if (IFSCOPE < 0)
                            {

                                var result = new OperationResult<string?>(null);
                                result.AddError(new EndIfWithoutIfError(segmentContext.Current));
                                return result;
                            }
                            if (willskip)
                            {
                                if (IFSCOPE == SKIP_POINT_IF_LAYER)
                                {
#if DEBUG
                                    Console.WriteLine("Skip closed.");
#endif
                                    willskip = false;
                                }
                            }
                        }
                        break;
                    case 8:
                        {
                            if (IFSCOPE > 0)
                                willskip = !willskip;
                            else
                            {
                                var result = new OperationResult<string?>(null);
                                result.AddError(new ElseWithoutIfError(segmentContext.Current));
                                return result;
                            }
                        }
                        break;
                    case 9:
                        {

                            var pragma = segmentContext.MatchCollectionMarch(false , "once");
                            if (pragma.Item1 == MatchResult.Match)
                            {
                                switch (pragma.Item2)
                                {
                                    case 0:
                                        {

                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return new OperationResult<string?>(null);
        }
        public void Process(VirtualFile Input , Preprocessed preprocessed , bool isHeader)
        {
#if DEBUG
            Console.WriteLine($"Prcess:{Input.ID}");
#endif
            StreamWriter sw;
            VirtualFile OutputFile;
            if (!isHeader)
            {
                OutputFile = new VirtualFile(Input.ID);
                OutputFile.FileInMemory = new MemoryStream();
                preprocessed.ProcessedCFile.Add(OutputFile);
            }
            else
            {
                if (preprocessed.CombinedHeader != null)
                {
                    OutputFile = preprocessed.CombinedHeader;
                }
                else
                {
                    OutputFile = new VirtualFile(Input.ID);
                    OutputFile.FileInMemory = new MemoryStream();
                    preprocessed.CombinedHeader = OutputFile;
                }
                if (!preprocessed.ProcessedHeader.ContainsKey(Input.ID))
                    preprocessed.ProcessedHeader.Add(Input.ID , Input);
            }
            sw = OutputFile.GetWriter();
            StreamReader streamReader = new StreamReader(Input.GetStream());
            string? Line = null;
            int IFSCOPE = 0;
            int SKIP_POINT_IF_LAYER = 0;
            bool willskip = false;
            bool SkipFile = false;
            //if (isHeader)
            //{
            //    preprocessed.ProcessedHeader.Add(Input.ID , Input);
            //}
            while (true)
            {
                Line = streamReader.ReadLine();
                if (Line == null) break;
                Line = Line.Trim();
#if DEBUG
                Console.WriteLine($"Preprocess:\t{Line}");
#endif
                if (Line.EndsWith('\\'))
                {
                    while (true)
                    {
                        var NLine = streamReader.ReadLine();
                        if (NLine == null)
                        {
                            break;
                        }
                        Line = Line.Substring(0 , Line.Length - 1);
                        Line += "\n";
                        Line += NLine;
                        if (!NLine.Trim().EndsWith('\\'))
                        {
                            break;
                        }
                    }
                }
                if (Line == null) break;
                if (Line.StartsWith("#"))
                {
                    var preprocessed_line = ProcessMacroLine(Input , Line , ref willskip , ref SkipFile , ref preprocessed , ref IFSCOPE , ref SKIP_POINT_IF_LAYER);
                    if (SkipFile)
                    {
                        break;
                    }
                    if (preprocessed_line != null)
                    {
                        if (preprocessed_line.Result != null)
                            sw.WriteLine(preprocessed_line.Result);
                    }
                }
                else
                {
                    if (!willskip)
                    {
                        var _Line = Line;
                        var _continue = false;
                        while (true)
                        {

                            (_Line,_continue) = process_line(_Line);
                            if (_continue == false)
                            {
                                break;
                            }
                        }
                        sw.WriteLine(_Line);
                    }
                }
            }
            sw.Flush();
            OnSingleFileProcessComplete.Invoke(OutputFile);
            if (CloseSWOnProcessEnd)
                sw.Close();
            if (CloseSROnProcessEnd)
                streamReader.Close();
            return;
        }
    }
}
