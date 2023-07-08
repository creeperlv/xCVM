using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class ExpressionAnalyzer : CAnalyzer
	{
		public ExpressionAnalyzer()
		{
			ConcernedSymbolTableBuildingAnalyzers = new List<int>
			{   ASTNodeType.DeclareVar,
				ASTNodeType.Expression,
				ASTNodeType.BinaryExpression,
				ASTNodeType.UnaryExpression,
			};
		}
		public override OperationResult<bool> BuildSymbolTable(AnalyzerProvider provider , int Pos , SymbolTable ParentTable , ref TreeNode node)
		{
			OperationResult<bool> FinalResult = false;
			if(node.Type != ASTNodeType.Expression)
			{
				return FinalResult;
			}
			CAnalyzerUtilities.SubAnalyze_BuildSymbolTable(ConcernedSymbolTableBuildingAnalyzers , provider , node , ParentTable);
			return FinalResult;
		}
	}
}
