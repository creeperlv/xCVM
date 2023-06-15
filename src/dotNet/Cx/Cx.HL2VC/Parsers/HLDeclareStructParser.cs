using Cx.Core;
using Cx.Core.SegmentContextUtilities;
using Cx.Core.VCParser;
using LibCLCC.NET.TextProcessing;
using System;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Parsers
{
    public class HLDeclareStructParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            var headmatch = context.Match("struct");
            var HEAD = context.Current;
            if (HEAD == null)
            {
                return false;
            }
            TreeNode node = new TreeNode();
            node.Segment = HEAD.Duplicate();
            if (headmatch == MatchResult.Match)
            {
                context.GoNext();
                var FormedName = Utilities.FormName(context , false);
                if (FinalResult.CheckAndInheritAbnormalities(FormedName))
                {
                    return FinalResult;
                }
                else
                {
                    var StructureName = FormedName.Result;
#if DEBUG
                    Console.WriteLine($"HLDeclareStructParser:{StructureName},{context.Current?.content??"null"}");
#endif
                    var LBTest = context.Match("{");
                    if ( LBTest== MatchResult.Match)
                    {
                        var DeclareVar = provider.GetParser(ASTNodeType.DeclareVar);
                        if (DeclareVar == null)
                        {
                            FinalResult.AddError(new ParserNotFoundError(context.Current , ASTNodeType.DeclareVar));
                            return FinalResult;
                        }
                        var _context = new SegmentContext(HEAD);
                       
                        var ClosedResult = ContextClosure.LRClose(context , "{" , "}");
                        if (FinalResult.CheckAndInheritAbnormalities(ClosedResult))
                        {
                            return FinalResult;
                        }
                        if (ClosedResult.Result == null)
                        {
                            FinalResult.AddError(new ClosureError(context.Current , "{" , "}"));
                            return FinalResult;
                        }
                        SegmentContext ClosedContext = ClosedResult.Result;
                        ClosedContext.GoNext();
                        while (true)
                        {
                            if (ClosedContext.ReachEnd)
                            {
                                break;
                            }
#if DEBUG
                            Console.WriteLine($"HLDCParser: Fields: {ClosedContext.Current?.content??"null"}");
#endif
                            var dsr = DeclareVar.Parse(provider , ClosedContext , node);

                            if (FinalResult.CheckAndInheritAbnormalities(dsr))
                            {
                                return FinalResult;
                            }
                            if (dsr.Result == false)
                            {
                                FinalResult.AddError(new WrongSubASTNode(ClosedContext.Current , ASTNodeType.DeclareVar));
                                return FinalResult;
                            }
#if DEBUG
                            Console.WriteLine($"HLDCParser: Fields Done 0: {ClosedContext.Current?.content ?? "null"}");
#endif
                            ClosedContext.GoNext();
#if DEBUG
                            Console.WriteLine($"HLDCParser: Fields Done 1: {ClosedContext.Current?.content ?? "null"}");
#endif
                            if (ClosedContext.ReachEnd)
                            {
                                FinalResult.AddError(new UnexpectedEndError(ClosedContext.Current));
                                return FinalResult;
                            }
                            if (ClosedContext.Current?.content != ";")
                            {
#if DEBUG
                                Console.WriteLine($"\x1b[31mError\x1b[0m HLDCParser: Not Semi-colon");
#endif
                                FinalResult.AddError(new IllegalIdentifierError(_context.Current));
                                return FinalResult;
                            }
                            ClosedContext.GoNext();
                        }
                        context.SetCurrent(ClosedContext.EndPoint);
#if DEBUG
                        Console.WriteLine("HLDCParser:Done!");
#endif
                    }
                    else
                    {
                        return FinalResult;
                    }
                    node.Segment.content = StructureName;
                    node.Type = ASTNodeType.DeclareStruct;
                    FinalResult.Result = true;
                    Parent.AddChild(node);
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("HLDSParser: Not A Struct!!");
#endif
            }
            return FinalResult;
        }
    }
}