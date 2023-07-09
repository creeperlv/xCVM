using Cx.Core;
using Cx.Core.Analyzer;
using System;
using System.Collections.Generic;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.HL2VC.Analyzers
{
	public class NamespaceAnalyzer:CAnalyzer
	{
		public NamespaceAnalyzer()
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
			return base.BuildSymbolTable(provider , Pos , ParentTable , ref node);
		}
	}
}
