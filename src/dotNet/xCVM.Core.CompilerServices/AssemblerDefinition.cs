﻿using System;
using System.Collections.Generic;

namespace xCVM.Core.CompilerServices
{
    [Serializable]
    public class AssemblerDefinition
    {
        public bool CaseSensitiveSegmentationIdentifiers = false;
        public bool CaseSensitiveInstructions = false;
        public bool CaseSensitiveExternIdentifiers = false;
        public bool CaseSensitiveReturnIdentifier = false;
        public bool CaseSensitiveTypeMapping = true;
        public bool CaseSensitiveInternalTypeIdentifier = true;
        public bool AcceptIDAlias = true;
        public bool UseStatementEndMark = true;
        public string StatementEndMark = ";";
        public bool UseExternStartMark = true;
        public string ExternStartMark = ":";
        public string FunctionIdentifier = "fn";
        public string StructIdentifier = "st";
        public string RuntimeStructIdentifier = "rst";
        public string ReturnIdentifier = "return";
        public string InternalTypeIdentifier = "internal";
        public List<string> StructIdentifiers =new List<string> { "struct" , "st" , "structure" };
        public Dictionary<string,int> PredefinedTypeMapping = new Dictionary<string,int>();
        public Dictionary<string, string> PredefinedSymbols = new Dictionary<string, string>();
        public List<InstructionDefinition> Definitions = new List<InstructionDefinition>();
    }
}
