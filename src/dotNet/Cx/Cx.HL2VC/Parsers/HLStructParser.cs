using Cx.Core;
using Cx.Core.SegmentContextUtilities;
using Cx.Core.VCParser;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Parsers
{
    public class HLStructParser : ContextualParser
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
                    FinalResult.Errors = FormedName.Errors;
                    return FinalResult;
                }
                else
                {
                    var StructureName = FormedName.Result;
                    if (context.Match("{") == MatchResult.Match)
                    {
                        var DeclareStruct = provider.GetParser(ASTNodeType.DeclareStruct);
                        if (DeclareStruct == null)
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
                        DeclareStruct.Parse(provider , _context , TargetRootNode);
                    }
                    TreeNode node = new TreeNode();
                    node.Segment = HEAD.Duplicate();
                    node.Segment.content = StructureName;
                    FinalResult.Result = true;
                }
            }
            return FinalResult;
        }
    }
}