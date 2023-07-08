namespace Cx.Core.Analyzer
{
	public class VanillaCAnalyzers : AnalyzerProvider
	{
		public VanillaCAnalyzers()
		{
			analyzers.Add(ASTNodeType.Root , new RootSpaceAnalyzer());
		}

	}
}
