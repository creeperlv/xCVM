using Cx.Core;
using Cx.Preprocessor;

namespace cxp
{
    public class Program
    {
        static void Main(string [ ] args)
        {
            var opt = Options.ParseFromArgs(args);
            if(opt.ShowVersion == true)
            {
                return;
            }
            if(opt.ShowHelp == true)
            {
                return;
            }
            FilesProvider filesProvider = new FilesProvider();
            opt.IncludeSearch.ForEach((x) => {
                filesProvider.SearchDirectories.Add(new DirectoryInfo(x));
            });
            VirtualFile MainC=new VirtualFile(opt.MainCFile) { FileOnDisk=new FileInfo(opt.MainCFile)};
            VirtualFile CombinedHeader=new VirtualFile(opt.OutputHeaderFile) { FileOnDisk=new FileInfo(opt.OutputHeaderFile)};
            Preprocessor preprocessor = new Preprocessor(filesProvider);
            Preprocessed preprocessed = new Preprocessed();
            preprocessed.ProcessedCFile = MainC;
            preprocessed.CombinedHeader = CombinedHeader;
            var result = preprocessor.Process(MainC ,preprocessed);
        }
    }
    public class Options
    {
        public List<string> IncludeSearch = new List<string>();
        public string MainCFile;
        public string OutputCFile;
        public string OutputHeaderFile;
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
                    case "--output-c":
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
                                        options.MainCFile = item;
                                        break;
                                    case 1:
                                        options.IncludeSearch.Add(item);
                                        break;
                                    case 2:
                                        options.OutputHeaderFile = item;
                                        break;
                                    case 3:
                                        options.OutputCFile = item;
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