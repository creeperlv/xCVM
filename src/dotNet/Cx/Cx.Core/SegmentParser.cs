using Cx.Core.VCParser;
using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.Core
{
    public static class FileParser
    {
        public static OperationResult<TreeNode> Parse(ParserProvider provider , FileInfo InputFile)
        {
            CStyleScanner scanner = new CStyleScanner();
            using var fs = InputFile.OpenRead();
            using var sr = new StreamReader(fs);
            var HEAD = scanner.Scan(sr.ReadToEnd() , false , InputFile.FullName);
            return SegmentParser.Parse(provider , HEAD);
        }
    }
    public static class SegmentParser
    {
        public static OperationResult<TreeNode> Parse(ParserProvider provider , Segment HEAD)
        {
            SegmentContext segmentContext = new SegmentContext(HEAD);
            var pr = provider.GetParser(ASTNodeType.Root);
            TreeNode root = new TreeNode
            {
                Type = ASTNodeType.Root
            };
            if (pr == null)
            {
                OperationResult<TreeNode> result = new OperationResult<TreeNode>(root);
                result.AddError(new ParserNotFoundError(HEAD, ASTNodeType.Root));
                return result;
            }
            OperationResult<TreeNode> _result = new OperationResult<TreeNode>(root);
            var r = pr.Parse(provider , segmentContext , root);
            if (r.Errors.Count > 0)
            {
                _result.Errors = r.Errors;
                return _result;
            }
            return root;

        }
    }
}
