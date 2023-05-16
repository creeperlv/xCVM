using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
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
            Process(InputCFile , preprocessed , false);
            return new OperationResult<Preprocessed?>(null);
        }
        public VirtualFile Process(VirtualFile Input , Preprocessed preprocessed , bool isHeader)
        {
            VirtualFile VirtualFile = new VirtualFile(Input.ID);
            VirtualFile.FileInMemory = new MemoryStream();
            StreamWriter sw = new StreamWriter(VirtualFile.GetStream());
            StreamReader streamReader = new StreamReader(Input.GetStream());
            string? Line = null;
            int IFSCOPE;
            bool willskip = false;
            if (isHeader)
            {
                preprocessed.ProcessedHeader.Add(Input.ID , Input);
            }
            while (true)
            {
                Line = streamReader.ReadLine();
                if (Line.Trim().EndsWith('\\'))
                {
                    while (true)
                    {
                        var NLine = streamReader.ReadLine();
                        if (NLine == null)
                        {
                            break;
                        }
                        Line = Line.Substring(0 , Line.Length - 1);
                        Line += "\n";
                        Line += NLine;
                        if (!NLine.Trim().EndsWith('\\'))
                        {
                            break;
                        }
                    }
                }
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
                            case 0:
                                {
                                    if (!willskip)
                                    {
                                        var m = segmentContext.MatchNext("<" , true);
                                        if (m == MatchResult.Match)
                                        {
                                            var f = FilesProvider.Find(segmentContext.Current?.content ?? "");
                                            if (f != null)
                                            {
                                                var vf = Process(f , preprocessed , true);
                                            }
                                        }

                                    }
                                }
                                break;
                            case 1:
                                {
                                    define(segmentContext);
                                }
                                break;
                            case 2:
                                {
                                    willskip = _if(segmentContext);
                                    IFSCOPE++;
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
