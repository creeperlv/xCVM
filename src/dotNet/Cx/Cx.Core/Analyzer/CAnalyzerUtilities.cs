using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class CAnalyzerUtilities
	{
		public static OperationResult<bool> SubAnalyze_BuildSymbolTable(List<int> ConcernedAnalyzers , AnalyzerProvider provider , TreeNode node , SymbolTable table)
		{
			OperationResult<bool> FinalResult = false;
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
					if (AR.Result.Item1)
					{
						if (AR.Result.Item2 != null)
						{
							table.Add(AR.Result.Item2);
							break;
						}
					}
				}
				Position++;
			}
			FinalResult.Result = true;
			return FinalResult;
		}
	}
}
