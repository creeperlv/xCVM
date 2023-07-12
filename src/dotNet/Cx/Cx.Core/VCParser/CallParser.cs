using Cx.Core.SegmentContextUtilities;
using System;
using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
	public class RawAssemblyParser : ContextualParser
	{
		public static string[] AssemblyFunctions = new string [ ] { "asm" };
		public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
		{
			OperationResult<bool> FinalResult = false;
			var HEAD = context.Current;
			if (HEAD == null)
			{
				FinalResult.Result = false;
				FinalResult.AddError(new UnexpectedEndOfFileError(HEAD));
				return FinalResult;
			}
			var asmmatch=context.MatchCollection(false,AssemblyFunctions);
			if(asmmatch.Item1!= MatchResult.Match)
			{
				return false;
			}
			context.GoNext();
			var LR = context.Current;
			if (LR == null)
			{
				FinalResult.Result = false;
				FinalResult.AddError(new UnexpectedEndOfFileError(HEAD));
				return FinalResult;
			}
			if (LR.content == "(")
			{
				var Arguments = ContextClosure.LRClose(context , "(" , ")");
				if (Arguments.Errors.Count > 0)
				{
					FinalResult.Errors = Arguments.Errors;
					return FinalResult;
				}
				if (Arguments.Result == null)
				{
					return FinalResult;
				}
				TreeNode node = new TreeNode();
				node.Type = ASTNodeType.RawAssembly;
				node.Segment = HEAD;
				var _context = Arguments.Result;
				while (true)
				{
					if (_context.ReachEnd)
					{
						break;
					}
					if (_context.Current == _context.EndPoint)
					{
						break;
					}
					var Current = _context.Current;
					if (Current == null)
					{
						break;
					}

				}
				context.SetCurrent(Arguments.Result.EndPoint);
			}
			else
			{

			}
			context.SetCurrent(HEAD);
			return FinalResult;

		}
	}
	public class CallParser : ContextualParser
	{
		public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
		{
			OperationResult<bool> FinalResult = false;
			var HEAD = context.Current;
			if (HEAD == null)
			{
				FinalResult.Result = false;
				FinalResult.AddError(new UnexpectedEndOfFileError(HEAD));
				return FinalResult;
			}
			context.GoNext();
			var LR = context.Current;
			if (LR == null)
			{
				FinalResult.Result = false;
				FinalResult.AddError(new UnexpectedEndOfFileError(HEAD));
				return FinalResult;
			}
			if (LR.content == "(")
			{
				var Arguments = ContextClosure.LRClose(context , "(" , ")");
				if (Arguments.Errors.Count > 0)
				{
					FinalResult.Errors = Arguments.Errors;
					return FinalResult;
				}
				if (Arguments.Result == null)
				{
					return FinalResult;
				}
				TreeNode node = new TreeNode();
				node.Type = ASTNodeType.Call;
				node.Segment = HEAD;
				var _context = Arguments.Result;
				while (true)
				{
					if (_context.ReachEnd)
					{
						break;
					}
					if (_context.Current == _context.EndPoint)
					{
						break;
					}
					var Current = _context.Current;
					if (Current == null)
					{
						break;
					}

				}
				context.SetCurrent(Arguments.Result.EndPoint);
			}
			else
			{

			}
			context.SetCurrent(HEAD);
			return FinalResult;

		}
	}
}