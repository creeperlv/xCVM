using System.Collections.Generic;

namespace Cx.Core.Analyzer
{
	public class AnalyzerProvider
	{
		public Dictionary<int , CAnalyzer> analyzers = new Dictionary<int , CAnalyzer>();
		public void RegisterAnalyzer(int ID , CAnalyzer parser)
		{
			if (analyzers.ContainsKey(ID))
			{
				analyzers [ ID ] = parser;
			}
			else analyzers.Add(ID , parser);
		}
		public CAnalyzer? GetAnalyzer(int ID)
		{
			if (analyzers.ContainsKey(ID))
			{
				return analyzers [ ID ];
			}
			return null;
		}
	}
}
