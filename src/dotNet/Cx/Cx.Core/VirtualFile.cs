namespace Cx.Core
{
    public class VirtualFile
    {
        public string? ID;
        public MemoryStream? FileInMemory;
        public FileInfo? FileOnDisk;
        Stream? FStream;
        public Stream? GetStream()
        {
            if (FileInMemory != null)
            {
                return FileInMemory;
            }
            if (FileOnDisk != null)
            {
                if (FileOnDisk.Exists)
                {
                    FStream=FileOnDisk.OpenRead();
                    return FStream;
                }
            }
            return null;
        }
    }

}