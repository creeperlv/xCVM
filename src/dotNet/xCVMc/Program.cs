using Newtonsoft.Json;
using xCVM.Core;
using xCVM.Core.CompilerServices;
using xCVMc.Data;

namespace xCVM.Compiler;
public class Program
{
    public static void Main(string[] args)
    {
        Arguments arguments = Arguments.FromStringArray(args);
        List<FileInfo> files = new List<FileInfo>();
        foreach (var item in arguments.Inputs)
        {
            files.Add(new FileInfo(item));
        }

        {
            xCVMAssembler xCVMAssembler = new xCVMAssembler(PredefiniedAssemblerDefinition.GetDefinition());
            //xCVMAssembler xCVMAssembler = new xCVMAssembler(null);
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
        }
    }
}
public enum OutputType
{
    Binary, Json
}
public class Arguments
{
    public List<string> Inputs = new List<string>();
    public string Output = "STDOUT";
    public bool IgnoreError;
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
                case "-J":
                case "--json":
                    arguments.OutputType = OutputType.Json;
                    break;
                case "-B":
                case "--binary":
                    arguments.OutputType = OutputType.Binary;
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