using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class DeclareVarAnalyzer : CAnalyzer
	{
		public override OperationResult<bool> BuildSymbolTable(AnalyzerProvider provider , int Pos , SymbolTable ParentTable , ref TreeNode node)
		{
			OperationResult<bool> FinalResult = false;
			if (node.Type == ASTNodeType.DeclareVar)
			{
				//It should be null at this point.
				if (node.Segment == null) return FinalResult;
				FinalResult.Result = true;
				ParentTable.Add(new Symbol(node.Segment , node , Pos , (int)SymbolType.Variable));
				return FinalResult;
			}
			return FinalResult;
		}
	}
}
