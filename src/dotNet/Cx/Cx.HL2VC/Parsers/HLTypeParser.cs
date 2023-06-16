using Cx.Core;
using Cx.Core.DataTools;
using Cx.Core.VCParser;
using System;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Parsers
{
    public class HLTypeParser : ContextualParser
    {
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> FinalResult = false;
            string FormedType = "";
            TreeNode _node = new TreeNode();
            _node.Type = ASTNodeType.DataType;
            DataType LastDT = DataType.Symbol;
            var HEAD = context.Current;
            if (HEAD == null)
            {
                return FinalResult;
            }

#if DEBUG
            Console.WriteLine($"HLTP: Is it struct??{context.Current?.content??"null"}");
#endif
            {
                var mr = context.Match("struct");
                if (mr == MatchResult.Match)
                {
#if DEBUG
                    Console.WriteLine("HLTP: Yes");
#endif
                    context.GoNext();
                    var name = Utilities.FormName(context , false);
                    if (FinalResult.CheckAndInheritAbnormalities(name))
                    {
                        return FinalResult;
                    }
                    var name_str = name.Result;
                    if (context.ReachEnd)
                    {
                        FinalResult.AddError(new UnexpectedEndError(context.Current));
                        return FinalResult;
                    }
                    if (context.Match("{") == MatchResult.Match)
                    {
#if DEBUG
                        Console.WriteLine("HLTP: This is a struct definition.");
#endif
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
                        var DeclareStruct = provider.GetParser(ASTNodeType.DeclareStruct);
                        if (DeclareStruct == null)
                        {
                            FinalResult.AddError(new ParserNotFoundError(context.Current , ASTNodeType.DeclareStruct));
                            return FinalResult;
                        }
                        context.SetCurrent(HEAD);
                        var dsr = DeclareStruct.Parse(provider , context , TargetRootNode);
                        if (FinalResult.CheckAndInheritAbnormalities(dsr)) { return FinalResult; }
                        if (dsr.Result == false)
                        {
                            FinalResult.AddError(new ParseFailError(HEAD , ASTNodeType.DeclareStruct));
                            return FinalResult;
                        }
                        _node.Segment = HEAD;
                        TreeNode _use_struct = new TreeNode();
                        _use_struct.Type = ASTNodeType.UseStruct;
                        _use_struct.Segment = HEAD.Next.Duplicate();
                        _use_struct.Segment.content = name_str;
                        _node.Segment = HEAD;
                        _node.AddChild(_use_struct);
                        Parent.AddChild(_node);
                        FinalResult.Result = true;
                        return FinalResult;
                    }
                    else
                    {
                        _node.Segment = HEAD;
                        TreeNode _use_struct = new TreeNode();
                        _use_struct.Type = ASTNodeType.UseStruct;
                        _use_struct.Segment = HEAD.Next.Duplicate();
                        _use_struct.Segment.content = name_str;
                        _node.AddChild(_use_struct);
                        _node.Segment = HEAD;
                        Parent.AddChild(_node);
                        FinalResult.Result = true;
                        return FinalResult;
                    }
                }
            }
            {
#if DEBUG
                Console.WriteLine("HLTP: Not A Struct.");
#endif
                var NameResult = Utilities.FormName(context , false);
                if (FinalResult.CheckAndInheritAbnormalities(NameResult)) return FinalResult;
                string str = NameResult;
                _node.Segment = HEAD.Duplicate();
                _node.Segment.content = str;
                //context.GoNext();
                while (true)
                {
                    if (context.ReachEnd) break;
                    if (context.Current == null) break;

#if DEBUG
                    Console.WriteLine($"HLTP: Wrap Pointer? {context.Current.content}");
#endif
                    if (context.Current.content == "*")
                    {
                        TreeNode PointerWrapper = new TreeNode();
                        PointerWrapper.Type = ASTNodeType.Pointer;
                        PointerWrapper.Segment = null;
                        PointerWrapper.AddChild(_node);
                        _node = PointerWrapper;
                        context.GoNext();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            {

#if DEBUG
                Console.WriteLine($"HLTP: End 0:{context.Current?.content??"null"}");
#endif
                Parent.AddChild(_node);
                //context.GoBack();
                FinalResult.Result = true;
                return FinalResult;
            }
        }
    }
}
