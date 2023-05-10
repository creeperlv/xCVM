namespace Cx.Core
{
    public class FilesProvider
    {
        public Dictionary<string , VirtualFile>? PredefinedFiles;
        public DirectoryInfo? Directory;
        public VirtualFile? Find(string path)
        {
            if (PredefinedFiles != null)
            {
                if (PredefinedFiles.TryGetValue(path , out var file))
                {
                    return file;
                }
            }
            if (Directory != null)
            {
                var target_path = Path.Combine(Directory.FullName , path);
                if (File.Exists(target_path))
                {
                    var fi = new FileInfo(target_path);
                    var ID = Path.GetRelativePath(Directory.FullName , fi.FullName);
                    return new VirtualFile() { FileOnDisk = fi , ID = ID };
                }
            }
            return null;
        }
    }

}