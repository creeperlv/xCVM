﻿using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class RootSpaceAnalyzer : CAnalyzer
	{
		public RootSpaceAnalyzer()
		{
			ConcernedSymbolTableBuildingAnalyzers = new List<int>
			{
				ASTNodeType.DeclareFunc,
				ASTNodeType.DeclareVar,
				ASTNodeType.Extern,
				ASTNodeType.DeclareStruct,
				ASTNodeType.TypeDef
			};
		}
		public override OperationResult<bool> BuildSymbolTable(AnalyzerProvider provider , int Pos , SymbolTable ParentTable , ref TreeNode node)
		{
			OperationResult<bool> FinalResult = false;
			List<CAnalyzer> CollectedAnalyzers = new List<CAnalyzer>();
			foreach (var item in ConcernedSymbolTableBuildingAnalyzers)
			{
				var p = provider.GetAnalyzer(item);
				if (p == null)
				{
					FinalResult.AddError(new AnalyzerNotFoundError(node.Segment , item));
					return FinalResult;
				}
				CollectedAnalyzers.Add(p);
			}
			AnalyzedTreeNode analyzedTreeNode = new AnalyzedTreeNode();
			analyzedTreeNode.Type = node.Type;
			analyzedTreeNode.Segment = node.Segment;
			var table = new SymbolTable();
			analyzedTreeNode.table = table;
			if (node.Type != ASTNodeType.Root) return FinalResult;
			int Position = 0;
			for (int i = 0 ; i < node.Children.Count ; i++)
			{
				TreeNode? item = node.Children [ i ];
				foreach (var analyzer in CollectedAnalyzers)
				{
					var AR = analyzer.BuildSymbolTable(provider , Position , table,ref item);
					if (FinalResult.CheckAndInheritAbnormalities(AR)) return FinalResult;
					if (AR.Result) break;
				}
				Position++;
			}
			FinalResult.Result = true;
			return FinalResult;
		}
	}
}