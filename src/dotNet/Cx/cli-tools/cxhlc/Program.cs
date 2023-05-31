using Cx.Core.VCParser;
using System.Reflection.Metadata;

namespace cxhlc
{
    public class Program
    {
        static void ShowVersion()
        {
            Console.WriteLine($"cxhlc - Cx High Level Compiler");
            Console.WriteLine($"Version:\x1b[32m{typeof(Program).Assembly.GetName()?.Version?.ToString() ?? "\x1b[33mnot-available"}\x1b[39m");
        }
        static void Main(string [ ] args)
        {
            Options options = Options.FromArguments(args);
            if (options.ShowVersion) ShowVersion();
            
        }
    }
    public class Tree
    {
        public string ID;
        public ASTNode? ASTNode;
        public string? FileInDisk;
    }
    public class Options
    {
        public bool ShowVersion = false;
        public List<string> Inputs = new List<string>();
        public string Output = "./build/vanilla/";
        public string TempFolder = "./build/temp/hl/";
        public List<string> References = new List<string>();
        public bool TransformInTemp;
        public static Options FromArguments(string [ ] args)
        {
            Options options = new Options();
            int Mode = 0;
            for (int i = 0 ; i < args.Length ; i++)
            {
                var item = args [ i ];
                switch (item)
                {
                    case "-v":
                    case "--version":
                        options.ShowVersion = true;
                        break;
                    case "-m":
                    case "--memoryless":
                        options.TransformInTemp = true;
                        break;
                    case "-i":
                        Mode = 0;
                        break;
                    case "-o":
                        Mode = 1;
                        break;
                    case "-t":
                        Mode = 2;
                        break;
                    case "-r":
                        Mode = 3;
                        break;
                    default:
                        switch (Mode)
                        {
                            case 0:
                                {
                                    options.Inputs.Add(item);
                                }
                                break;
                            case 1:
                                {
                                    options.Output = item;
                                }
                                break;
                            case 2:
                                {
                                    options.TempFolder = item;
                                }
                                break;
                            case 3:
                                {
                                    options.References.Add(item);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }

            return options;
        }
    }
}