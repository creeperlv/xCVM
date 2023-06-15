using Cx.Core;
using Cx.Core.DataTools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Parsers
{
    public class Utilities
    {
        public static TreeNode? GetRootNode(TreeNode node)
        {
            TreeNode? Current = node;
            if (Current == null) return null;
            while (true)
            {
                if (Current.Type == ASTNodeType.Root)
                {
                    return Current;
                }
                if (Current.Parent == null)
                {
                    return Current;
                }
                Current = Current.Parent;
            }
        }
        public static TreeNode? GetNamespaceNode(TreeNode node)
        {
            TreeNode? Current = node;
            while (true)
            {
                if (Current == null) break;
                if (Current.Type == HLASTNodeType.Namespace)
                {
                    return Current;
                }
                Current = Current.Parent;
            }
            return null;
        }
        public static bool IsInNamespace(TreeNode node)
        {
            TreeNode? Current = node;
            while (true)
            {
                if (Current == null) break;
                if (Current.Type == HLASTNodeType.Namespace)
                {
                    return true;
                }
                Current = Current.Parent;
            }
            return false;
        }
        /// <summary>
        /// Ends with the Segment that farther than the real end name segment.
        /// <br/>
        /// A . B =
        /// <br/>
        /// Ends at '='
        /// </summary>
        /// <param name="context"></param>
        /// <param name="AppendSplitter"></param>
        /// <returns></returns>
        public static OperationResult<string> FormName(SegmentContext context , bool AppendSplitter)
        {

            string FormedPrefix = "";

            DataType LastDT = DataType.Symbol;
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
                                if (AppendSplitter)
                                    if (!FormedPrefix.EndsWith("_"))
                                        FormedPrefix += "_";
                                return FormedPrefix;
                            }
                            FormedPrefix += Current.content;
                            LastDT = DT;
                        }
                        break;
                    case DataType.Symbol:
                        {
                            if (LastDT == DataType.Symbol)
                            {
                                if (AppendSplitter)
                                    if (!FormedPrefix.EndsWith("_"))
                                        FormedPrefix += "_";
                                return FormedPrefix;
                            }
                            else
                            {
                                if (Current.content == ".")
                                {
                                    FormedPrefix += "_";
                                }
                                else
                                {
                                    if (AppendSplitter)
                                        if (!FormedPrefix.EndsWith("_"))
                                            FormedPrefix += "_";
                                    return FormedPrefix;
                                }
                                LastDT = DT;
                            }
                        }
                        break;
                    default:
                        {
                            if (AppendSplitter)
                                if (!FormedPrefix.EndsWith("_"))
                                    FormedPrefix += "_";
                            return FormedPrefix;
                        }
                }
                context.GoNext();
            }
            return FormedPrefix;
        }
    }
}
