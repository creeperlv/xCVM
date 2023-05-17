using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
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
            var value = context.FormTillEnd(false , ' ').Trim();
            Define(key , value);
        }
        public void Undefine(SegmentContext context)
        {
            var key = context.Current?.content ?? "";
            if (Symbols.ContainsKey(key)) Symbols.Remove(key);
        }
        public ASTNode ParseEval(SegmentContext context)
        {
            SegmentContext segmentContext = new SegmentContext(context.Current);
            while (true)
            {
                if (segmentContext.Current.content == "&&")
                {

                }
            }
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
        public OperationResult<Preprocessed?> Process(VirtualFile InputCFile , Preprocessed? predprepro = null)
        {
            Preprocessed preprocessed;
            if (predprepro != null)
            {
                preprocessed = predprepro;
            }
            else { preprocessed = new Preprocessed(); }
            Process(InputCFile , preprocessed , false);
            return new OperationResult<Preprocessed?>(null);
        }
        public string process_line(string Line)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var LineParse = CStyleParser.Parse(Line [ 1.. ] , false);
            SegmentContext segmentContext = new SegmentContext(LineParse);
            while (true)
            {
                if (segmentContext.ReachEnd)
                {
                    break;
                }
                if (segmentContext.Current == null)
                {
                    break;
                }
                if (segmentContext.Current.content == "" && segmentContext.Current.Next == null)
                {
                    break;
                }
                var item = segmentContext.Current.content;
                stringBuilder.Append(" ");
                if (segmentContext.Current.isEncapsulated)
                {
                    stringBuilder.Append(segmentContext.Current.EncapsulationIdentifier);
                    stringBuilder.Append(item);
                    stringBuilder.Append(segmentContext.Current.EncapsulationIdentifier);
                }
                else
                {

                    if (Symbols.ContainsKey(item))
                    {
                        stringBuilder.Append(Symbols [ item ]);

                    }
                    else
                    {
                        stringBuilder.Append(item);
                    }
                }
            }
            return stringBuilder.ToString();
        }
        public OperationResult<string?> process_macro_line(string Line , ref bool willskip , ref Preprocessed preprocessed , ref int IFSCOPE)
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
                                        Process(f , preprocessed , true);
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
                    case 3:
                        {
                            Undefine(segmentContext);
                        }
                        break;
                    default:
                        break;
                }
            }
            return new OperationResult<string?>(null);
        }
        public void Process(VirtualFile Input , Preprocessed preprocessed , bool isHeader)
        {
            StreamWriter sw;
            VirtualFile VirtualFile;
            if (!isHeader)
            {
                if (preprocessed.ProcessedCFile == null)
                {
                    VirtualFile = new VirtualFile(Input.ID);
                    VirtualFile.FileInMemory = new MemoryStream();
                    preprocessed.ProcessedCFile = VirtualFile;
                }
                else
                {
                    VirtualFile = preprocessed.ProcessedCFile;
                }
            }
            else
            {
                if (preprocessed.CombinedHeader != null)
                {
                    VirtualFile = preprocessed.CombinedHeader;
                }
                else
                {
                    VirtualFile = new VirtualFile(Input.ID);
                    VirtualFile.FileInMemory = new MemoryStream();
                    preprocessed.CombinedHeader = VirtualFile;
                }
            }
            sw = new StreamWriter(VirtualFile.GetStream());
            StreamReader streamReader = new StreamReader(Input.GetStream());
            string? Line = null;
            int IFSCOPE = 0;
            bool willskip = false;
            if (isHeader)
            {
                preprocessed.ProcessedHeader.Add(Input.ID , Input);
            }
            while (true)
            {
                Line = streamReader.ReadLine().Trim();
                if (Line.EndsWith('\\'))
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
                    var preprocessed_line = process_macro_line(Line , ref willskip , ref preprocessed , ref IFSCOPE);
                    if (preprocessed_line != null)
                    {
                        sw.WriteLine(preprocessed_line.Result);
                    }
                }
                else
                {
                    if (!willskip)
                    {
                        var preprocessed_line = process_line(Line);
                        sw.WriteLine(preprocessed_line);
                    }
                }
            }
            return;
        }
    }
}
