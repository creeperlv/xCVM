using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class ScopeAnalyzer : CAnalyzer
	{
		public ScopeAnalyzer()
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
		public override OperationResult<bool> BuildSymbolTable(AnalyzerProvider provider , int Pos , SymbolTable ParentTable , ref TreeNode node)
		{
			OperationResult<bool> FinalResult = false;
			if (node.Type == ASTNodeType.Scope)
			{

				//A DeclareFunc Node cannot exist without a parent node.
				if (node.Parent == null) return FinalResult;
				Symbol symbol = new Symbol(node.Segment , node , Pos , (int)SymbolType.SubSymbolTable);
				AnalyzedTreeNode analyzedTreeNode = AnalyzedTreeNode.FromTreeNode(node);
				node.Parent = analyzedTreeNode;
				node = analyzedTreeNode;
				SymbolTable table = new SymbolTable();
				analyzedTreeNode.table = table;
				var CAUResult=CAnalyzerUtilities.SubAnalyze_BuildSymbolTable(ConcernedSymbolTableBuildingAnalyzers , provider , node,table);
				if (FinalResult.CheckAndInheritAbnormalities(CAUResult)) return FinalResult;
				FinalResult.Result = true;
				ParentTable.Add(symbol);
			}
			return FinalResult;
		}
	}
}
