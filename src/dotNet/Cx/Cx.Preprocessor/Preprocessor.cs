using System;
using System.Collections.Generic;
using System.IO;
using Cx.Core;
using LibCLCC.NET.TextProcessing;
using xCVM.Core.CompilerServices;

namespace Cx.Preprocessor
{

    public class Preprocessor
    {
        FilesProvider FilesProvider;
        public Preprocessor(FilesProvider filesProvider)
        {
            FilesProvider = filesProvider;
        }
        CStyleParser CStyleParser = new CStyleParser();
        public Dictionary<string , string> Symbols = new Dictionary<string , string>();
        public void Define(string symbol , string value)
        {
            if (Symbols.ContainsKey(symbol)) { Symbols [ symbol ] = value; }
            else { Symbols.Add(symbol , value); }
        }
        public void Undefine(string symbol)
        {
            if (Symbols.ContainsKey(symbol)) Symbols.Remove(symbol);
        }
        public bool _if(string Condition)
        {

            return false;
        }
        public bool ifdef(string Condition)
        {
            return _if($"defined({Condition})");
        }
        public bool elif(string Condition)
        {
            return _if(Condition);
        }
        public bool defined(string name)
        {
            return Symbols.ContainsKey(name);
        }

        public VirtualFile Process(Stream Input , string Identifier)
        {
            VirtualFile VirtualFile = new VirtualFile();
            VirtualFile.ID = Identifier;
            VirtualFile.FileInMemory = new MemoryStream();
            StreamWriter sw = new StreamWriter(VirtualFile.FileInMemory);
            StreamReader streamReader = new StreamReader(VirtualFile.FileInMemory);
            string? Line = null;
            while (true)
            {
                Line = streamReader.ReadLine();
                if (Line == null) break;
                if (Line.StartsWith("#"))
                {
                    var LineParse = CStyleParser.Parse(Line [ 1.. ] , false);
                    SegmentContext segmentContext = new SegmentContext(LineParse);
                    var macro = segmentContext.MatchCollectionMarch(false , "include" , "define" , "if" , "undefine"
                        , "ifndef" , "ifdef" , "elif" , "endif" , "else");
                    if (macro.Item1 == MatchResult.Match)
                    {

                    }
                }
            }
            return VirtualFile;
        }
    }
}
