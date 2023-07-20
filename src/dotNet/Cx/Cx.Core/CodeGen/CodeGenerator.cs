using Cx.Core.Analyzer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xCVM.Core.CompilerServices;

namespace Cx.Core.CodeGen
{
	public class CodeGenerator
	{
		public virtual OperationResult<bool> Write(GeneratorProvider provider , AnalyzedTreeNode node , StreamWriter writer)
		{
			return new OperationResult<bool>(false);
		}
		public OperationResult<bool> CheckThenRun(AnalyzedTreeNode node , int [ ] AcceptedIDs , Func<OperationResult<bool>> func)
		{
			bool Hit = false;
			foreach (var item in AcceptedIDs)
			{
				if (node.Type == item)
				{
					Hit = true;
					break;
				}
			}
			if (!Hit) { return false; }
			return func();
		}
	}
	public class ToASM_Scope_Generator : CodeGenerator
	{
		static readonly int [ ] types = new int [ ] { ASTNodeType.Scope };
		public override OperationResult<bool> Write(GeneratorProvider provider , AnalyzedTreeNode node , StreamWriter writer)
		{
			return CheckThenRun(node , types , () => { return false; });
		}
	}
	public class ToASM_RawAssembly_Generator : CodeGenerator
	{
		static readonly int [ ] types = new int [ ] { ASTNodeType.RawAssembly };
		public override OperationResult<bool> Write(GeneratorProvider provider , AnalyzedTreeNode node , StreamWriter writer)
		{
			return CheckThenRun(node , types , () => { return false; });
		}
	}
	public class ToASM_CallGenerator : CodeGenerator
	{
		static readonly int [ ] types = new int [ ] { ASTNodeType.Call };
		public override OperationResult<bool> Write(GeneratorProvider provider , AnalyzedTreeNode node , StreamWriter writer)
		{
			return CheckThenRun(node , types , () => { return false; });
		}
	}
	public class GeneratorProvider
	{

		internal Dictionary<int , CodeGenerator> generators = new Dictionary<int , CodeGenerator>();
		public void RegisterGenerator(int ID , CodeGenerator generator)
		{
			if (generators.ContainsKey(ID))
			{
				generators [ ID ] = generator;
			}
			else generators.Add(ID , generator);
		}
		public CodeGenerator? GetGenerator(int ID)
		{
			if (generators.ContainsKey(ID))
			{
				return generators [ ID ];
			}
			return null;
		}
	}
}
