using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class FuncAnalyzer : CAnalyzer
	{
		public FuncAnalyzer()
		{
			ConcernedSymbolTableBuildingAnalyzers = new List<int>
			{   ASTNodeType.DeclareVar,
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
			if (node.Type == ASTNodeType.DeclareFunc)
			{
				//It should be null at this point.
				if (node.Segment == null) return FinalResult;
				//A DeclareFunc Node cannot exist without a parent node.
				if (node.Parent == null) return FinalResult;
				Symbol symbol = new Symbol(node.Segment , node , Pos , (int)SymbolType.Function);
				AnalyzedTreeNode analyzedTreeNode = AnalyzedTreeNode.FromTreeNode(node);
				node.Parent.ReplaceChild(node , node);
				node = analyzedTreeNode;
				analyzedTreeNode.table = new SymbolTable();
				var CAUResult = CAnalyzerUtilities.SubAnalyze_BuildSymbolTable(ConcernedSymbolTableBuildingAnalyzers , provider , node, analyzedTreeNode.table);
				if (FinalResult.CheckAndInheritAbnormalities(CAUResult))
				{
					return FinalResult;
				}

				FinalResult.Result = true;
				ParentTable.Add(symbol);
				return FinalResult;
			}
			else
			{
				return FinalResult;
			}

		}
	}
}
