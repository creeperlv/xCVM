using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class FuncAnalyzer : CAnalyzer
	{
		public FuncAnalyzer()
		{
			ConcernedAnalyzers = new List<int>
			{   ASTNodeType.DeclareVar,
				ASTNodeType.Scope,
				ASTNodeType.If,
				ASTNodeType.While,
				ASTNodeType.For,
				ASTNodeType.AssignedDeclareVariable,
			};
		}
		public override OperationResult<(bool, Symbol?)> BuildSymbolTable(AnalyzerProvider provider , int Pos , ref TreeNode node)
		{
			OperationResult<(bool, Symbol?)> FinalResult = (false, null);
			if (node.Type == ASTNodeType.DeclareFunc)
			{
				//It should be null at this point.
				if (node.Segment == null) return FinalResult;
				//A DeclareFunc Node cannot exist without a parent node.
				if (node.Parent== null) return FinalResult;
				Symbol symbol = new Symbol(node.Segment , node , Pos , SymbolType.Function);
				AnalyzedTreeNode analyzedTreeNode = AnalyzedTreeNode.FromTreeNode(node);
				node.Parent.ReplaceChild(node , node);
				node = analyzedTreeNode;
				analyzedTreeNode.table = new SymbolTable();
				List<CAnalyzer> CollectedAnalyzers = new List<CAnalyzer>();
				foreach (var item in ConcernedAnalyzers)
				{
					var p = provider.GetAnalyzer(item);
					if (p == null)
					{
						FinalResult.AddError(new AnalyzerNotFoundError(node.Segment , item));
						return FinalResult;
					}
					CollectedAnalyzers.Add(p);
				}
				int Position = 0;
				for (int i = 0 ; i < node.Children.Count ; i++)
				{
					TreeNode? item = node.Children [ i ];
					foreach (var analyzer in CollectedAnalyzers)
					{
						var AR = analyzer.BuildSymbolTable(provider , Position , ref item);
						if (FinalResult.CheckAndInheritAbnormalities(AR)) return FinalResult;
						if (AR.Result.Item1) break;
					}
					Position++;
				}
				FinalResult.Result.Item1 = true;
				FinalResult.Result.Item2 = symbol;
				return FinalResult;
			}
			else
			{
				return FinalResult;
			}

		}
	}
}
