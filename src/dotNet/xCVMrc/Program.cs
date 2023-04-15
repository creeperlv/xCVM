using xCVM.Core;
using xCVM.Core.CompilerServices;

namespace xCVMrc
{
    internal class Program
    {
        public static void ShowVersion()
        {
            Console.WriteLine("Common Extensible Virtual Machine Resource Compiler");
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
        static void Main(string[] args)
        {
            Arguments arguments = Arguments.FromStringArray(args);
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
            var result = resourceCompiler.Compile(options, files.ToArray());
            if (arguments.Output == "null")
            {
                if (result.resource != null)
                {
                    if (result.resource.xCVMResource != null)
                    {
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