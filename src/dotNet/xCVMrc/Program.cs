using System.Linq.Expressions;
using xCVM.Core;
using xCVM.Core.CompilerServices;
using xCVM.ShellUtilities;

namespace xCVMrc
{
    internal class Program
    {
        public static void ShowVersion()
        {
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 0,
                Words = new List<Term> {
                    new Term { Content = "Extensible Common Virtual Machine Resource Compiler" } }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 0,
                Words = new List<Term> {
                    new Term { Content = "CLI:" },
                    new Term { Content = typeof(Arguments).Assembly.GetName().Version+"", Color= ConsoleColor.Green },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 0,
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
                EndWithNewLine = true,
                Intend = 0,
                Words = new List<Term> {
                    new Term { Content = "Extensible Common Virtual Machine Resource Compiler v" },
                    new Term { Content = typeof(Arguments).Assembly.GetName().Version+"", Color= ConsoleColor.Green },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 0,
                PaddingTop = 1,
                PaddingBottom = 1,
                Words = new List<Term> {
                    new Term { Content = "Usage: " },
                    new Term { Content = "xCVMrc [options...] input_file..." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 0,
                Words = new List<Term> {
                    new Term { Content = "Options:" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 1,
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
                EndWithNewLine = true,
                Intend = 2,
                Words = new List<Term> {
                    new Term { Content = "Specify where to write the produced file. Using null or not specifying will write the result to the standard output." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 1,
                Words = new List<Term> {
                    new Term { Content = "-V" },
                    new Term { Content = "\t" },
                    new Term { Content = "--version" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 2,
                Words = new List<Term> {
                    new Term { Content = "Print the version info." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 1,
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
                EndWithNewLine = true,
                Intend = 2,
                Words = new List<Term> {
                    new Term { Content = "Will produce the development definition. It is enabled by default." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 1,
                Words = new List<Term> {
                    new Term { Content = "-D" },
                    new Term { Content = "\t" },
                    new Term { Content = "--no-dev" },
                    new Term { Content = "\t" },
                    new Term { Content = "--no-dev-definition" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 2,
                Words = new List<Term> {
                    new Term { Content = "Will not produce the development definition." },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 1,
                Words = new List<Term> {
                    new Term { Content = "-OD" },
                    new Term { Content = "\t" },
                    new Term { Content = "--only-definition" },
                }
            });
            Shell.Say(new Sentence
            {
                EndWithNewLine = true,
                Intend = 2,
                Words = new List<Term> {
                    new Term { Content = "Will only produce the development definition." },
                }
            });
        }
        static void Main(string[] args)
        {
            Arguments arguments = Arguments.FromStringArray(args);
            if (arguments.help)
            {
                Help();
                return;
            }
            if (arguments.VersionInfo)
            {
                ShowVersion();
                return;
            }
            ResourceCompilerOptions options = new ResourceCompilerOptions();
            if (arguments.Output == "null")
            {
                options.CompileToMemory = true;
                options.Destination = null;
            }
            else
            {
                options.CompileToMemory = false;
                options.Destination = arguments.Output;
            }
            ResourceCompiler resourceCompiler = new ResourceCompiler();
            List<FileInfo> files = new List<FileInfo>();
            foreach (var file in arguments.Inputs)
            {
                files.Add(new FileInfo(file));
            }
            if (arguments.OnlyDevDefinition)
            {
                var result = resourceCompiler.CompileDevDefinition(options, files.ToArray());
                if (arguments.Output == "null")
                {
                    using (var stream = Console.OpenStandardOutput())
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            result.Result.WriteWriter(writer);
                        }
                    }

                }
                else
                    result.Result.WriteToFile(arguments.Output);
            }
            else
            {
                var result = resourceCompiler.Compile(options, files.ToArray());
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

                }
                else
                {

                    if (arguments.Output == "null")
                    {
                        if (result.Result != null)
                        {
                            if (result.Result.resource != null)
                            {
                                if (result.Result.resource.xCVMResource != null)
                                {
                                    result.Result.resource.xCVMResource.WriteToStream(Console.OpenStandardOutput());
                                }
                            }
                        }
                    }
                    else
                    {
                        if (result.Result != null)
                        {
                            if (result.Result.resource != null)
                            {
                                if (result.Result.Definition != null)
                                {
                                    if (arguments.OutDevDefinition == true)
                                        result.Result.Definition.WriteToFile(arguments.Output + ".def");
                                }
                            }
                        }
                    }
                }

            }
        }
    }
    public class Arguments
    {
        public List<string> Inputs = new List<string>();
        public string Output = "null";
        public bool VersionInfo;
        public bool help = false;
        public bool OutDevDefinition = true;
        public bool OnlyDevDefinition = false;
        public static Arguments FromStringArray(string[] strings)
        {
            Arguments arguments = new Arguments();
            int mode = 0;
            for (int i = 0; i < strings.Length; i++)
            {
                var item = strings[i];
                switch (item)
                {
                    case "-?":
                    case "-h":
                    case "-help":
                    case "--help":
                        {
                            arguments.help = true;
                        }
                        break;
                    case "-o":
                        mode = 1;
                        break;
                    case "-s":
                        mode = 0;
                        break;
                    case "-OD":
                    case "--only-definition":
                        {
                            arguments.OnlyDevDefinition = true;
                        }
                        break;
                    case "-d":
                    case "--dev":
                    case "--dev-definition":
                        {
                            arguments.OutDevDefinition = true;
                        }
                        break;
                    case "-D":
                    case "--no-dev":
                    case "--no-dev-definition":
                        {
                            arguments.OutDevDefinition = false;
                        }
                        break;
                    case "-V":
                    case "--version":
                        {
                            arguments.VersionInfo = true;
                        }
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