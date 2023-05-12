﻿using System;
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
        FilesProvider ProcessedFile;
        bool ProcessIntoMemory;
        public Preprocessor(FilesProvider filesProvider)
        {
            FilesProvider = filesProvider;
            ProcessedFile = new FilesProvider();
        }
        CStyleParser CStyleParser = new CStyleParser();
        public Dictionary<string , string> Symbols = new Dictionary<string , string>();
        public void Define(string symbol , string value)
        {
            if (Symbols.ContainsKey(symbol)) { Symbols [ symbol ] = value; }
            else { Symbols.Add(symbol , value); }
        }
        public void define(SegmentContext context)
        {
            var key = context.Current?.content ?? "";
            var value = context.Current?.Next?.content ?? "";
            Define(key , value);
        }
        public void Undefine(string symbol)
        {
            if (Symbols.ContainsKey(symbol)) Symbols.Remove(symbol);
        }
        public bool _if(SegmentContext context)
        {

            return false;
        }
        public bool ifdef(SegmentContext context)
        {
            return defined(context.Current?.content ?? "");
        }
        public bool elif(SegmentContext context)
        {
            return _if(context);
        }
        public bool defined(string name)
        {
            return Symbols.ContainsKey(name);
        }
        public OperationResult<Preprocessed?> Process(VirtualFile InputCFile)
        {
            Preprocessed preprocessed = new Preprocessed();
            Process(InputCFile.GetStream() , InputCFile.ID , preprocessed , false);
            return new OperationResult<Preprocessed?>(null);
        }
        public VirtualFile Process(Stream Input , string Identifier , Preprocessed preprocessed , bool isHeader)
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
                        , "ifndef" , "ifdef" , "elif" , "endif" , "else" , "pragma");
                    if (macro.Item1 == MatchResult.Match)
                    {
                        switch (macro.Item2)
                        {
                            case 1:
                                {
                                    var m =segmentContext.MatchNext("<" , true);
                                    if(m== MatchResult.Match)
                                    {
                                        var f=FilesProvider.Find(segmentContext.Current?.content??"");
                                        if (f != null)
                                        {
                                            var vf=Process(f.GetStream() , f.ID , preprocessed , true);
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return VirtualFile;
        }
    }
}
