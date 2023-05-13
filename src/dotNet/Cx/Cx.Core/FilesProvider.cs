namespace Cx.Core
{
    public class FilesProvider
    {
        public Dictionary<string , VirtualFile>? PredefinedFiles;
        public List<DirectoryInfo> SearchDirectories=new List<DirectoryInfo>();
        public VirtualFile? Find(string path , VirtualFile RelatedFile)
        {
            var f = Find(path);
            if (f != null) return f;
            if (RelatedFile.FileOnDisk != null)
            {
                var d = RelatedFile.FileOnDisk.Directory;
                var t_p = Path.Combine(d.FullName , path);
                FileInfo fi = new FileInfo(t_p);
                if (fi.Exists)
                {
                    var ID = fi.FullName;
                    return new VirtualFile(ID) { FileOnDisk = fi  };
                }
            }
            return null;
        }
        public VirtualFile? Find(string path)
        {
            if (PredefinedFiles != null)
            {
                if (PredefinedFiles.TryGetValue(path , out var file))
                {
                    return file;
                }
            }
            foreach (var Directory in SearchDirectories)
            {
                var target_path = Path.Combine(Directory.FullName , path);
                if (File.Exists(target_path))
                {
                    var fi = new FileInfo(target_path);
                    var ID = fi.FullName;
                    return new VirtualFile(ID) { FileOnDisk = fi };
                }
            }
            return null;
        }
    }

}