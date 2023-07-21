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
		public override OperationResult<bool> ReferenceAnalyze(AnalyzerProvider provider , int Pos , AnalyzedTreeNode node)
		{
			OperationResult<bool> FinalResult = false;
			if (node.Type != ASTNodeType.Expression)
			{
				return FinalResult;
			}
			return FinalResult;
		}
		public override OperationResult<bool> BuildSymbolTable(AnalyzerProvider provider , int Pos , SymbolTable ParentTable , ref TreeNode node)
		{
			OperationResult<bool> FinalResult = false;
			if(node.Type != ASTNodeType.Expression)
			{
				return FinalResult;
			}
			var st=CAnalyzerUtilities.SubAnalyze_BuildSymbolTable(ConcernedSymbolTableBuildingAnalyzers , provider , node , ParentTable);
			if (FinalResult.CheckAndInheritAbnormalities(st))
			{
				return FinalResult;
			}
			FinalResult.Result = true;
			return FinalResult;
		}
	}
}
