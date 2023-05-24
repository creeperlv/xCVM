using Cx.Core;
using Cx.Preprocessor;

namespace cxp
{
    public class Program
    {
        static void ShowVersion()
        {
            Console.WriteLine($"cxp - Cx Preprocessor");
            Console.WriteLine($"Version:\x1b[32m{typeof(Program).Assembly.GetName()?.Version?.ToString() ?? "\x1b[33mnot-available"}\x1b[39m");
        }
        static void Main(string [ ] args)
        {
            var opt = Options.ParseFromArgs(args);
            if (opt.ShowVersion == true)
            {
                ShowVersion();
                return;
            }
            if (opt.ShowHelp == true)
            {
                return;
            }
            FilesProvider filesProvider = new FilesProvider();
            opt.IncludeSearch.ForEach((x) =>
            {
                filesProvider.SearchDirectories.Add(new DirectoryInfo(x));
            });
            List<VirtualFile> Input = new List<VirtualFile>();
            opt.Inputs.ForEach((x) => { Input.Add(new VirtualFile(x) { FileOnDisk = new FileInfo(x) }); });
            var header_path = Path.Combine(opt.OutputFolder , opt.CombinedHeaderFile);
            VirtualFile CombinedHeader = new VirtualFile(header_path)
            {
                FileOnDisk = new FileInfo(header_path) ,
                CreateWhenNotExist = true
            };
            Preprocessor preprocessor = new Preprocessor(filesProvider);
            preprocessor.Symbols = opt.PredefinedSymbols;
            Preprocessed preprocessed = new Preprocessed();
            preprocessed.CombinedHeader = CombinedHeader;
            int ID = 0;
            if (!Directory.Exists(opt.OutputFolder))
            {
                Directory.CreateDirectory(opt.OutputFolder);
            }
            preprocessor.OnSingleFileProcessComplete.Add((f) =>
            {
                if (f.ID.EndsWith(".c"))
                    using (var fs = File.Open(Path.Combine(opt.OutputFolder , string.Format(opt.CFileNameScheme , ID)) , FileMode.OpenOrCreate))
                    {
                        fs.SetLength(0);
                        f.Dump(fs);
#if DEBUG
                        Console.WriteLine($"Dumped:{f.ID}");
#endif
                        ID++;
                    }
                else
                {

#if DEBUG
                    Console.WriteLine($"Ignored:{f.ID}");
#endif
                }
                return;
            });
            foreach (var input in Input)
            {
                var result = preprocessor.Process(input , preprocessed);
            }
        }
    }
    public class Options
    {
        public List<string> IncludeSearch = new List<string>();
        public List<string> Inputs = new List<string>();
        public string OutputFolder = "./build/preproc/";
        public string CFileNameScheme = "p_{0}.c";
        public string CombinedHeaderFile = "0_HEAD.h";
        public bool ShowVersion;
        public bool ShowHelp;
        public Dictionary<string , string> PredefinedSymbols = new Dictionary<string , string>();
        public static Options ParseFromArgs(string [ ] args)
        {
            Options options = new Options();
            var count = args.Length;
            int InputMode = 0;
            for (int i = 0 ; i < count ; i++)
            {
                var item = args [ i ];
                switch (item)
                {
                    case "-v":
                    case "--version":
                        {
                            options.ShowVersion = true;
                        }
                        break;
                    case "-s":
                    case "--search":
                    case "--search-directory":
                    case "-i":
                    case "--include":
                        InputMode = 1;
                        break;
                    case "-n":
                    case "--name":
                        InputMode = 2;
                        break;
                    case "-o":
                    case "--output":
                        InputMode = 3;
                        break;
                    case "-d":
                    case "--define":
                        InputMode = 4;
                        break;
                    case "-H":
                    case "--combined-header":
                        InputMode = 5;
                        break;
                    case "-c":
                    case "--cfile":
                    case "--c-file":
                        InputMode = 0;
                        break;
                    default:
                        {
                            if (item.StartsWith("-"))
                            {
                                //Ignore.
                            }
                            else
                            {
                                switch (InputMode)
                                {
                                    case 0:
                                        options.Inputs.Add(item);
                                        break;
                                    case 1:
                                        options.IncludeSearch.Add(item);
                                        break;
                                    case 2:
                                        options.CFileNameScheme = item;
                                        break;
                                    case 3:
                                        options.OutputFolder = item;
                                        break;
                                    case 4:
                                        {
                                            if (item.Contains('='))
                                            {
                                                var seperator = item.IndexOf('=');
                                                var Name = item.Substring(0 , seperator);
                                                var Value = item.Substring(seperator + 1);
                                                if (!options.PredefinedSymbols.ContainsKey(Name))
                                                    options.PredefinedSymbols.Add(Name , Value);
                                                else options.PredefinedSymbols [ Name ] = Value;
                                            }
                                            else
                                            {
                                                if (!options.PredefinedSymbols.ContainsKey(item))
                                                    options.PredefinedSymbols.Add(item , "");
                                                else options.PredefinedSymbols [ item ] = "";
                                            }
                                        }
                                        break;
                                    case 5:
                                        options.CombinedHeaderFile = item;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
            return options;
        }
    }
}