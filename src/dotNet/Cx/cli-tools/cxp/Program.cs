using Cx.Core;
using Cx.Preprocessor;

namespace cxp
{
    public class Program
    {
        static void Main(string [ ] args)
        {
            var opt = Options.ParseFromArgs(args);
            if (opt.ShowVersion == true)
            {
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
            var header_path = Path.Combine(opt.OutputFolder , "0_HEAD.h");
            VirtualFile CombinedHeader = new VirtualFile(header_path)
            {
                FileOnDisk = new FileInfo(header_path) ,
                CreateWhenNotExist = true
            };
            Preprocessor preprocessor = new Preprocessor(filesProvider);
            Preprocessed preprocessed = new Preprocessed();
            preprocessed.CombinedHeader = CombinedHeader;
            int ID = 0;
            if (!Directory.Exists(opt.OutputFolder))
            {
                Directory.CreateDirectory(opt.OutputFolder);
            }
            preprocessor.OnSingleFileProcessComplete.Add((f) =>
            {
                if(f.ID.EndsWith(".c"))
                using (var fs = File.Open(Path.Combine(opt.OutputFolder , "p_" + ID + ".c") , FileMode.OpenOrCreate))
                {
                    fs.SetLength(0);
                    f.Dump(fs);
                    ID++;
                }
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
        public bool ShowVersion;
        public bool ShowHelp;
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
                    case "-s":
                    case "--search":
                    case "--search-directory":
                    case "-i":
                    case "--include":
                        InputMode = 1;
                        break;
                    case "-h":
                    case "--header":
                        InputMode = 2;
                        break;
                    case "-o":
                    case "--output":
                        InputMode = 3;
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
                                        break;
                                    case 3:
                                        options.OutputFolder = item;
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