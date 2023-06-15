using Cx.Core;
using Cx.Core.DataTools;
using Cx.Core.SegmentContextUtilities;
using Cx.Core.VCParser;
using Cx.HL2VC.Parsers;
using LibCLCC.NET.TextProcessing;
using System;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC
{
    public class NamespaceParser : ContextualParser
    {
        public NamespaceParser()
        {
            ConcernedParsers.Add(ASTNodeType.DeclareStruct);
            //ConcernedParsers.Add(ASTNodeType.TypeDef);
            ConcernedParsers.Add(ASTNodeType.DeclareFunc);
        }
        public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
        {
            OperationResult<bool> result = new OperationResult<bool>(false);
            var BRKPNT = context.Current;
            var nr = context.Match("namespace");
#if DEBUG
            Console.WriteLine($"namespace?{context.Current?.content ?? "null"}={nr}");
#endif
            if (nr == MatchResult.Match)
            {
                context.GoNext();
#if DEBUG
                Console.WriteLine($"namespace...:{context.Current?.content ?? "null"}");
#endif
                var __head = context.Current;
                if (__head == null)
                {
                    result.AddError(new UnexpectedEndError(__head));
                    return result;
                }
                var NamespaceResult = Utilities.FormName(context , true);
                if (result.CheckAndInheritAbnormalities(NamespaceResult))
                {
                    return result;
                }
                var FormedPrefix = NamespaceResult.Result;
                var __seg = __head.Duplicate();
                __seg.content = FormedPrefix;
                TreeNode root = new TreeNode();
                root.Type = HLASTNodeType.Namespace;
#if DEBUG
                Console.WriteLine($"namespace formed:{FormedPrefix}");
#endif
                root.Segment = __seg;
                if (context.Match("{") == MatchResult.Match)
                {
                    var __r = ContextClosure.LRClose(context , "{" , "}");
                    if (__r.Errors.Count > 0)
                    {
                        OperationResult<bool> operationResult = false;
                        operationResult.Errors = __r.Errors;
#if DEBUG
                        Console.WriteLine($"Error in LR closure!");
#endif
                        return operationResult;
                    }
                    var newContext = __r.Result;
                    if (newContext == null)
                    {
                        result.AddError(new ClosureError(__head , "{" , "}"));
                        return result;
                    }
#if DEBUG
                    Console.WriteLine($"NamespaceParser: LRClosure.HEAD:{newContext.HEAD?.content??"null"}");
#endif
                    newContext.GoNext();
#if DEBUG
                    Console.WriteLine($"NamespaceParser: LRClosure.Current:{newContext.Current?.content ?? "null"}");
#endif
                    {
                        Parent.AddChild(root);
                        while (true)
                        {
                            if (newContext.ReachEnd) break;
                            if (newContext.Current == null) break;
                            if (newContext.Current.Next == null) break;
                            var Hit = false;
                            var _Current = newContext.Current;
#if DEBUG
                            Console.WriteLine($"NamespaceParser: Current:{newContext.Current?.content ?? "null"}");
#endif
                            foreach (var id in ConcernedParsers)
                            {
                                newContext.SetCurrent(_Current);
                                var item = provider.GetParser(id);
                                if (item == null)
                                {
                                    result.AddError(new ParserNotFoundError(newContext.Current , id));
                                    return result;
                                }
#if DEBUG
                                Console.WriteLine($"NP: Try at:{newContext.Current?.content ?? "null"} with:{id}");
#endif
                                var _result = item.Parse(provider , newContext , root);
                                if (result.CheckAndInheritAbnormalities(_result)) return result;
                                if (_result.Result == true)
                                {
                                    Hit = true;
                                    break;
                                }
                            }
                            if (Hit == false)
                            {
                                result.Result = false;
#if DEBUG
                                Console.WriteLine("NamespaceParser:Miss!");
#endif
                                foreach (var id in ConcernedParsers)
                                {
                                    result.AddError(new ParseFailError(_Current,id));
                                }
                                return result;
                            }
                            newContext.GoNext();
                        }
                    }

                    result.Result = true;
                    context.SetCurrent(newContext.EndPoint);
                    return result;
                }

            }
            else
            {
                context.SetCurrent(BRKPNT);
            }
            return result;
        }
    }
}
