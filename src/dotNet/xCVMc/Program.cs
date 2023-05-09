using System;
using System.Text.Json;
using xCVM.Core;
using xCVM.Core.CompilerServices;
using xCVM.ShellUtilities;
using xCVMc.Data;

namespace xCVM.Compiler
{
    public class Program
    {
        public static void ShowVersion()
        {
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 0 ,
                Words = new List<Term> {
                    new Term { Content = "Extensible Common Virtual Machine Compiler" } }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 0 ,
                Words = new List<Term> {
                    new Term { Content = "CLI:" },
                    new Term { Content = typeof(Arguments).Assembly.GetName().Version+"", Color= ConsoleColor.Green },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 0 ,
                Words = new List<Term> {
                    new Term { Content = "xCVM.Core:" },
                    new Term { Content = typeof(xCVMCore).Assembly.GetName().Version+"", Color= ConsoleColor.Green },
                    new Term { Content = ""},
                }
            });
        }
        static void Help()
        {

            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 0 ,
                Words = new List<Term> {
                    new Term { Content = "Extensible Common Virtual Machine Resource Compiler v" },
                    new Term { Content = typeof(Arguments).Assembly.GetName().Version+"", Color= ConsoleColor.Green },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 0 ,
                PaddingTop = 1 ,
                PaddingBottom = 1 ,
                Words = new List<Term> {
                    new Term { Content = "Usage: " },
                    new Term { Content = "xCVMc [options...] input_file..." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 0 ,
                Words = new List<Term> {
                    new Term { Content = "Options:" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-o" },
                    new Term { Content = "|" },
                    new Term { Content = "--output" },
                    new Term { Content = "\t" },
                    new Term { Content = "<path-to-file>|null" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Specify where to write the produced file. Using null or not specifying will write the result to the standard output." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-V" },
                    new Term { Content = "\t" },
                    new Term { Content = "--version" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Print the version info." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-J" },
                    new Term { Content = "\t" },
                    new Term { Content = "--json" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Output as json." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-B" },
                    new Term { Content = "\t" },
                    new Term { Content = "--binary" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Output as binary. (Used by default)" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-H" },
                    new Term { Content = "\t" },
                    new Term { Content = "--hex" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Output as binary, in hex format." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-d" },
                    new Term { Content = "\t" },
                    new Term { Content = "--dev" },
                    new Term { Content = "\t" },
                    new Term { Content = "--dev-definition" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Will produce the development definition. It is enabled by default." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-D" },
                    new Term { Content = "|" },
                    new Term { Content = "--definition" },
                    new Term { Content = "\t" },
                    new Term { Content = "file_of_definition" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Use another xCVM assembler definition." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-R" },
                    new Term { Content = "|" },
                    new Term { Content = "--resource" },
                    new Term { Content = "\t" },
                    new Term { Content = "file_of_resource" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Specify which resource file to combine." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 1 ,
                PaddingTop = 1 ,
                Words = new List<Term> {
                    new Term { Content = "-E" },
                    new Term { Content = "|" },
                    new Term { Content = "--export-definition" },
                    new Term { Content = "\t" },
                    new Term { Content = "file_of_definition" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true ,
                Intend = 2 ,
                Words = new List<Term> {
                    new Term { Content = "Export current xCVM assembler definition." },
                }
            });
        }
        public static void Main(string [ ] args)
        {
            Arguments arguments = Arguments.FromStringArray(args);
            if (arguments.ShowHelp)
            {
                Help();
                return;
            }
            if (arguments.VersionInfo)
            {
                ShowVersion();
                return;
            }
            List<FileInfo> files = new List<FileInfo>();
            foreach (var item in arguments.Inputs)
            {
                files.Add(new FileInfo(item));
            }

            {
                xCVMAssembler xCVMAssembler;
                if (arguments.UseAlternativeDefinition == null
                    || arguments.UseAlternativeDefinition == ""
                    || arguments.UseAlternativeDefinition.ToUpper() == "DEFAULT")
                {
                    xCVMAssembler = new xCVMAssembler(PredefiniedAssemblerDefinition.GetDefinition());
                }
                else
                {

                    xCVMAssembler = new xCVMAssembler(JsonSerializer.Deserialize<AssemblerDefinition>(
                                                      File.ReadAllText(arguments.UseAlternativeDefinition) ,
                                                      SourceGenerationContext.Default.AssemblerDefinition));
                }
                if (arguments.OutDefinition != null)
                {
                    if (xCVMAssembler.AssemblerDefinition != null)
                    {
                        TextWriter textWriter = Console.Out;
                        Stream stream=null;
                        if (arguments.OutDefinition != "STDOUT" && arguments.OutDefinition != "null")
                        {
                            stream = File.OpenWrite(arguments.OutDefinition);
                            stream.SetLength(0);
                            textWriter = new StreamWriter(stream);
                        }
                        textWriter.Write(JsonSerializer.Serialize(xCVMAssembler.AssemblerDefinition , SourceGenerationContext.Default.AssemblerDefinition));
                        if (arguments.OutDefinition != "STDOUT" && arguments.OutDefinition != "null")
                        {
                            textWriter.Close();
                            stream.Close();
                        }
                    }
                    return;
                }
                xCVMModule? xCVMModule = null;
                var result = xCVMAssembler.Assemble(files);
                if (result.Errors.Count > 0)
                {
                    foreach (var err in result.Errors)
                    {
                        Console.Write("Error:");
                        Console.WriteLine(err.Message);
                        if (err.Segment is not null)
                        {
                            Console.Write("\tAt:");
                            Console.Write(err.Segment.ID);
                            Console.Write(":");
                            Console.Write(err.Segment.LineNumber);
                            Console.WriteLine();
                            Console.Write("\t\t-->");
                            Console.Write(err.Segment.content);
                            Console.WriteLine();
                        }
                    }
                    if (arguments.IgnoreError)
                    {
                        xCVMModule = result.Result;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    xCVMModule = result.Result;
                    if (arguments.ResourceFile != null)
                    {
                        var res = xCVMResource.FromStream(File.Open(arguments.ResourceFile , FileMode.Open));
                        xCVMModule.xCVMResource = res;
                    }
                }
                if (arguments.Output == "STDOUT" || arguments.Output == "null")
                {
                    switch (arguments.OutputType)
                    {
                        case OutputType.Binary:
                            {

                                var s = Console.OpenStandardOutput();
                                xCVMModule.WriteBrinary(s);
                                s.Flush();
                            }
                            break;
                        case OutputType.HEX:
                            {
                                var s = new HEXStream { UnderlyingWriter = Console.Out };
                                xCVMModule.WriteBrinary(s);
                                s.Flush();
                            }
                            break;
                        case OutputType.Json:
                            {
                                Console.Out.WriteLine(JsonSerializer.Serialize(xCVMModule , SourceGenerationContext.Default.xCVMModule));
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (arguments.OutputType)
                    {
                        case OutputType.Binary:
                            {
                                using (var s = File.Open(arguments.Output , FileMode.OpenOrCreate , FileAccess.Write , FileShare.Read))
                                {
                                    s.SetLength(0);
                                    xCVMModule.WriteBrinary(s);
                                    s.Flush();
                                }
                            }
                            break;

                        case OutputType.HEX:
                            {
                                using (var fs = File.OpenWrite(arguments.Output))
                                {
                                    try
                                    {
                                        fs.SetLength(0);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    using (var fw = new StreamWriter(fs))
                                    {
                                        using (var s = new HEXStream { UnderlyingWriter = fw })
                                        {

                                            xCVMModule.WriteBrinary(s);
                                            s.Flush();
                                        }
                                    }
                                }
                            }
                            break;
                        case OutputType.Json:
                            {
                                File.WriteAllText(arguments.Output , JsonSerializer.Serialize(xCVMModule , typeof(xCVMModule) , SourceGenerationContext.Default));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    public enum OutputType
    {
        Binary, Json, HEX
    }
    public class Arguments
    {
        public List<string> Inputs = new List<string>();
        public string Output = "a.out";
        public string UseAlternativeDefinition = "DEFAULT";
        public string OutDefinition = null;
        public string ResourceFile = null;
        public bool IgnoreError;
        public bool ShowHelp;
        public bool VersionInfo;
        public OutputType OutputType = OutputType.Binary;
        public static Arguments FromStringArray(string [ ] strings)
        {
            Arguments arguments = new Arguments();
            int mode = 0;
            for (int i = 0 ; i < strings.Length ; i++)
            {
                var item = strings [ i ];
                switch (item)
                {
                    case "-?":
                    case "-h":
                    case "-help":
                    case "--help":
                        {
                            arguments.ShowHelp = true;
                        }
                        break;
                    case "-o":
                        mode = 1;
                        break;
                    case "-s":
                        mode = 0;
                        break;
                    case "-D":
                    case "--definition":
                        mode = 2;
                        break;
                    case "-R":
                    case "--resource":
                        mode = 4;
                        break;
                    case "-E":
                    case "--export-definition":
                        mode = 3;
                        break;
                    case "-V":
                    case "--version":
                        {
                            arguments.VersionInfo = true;
                        }
                        break;
                    case "-J":
                    case "--json":
                        arguments.OutputType = OutputType.Json;
                        break;
                    case "-B":
                    case "--binary":
                        arguments.OutputType = OutputType.Binary;
                        break;
                    case "-H":
                    case "--hex":
                        arguments.OutputType = OutputType.HEX;
                        break;
                    default:
                        {
                            switch (mode)
                            {
                                case 0:
                                    arguments.Inputs.Add(item);
                                    break;
                                case 1:
                                    {
                                        arguments.Output = item;
                                        mode = 0;
                                    }
                                    break;
                                case 2:
                                    {
                                        arguments.UseAlternativeDefinition = item;
                                        mode = 0;
                                    }
                                    break;
                                case 3:
                                    {
                                        arguments.OutDefinition = item;
                                        mode = 0;
                                    }
                                    break;
                                case 4:
                                    {
                                        arguments.ResourceFile = item;
                                        mode = 0;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }
            }
            return arguments;
        }
    }
}