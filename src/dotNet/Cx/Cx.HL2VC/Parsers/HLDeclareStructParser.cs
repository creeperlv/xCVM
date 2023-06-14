using Cx.Core;
using Cx.Core.SegmentContextUtilities;
using Cx.Core.VCParser;
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
                    if (context.Match("{") == MatchResult.Match)
                    {
                        var DeclareVar = provider.GetParser(ASTNodeType.DeclareVar);
                        if (DeclareVar == null)
                        {
                            FinalResult.AddError(new ParserNotFoundError(context.Current));
                            return FinalResult;
                        }
                        var _context = new SegmentContext(HEAD);
                        var TargetRootNode = Utilities.GetNamespaceNode(Parent);
                        if (TargetRootNode == null)
                        {
                            TargetRootNode = Utilities.GetRootNode(Parent);
                            if (TargetRootNode == null)
                            {
                                FinalResult.AddError(new CannotFoundNodeError(context.Current , ASTNodeType.Root));
                                FinalResult.AddError(new CannotFoundNodeError(context.Current , HLASTNodeType.Namespace));
                                return FinalResult;
                            }
                        }
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
                        while (true)
                        {
                            if (ClosedContext.ReachEnd)
                            {
                                break;
                            }
                            var dsr = DeclareVar.Parse(provider , _context , TargetRootNode);

                            if (FinalResult.CheckAndInheritAbnormalities(dsr))
                            {
                                return FinalResult;
                            }
                            if (dsr.Result == false)
                            {
                                FinalResult.AddError(new WrongSubASTNode(_context.Current , ASTNodeType.DeclareVar));
                                return FinalResult;
                            }
                            _context.GoNext();
                            if (ClosedContext.ReachEnd)
                            {
                                FinalResult.AddError(new UnexpectedEndError(_context.Current));
                                return FinalResult;
                            }
                            if (_context.Current?.content != ";")
                            {
                                FinalResult.AddError(new IllegalIdentifierError(_context.Current));
                                return FinalResult;
                            }
                            _context.GoNext();
                        }
                    }
                    else
                    {
                        return FinalResult;
                    }
                    TreeNode node = new TreeNode();
                    node.Segment = HEAD.Duplicate();
                    node.Segment.content = StructureName;
                    FinalResult.Result = true;
                    Parent.AddChild(node);
                }
            }
            return FinalResult;
        }
    }
}