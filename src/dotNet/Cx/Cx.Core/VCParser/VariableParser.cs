using Cx.Core.DataTools;
using xCVM.Core.CompilerServices;

namespace Cx.Core.VCParser
{
	/// <summary>
	/// Ends after the final part. Like A.B+3, ends at `+`.
	/// </summary>
	public class VariableParser : ContextualParser
	{
		public override OperationResult<bool> Parse(ParserProvider provider , SegmentContext context , TreeNode Parent)
		{
			OperationResult<bool> FinalResult = false;
			TreeNode treeNode = new TreeNode();
			if (context.ReachEnd) return FinalResult;
			while (true)
			{
				if (context.ReachEnd)
				{
					Parent.AddChild(treeNode);
					break;
				}
				var current_content = context.Current?.content ?? "";
				if (DataTypeChecker.DetermineDataType(current_content) == DataType.String)
				{
					if (treeNode.Children.Count == 0)
					{
						treeNode.Type = ASTNodeType.Variable;
					}
					treeNode.Segment = context.Current;
				}
				else if (DataTypeChecker.DetermineDataType(current_content) == DataType.Symbol)
				{
					switch (current_content)
					{
						case ".":
							{
								TreeNode node = new TreeNode();
								node.Type = ASTNodeType.FieldInStruct;
								node.AddChild(treeNode);
								treeNode = node;
							}
							break;
						case "->":
							{
								TreeNode node = new TreeNode();
								node.Type = ASTNodeType.FieldInPointer;
								node.AddChild(treeNode);
								treeNode = node;
							}
							break;
						default:
							{
								Parent.AddChild(treeNode);
								FinalResult = true;
								return FinalResult;
							}
					}
				}
				else
				{
					Parent.AddChild(treeNode);
					FinalResult = true;
					return FinalResult;
				}
				context.GoNext();
			}
			return FinalResult;
		}
	}
}