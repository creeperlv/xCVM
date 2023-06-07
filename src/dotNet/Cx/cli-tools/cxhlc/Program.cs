using Cx.Core;
using Cx.Core.DataTools;
using Cx.Core.VCParser;
using Cx.HL2VC;
using Cx.HL2VC.Parsers;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using xCVM.Core;

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
            var prov = HLCParsers.GetProvider();
            Dictionary<string , FileInfo> Files = new Dictionary<string , FileInfo>();
            List<string> References = new List<string>();
            if (options.TransformInTemp)
            {
                if (!Directory.Exists(options.TempFolder))
                {
                    Directory.CreateDirectory(options.TempFolder);
                }
            }
            foreach (var item in options.References)
            {
                FileInfo info = new FileInfo(item);
                if (info.Exists)
                {
                    References.Add(info.FullName);
                }
            }
            foreach (var item in options.Inputs)
            {
                FileInfo info = new FileInfo(item);
                if (info.Exists)
                {
                    Files.Add(info.FullName , info);
                }
            }
            Dictionary<string , Tree> Trees = new Dictionary<string , Tree>();
            foreach (var item in Files)
            {
                var ast_node = FileParser.Parse(prov , item.Value);
                if (options.TransformInTemp)
                {
                    if (References.Contains(item.Key))
                    {
                        Trees.Add(item.Key , new Tree(item.Key) { ASTNode = ast_node });

                    }
                    else
                    {
                        var fn = Guid.NewGuid().ToString() + ".tree";
                        var file_path = Path.Combine(options.TempFolder , fn);
                        using var fs = File.Open(file_path , FileMode.OpenOrCreate);
                        fs.SetLength(0);
                        TreeSerializer.Serialize(fs , ast_node);
                        //using var sw = new StreamWriter(fs);
                        //sw.Write(JsonSerializer.Serialize(ast_node , IntermediateSerializationContext.Default.ASTNode));
                        //sw.Flush();
                        fs.Flush();
                        Trees.Add(item.Key , new Tree(item.Key) { FileInDisk = file_path });

                    }
                }
                else
                {

                    Trees.Add(item.Key , new Tree(item.Key) { ASTNode = ast_node });
                }
            }
            if (options.TransformInTemp && !options.KeepTrees)
            {
                foreach (var item in Trees)
                {
                    if (item.Value.FileInDisk != null)
                    {
                        if (File.Exists(item.Value.FileInDisk))
                        {
                            File.Delete(item.Value.FileInDisk);
                        }
                    }
                }
            }
        }
    }
    public class Tree
    {
        public string ID;

        public Tree(string iD)
        {
            ID = iD;
        }

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
        public bool KeepTrees = false;
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
                    case "-k":
                    case "--keep-trees":
                        options.KeepTrees = true;
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