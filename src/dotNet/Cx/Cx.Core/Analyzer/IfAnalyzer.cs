using System.Collections.Generic;

namespace Cx.Core.Analyzer
{
	public class IfAnalyzer : CAnalyzer
	{
		public IfAnalyzer()
		{
			ConcernedSymbolTableBuildingAnalyzers = new List<int>
			{   ASTNodeType.DeclareVar,
				ASTNodeType.Expression,
				ASTNodeType.Scope,
				ASTNodeType.If,
				ASTNodeType.While,
				ASTNodeType.For,
				ASTNodeType.AssignedDeclareVariable,
			};
		}
	}
}
