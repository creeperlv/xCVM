using System;
using System.Collections.Generic;

namespace xCVM.Core.CompilerServices
{
    [Serializable]
    public class AssemblerDefinition
    {
        public bool CaseSensitiveSegmentationIdentifiers=false;
        public bool CaseSensitiveInstructions=false;
        public bool AcceptIDAlias=true;
        public bool UseStatementEndMark=true;
        public string StateMentEndMark = ";";
        public Dictionary<string,string> PredefinedSymbols = new Dictionary<string,string>();
        public List<Instruction3OperatorsDefinition> Definitions = new List<Instruction3OperatorsDefinition>();
    }
}
