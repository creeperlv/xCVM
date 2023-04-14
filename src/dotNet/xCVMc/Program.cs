using Newtonsoft.Json;
using System;
using xCVM.Core;
using xCVM.Core.CompilerServices;
using xCVMc.Data;

namespace xCVM.Compiler
{
    public class Program
    {
        public static void ShowVersion()
        {
            Console.WriteLine("Common Extensible Virtual Machine Compiler");
            Console.Write("CLI:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(typeof(Arguments).Assembly.GetName().Version);
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("xCVM.Core:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(typeof(xCVMCore).Assembly.GetName().Version);
            Console.ResetColor();
            Console.WriteLine();
        }
        public static void Main(string[] args)
        {
            Arguments arguments = Arguments.FromStringArray(args);
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
                    xCVMAssembler = new xCVMAssembler(JsonConvert.DeserializeObject<AssemblerDefinition>(File.ReadAllText(arguments.UseAlternativeDefinition)));
                }
                if (arguments.OutDefinition != null)
                {
                    if (xCVMAssembler.AssemblerDefinition != null)
                    {
                        TextWriter textWriter = Console.Out;
                        if (arguments.OutDefinition != "STDOUT" && arguments.OutDefinition != "null")
                        {
                            textWriter = new StreamWriter(File.OpenWrite(arguments.OutDefinition));
                        }
                        textWriter.Write(JsonConvert.SerializeObject(xCVMAssembler.AssemblerDefinition, Formatting.Indented));
                        if (arguments.OutDefinition != "STDOUT" && arguments.OutDefinition != "null") textWriter.Close();
                    }
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
                        xCVMModule = result.module;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    xCVMModule = result.module;
                }
                if (arguments.Output == "STDOUT" || arguments.Output == "null")
                {
                    switch (arguments.OutputType)
                    {
                        case OutputType.Binary:
                            {

                                var s=Console.OpenStandardOutput();
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
                                Console.Out.WriteLine(JsonConvert.SerializeObject(xCVMModule, Formatting.Indented));
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
                                using (var s = File.OpenWrite(arguments.Output))
                                {
                                    xCVMModule.WriteBrinary(s);
                                    s.Flush();
                                }
                            }
                            break;

                        case OutputType.HEX:
                            {
                                using (var fs = File.OpenWrite(arguments.Output))
                                {
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
                                File.WriteAllText(arguments.Output, JsonConvert.SerializeObject(xCVMModule, Formatting.Indented));
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
        public string Output = "STDOUT";
        public string UseAlternativeDefinition = "DEFAULT";
        public string OutDefinition = null;
        public bool IgnoreError;
        public bool VersionInfo;
        public OutputType OutputType = OutputType.Binary;
        public static Arguments FromStringArray(string[] strings)
        {
            Arguments arguments = new Arguments();
            int mode = 0;
            for (int i = 0; i < strings.Length; i++)
            {
                var item = strings[i];
                switch (item)
                {
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
                                    }
                                    break;
                                case 2:
                                    {
                                        arguments.UseAlternativeDefinition = item;
                                    }
                                    break;
                                case 3:
                                    {
                                        arguments.OutDefinition = item;
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