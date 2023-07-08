using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class CAnalyzer
	{
		public List<int> ConcernedSymbolTableBuildingAnalyzers = new List<int>();
		public virtual OperationResult<bool> BuildSymbolTable(AnalyzerProvider provider , int Pos ,SymbolTable ParentTable, ref TreeNode node)
		{
			return false;
		}
	}
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
	public class AnalyzedTreeNode : TreeNode
	{
		public SymbolTable? table = null;
		public static AnalyzedTreeNode FromTreeNode(TreeNode node)
		{
			AnalyzedTreeNode analyzedTreeNode = new AnalyzedTreeNode();
			analyzedTreeNode.Type = node.Type;
			analyzedTreeNode.Segment = node.Segment;
			analyzedTreeNode.table = null;
			return analyzedTreeNode;

		}
	}
	public enum SymbolType
	{
		Function, Variable, TypeDefinition, StructDeclaration,SubSymbolTable
	}
	public class Symbol
	{
		public Segment? Name;
		public TreeNode ReferredNode;
		public int Position;
		public SymbolType symbolType;
		public Symbol(Segment? name , TreeNode referredNode , int position , SymbolType symbolType)
		{
			Name = name;
			ReferredNode = referredNode;
			Position = position;
			this.symbolType = symbolType;
		}
	}
	public class SymbolTable
	{
		List<Symbol> Symbols = new List<Symbol>();
		public void Add(Symbol symbol)
		{
			Symbols.Add(symbol);
		}
	}
}
