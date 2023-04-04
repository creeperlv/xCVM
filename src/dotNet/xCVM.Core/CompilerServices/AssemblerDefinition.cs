using System;
using System.Collections.Generic;

namespace xCVM.Core.CompilerServices
{
    [Serializable]
    public class AssemblerDefinition
    {
        public bool UseStatementEndMark=true;
        public string StateMentEndMark = ";";
        public Dictionary<string,string> PredefinedSymbols = new Dictionary<string,string>();
        public List<Instruction3OperatorsDefinition> Definitions = new List<Instruction3OperatorsDefinition>();
    }
}
