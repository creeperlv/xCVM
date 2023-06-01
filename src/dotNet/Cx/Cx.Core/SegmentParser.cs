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
        public static OperationResult<ASTNode> Parse(ParserProvider provider , FileInfo InputFile)
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
        public static OperationResult<ASTNode> Parse(ParserProvider provider , Segment HEAD)
        {
            SegmentContext segmentContext = new SegmentContext(HEAD);
            var pr = provider.GetParser(ASTNodeType.Root);
            ASTNode root = new ASTNode
            {
                Type = ASTNodeType.Root
            };
            if (pr == null)
            {
                OperationResult<ASTNode> result = new OperationResult<ASTNode>(root);
                result.AddError(new ParserNotFoundError(HEAD));
                return result;
            }
            OperationResult<ASTNode> _result = new OperationResult<ASTNode>(root);
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
