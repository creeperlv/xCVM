using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class RootSpaceAnalyzer : CAnalyzer
	{
		public RootSpaceAnalyzer()
		{
			ConcernedAnalyzers = new List<int>
			{
				ASTNodeType.DeclareFunc,
				ASTNodeType.DeclareVar,
				ASTNodeType.Extern,
				ASTNodeType.DeclareStruct,
				ASTNodeType.TypeDef
			};
		}
		public override OperationResult<(bool, Symbol?)> BuildSymbolTable(AnalyzerProvider provider , int Pos , ref TreeNode node)
		{
			OperationResult<(bool, Symbol?)> FinalResult = (false, null);
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
			AnalyzedTreeNode analyzedTreeNode = new AnalyzedTreeNode();
			analyzedTreeNode.Type = node.Type;
			analyzedTreeNode.Segment = node.Segment;
			analyzedTreeNode.table = new SymbolTable();
			if (node.Type != ASTNodeType.Root) return FinalResult;
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
			return FinalResult;
		}
	}

	public class FuncAnalyzer : CAnalyzer
	{
		public FuncAnalyzer()
		{
			ConcernedAnalyzers = new List<int>
			{	ASTNodeType.DeclareVar,
				ASTNodeType.Scope,
				ASTNodeType.If,
				ASTNodeType.While,
				ASTNodeType.For,
				ASTNodeType.AssignedDeclareVariable,
			};
		}
	}
}
