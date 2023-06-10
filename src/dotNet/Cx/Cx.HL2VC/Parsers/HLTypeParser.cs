﻿using Cx.Core;
using Cx.Core.DataTools;
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
            if (HEAD== null) {
                return FinalResult;
            }
            {
                var mr = context.Match("struct");
                if(mr== MatchResult.Match)
                {

                    while (true)
                    {
                        if (context.ReachEnd)
                        {
                            break;
                        }
                        var Current = context.Current;
                        if (Current == null)
                        {
                            break;
                        }
                        var DT = DataTypeChecker.DetermineDataType(Current.content);
                        switch (DT)
                        {
                            case DataType.String:
                                {
                                    if (LastDT == DataType.String)
                                    {
                                        _node.Segment = HEAD.Duplicate();
                                        _node.Segment.content = FormedType;
                                        Parent.AddChild(_node);
                                        FinalResult.Result = true;
                                        context.GoBack();
                                        return FinalResult;
                                    }
                                    if (_node.Type == ASTNodeType.Pointer)
                                    {
                                        Parent.AddChild(_node);
                                        FinalResult.Result = true;
                                        context.GoBack();
                                        return FinalResult;
                                    }
                                    FormedType += Current.content;
                                    LastDT = DT;
                                }
                                break;
                            case DataType.Symbol:
                                {
                                    if (Current.content == ".")
                                    {
                                        if (_node.Type == ASTNodeType.Pointer)
                                        {
                                            Parent.AddChild(_node);
                                            FinalResult.Result = true;
                                            context.GoBack();
                                            return FinalResult;
                                        }
                                        FormedType += "_";
                                    }
                                    else
                                    if (Current.content == "{")
                                    {
                                        // Defined a new struct.
                                    }
                                    else if (Current.content == "*")
                                    {
#if DEBUG
                                        Console.WriteLine($"HLTypeParser: Wrap Pointer.");
#endif
                                        if (_node.Type == ASTNodeType.DataType)
                                        {
                                            _node.Segment = HEAD.Duplicate();
                                            _node.Segment.content = FormedType;
                                        }
                                        TreeNode Pointer = new TreeNode();
                                        Pointer.Segment = Current;
                                        Pointer.Type = ASTNodeType.Pointer;
                                        Pointer.AddChild(_node);
                                        _node = Pointer;
                                    }
                                    else
                                    {

                                        Parent.AddChild(_node);
                                        FinalResult.Result = true;
                                        context.GoBack();
                                        return FinalResult;
                                    }
                                    LastDT = DT;
                                }
                                break;
                            default:
                                {
                                    Parent.AddChild(_node);
                                    FinalResult.Result = true;
                                    context.GoBack();
                                    return FinalResult;
                                }
                        }
                        context.GoNext();
                    }
                }
            }
            while (true)
            {
                if (context.ReachEnd)
                {
                    break;
                }
                var Current = context.Current;
                if (Current == null)
                {
                    break;
                }
                var DT = DataTypeChecker.DetermineDataType(Current.content);
                switch (DT)
                {
                    case DataType.String:
                        {
                            if (LastDT == DataType.String)
                            {
                                _node.Segment = HEAD.Duplicate();
                                _node.Segment.content = FormedType;
                                Parent.AddChild(_node);
                                FinalResult.Result = true;
                                context.GoBack();
                                return FinalResult;
                            }
                            if (_node.Type == ASTNodeType.Pointer)
                            {
                                Parent.AddChild(_node);
                                FinalResult.Result = true;
                                context.GoBack();
                                return FinalResult;
                            }
                            FormedType += Current.content;
                            LastDT = DT;
                        }
                        break;
                    case DataType.Symbol:
                        {
                            if (Current.content == ".")
                            {
                                if (_node.Type == ASTNodeType.Pointer)
                                {
                                    Parent.AddChild(_node);
                                    FinalResult.Result = true;
                                    context.GoBack();
                                    return FinalResult;
                                }
                                FormedType += "_";
                            }
                            else if (Current.content == "*")
                            {
#if DEBUG
                                Console.WriteLine($"HLTypeParser: Wrap Pointer.");
#endif
                                if (_node.Type == ASTNodeType.DataType)
                                {
                                    _node.Segment = HEAD.Duplicate();
                                    _node.Segment.content = FormedType;
                                }
                                TreeNode Pointer = new TreeNode();
                                Pointer.Segment = Current;
                                Pointer.Type = ASTNodeType.Pointer;
                                Pointer.AddChild(_node);
                                _node = Pointer;
                            }
                            else
                            {

                                Parent.AddChild(_node);
                                FinalResult.Result = true;
                                context.GoBack();
                                return FinalResult;
                            }
                            LastDT = DT;
                        }
                        break;
                    default:
                        {
                            Parent.AddChild(_node);
                            FinalResult.Result = true;
                            context.GoBack();
                            return FinalResult;
                        }
                }
                context.GoNext();
            }
            {

                Parent.AddChild(_node);
                FinalResult.Result = true;
                context.GoBack();
                return FinalResult;
            }
        }
    }
}
