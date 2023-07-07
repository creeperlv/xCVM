﻿using System.Collections.Generic;
using xCVM.Core.CompilerServices;

namespace Cx.Core.Analyzer
{
	public class DeclareVarAnalyzer : CAnalyzer
	{
		public override OperationResult<(bool, Symbol?)> BuildSymbolTable(AnalyzerProvider provider , int Pos , ref TreeNode node)
		{
			OperationResult<(bool, Symbol?)> FinalResult = (false, null);
			if (node.Type == ASTNodeType.DeclareVar)
			{
				//It should be null at this point.
				if (node.Segment == null) return FinalResult;
				FinalResult.Result.Item1 = true;
				FinalResult.Result.Item2 = new Symbol(node.Segment , node , Pos , SymbolType.Variable);
				return FinalResult;
			}
			return FinalResult;
		}
	}
	public class IfAnalyzer : CAnalyzer
	{

	}
	public class ScopeAnalyzer : CAnalyzer
	{
		public ScopeAnalyzer()
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
			if (node.Type == ASTNodeType.Scope)
			{

				//A DeclareFunc Node cannot exist without a parent node.
				if (node.Parent == null) return FinalResult;
				Symbol symbol = new Symbol(node.Segment , node , Pos , SymbolType.SubSymbolTable);
				AnalyzedTreeNode analyzedTreeNode = AnalyzedTreeNode.FromTreeNode(node);
				node.Parent = analyzedTreeNode;
				node = analyzedTreeNode;
				SymbolTable table = new SymbolTable();
				analyzedTreeNode.table = table;
				var CAUResult=CAnalyzerUtilities.SubAnalyze_BuildSymbolTable(ConcernedAnalyzers , provider , node,table);
				if (FinalResult.CheckAndInheritAbnormalities(CAUResult)) return FinalResult;
				FinalResult.Result.Item1 = true;
				FinalResult.Result.Item2 = symbol;
			}
			return FinalResult;
		}
	}
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
				if (node.Parent == null) return FinalResult;
				Symbol symbol = new Symbol(node.Segment , node , Pos , SymbolType.Function);
				AnalyzedTreeNode analyzedTreeNode = AnalyzedTreeNode.FromTreeNode(node);
				node.Parent.ReplaceChild(node , node);
				node = analyzedTreeNode;
				analyzedTreeNode.table = new SymbolTable();
				var CAUResult = CAnalyzerUtilities.SubAnalyze_BuildSymbolTable(ConcernedAnalyzers , provider , node, analyzedTreeNode.table);
				if (FinalResult.CheckAndInheritAbnormalities(CAUResult))
				{
					return FinalResult;
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
