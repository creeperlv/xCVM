using System;
using System.Collections.Generic;

namespace xCVM.Core.CompilerServices
{
    [Serializable]
    public class AssemblerDefinition
    {
        public bool CaseSensitiveSegmentationIdentifiers = false;
        public bool CaseSensitiveInstructions = false;
        public bool CaseSensitiveExternIdentifiers = false;
        public bool AcceptIDAlias = true;
        public bool UseStatementEndMark = true;
        public string StatementEndMark = ";";
        public bool UseExternStartMark = true;
        public string ExternStartMark = ":";
        public string FunctionIdentifier = "fn";
        public string StructIdentifier = "st";
        public Dictionary<string, string> PredefinedSymbols = new Dictionary<string, string>();
        public List<InstructionDefinition> Definitions = new List<InstructionDefinition>();
    }
}
