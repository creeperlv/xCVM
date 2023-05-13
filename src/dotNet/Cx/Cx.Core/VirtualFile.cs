using System.Collections;

namespace Cx.Core
{
    public class VirtualFile : IDisposable, IEquatable<VirtualFile>, IEqualityComparer<VirtualFile>
    {
        public string ID;

        public VirtualFile(string iD)
        {
            ID = iD;
        }

        public MemoryStream? FileInMemory;
        public FileInfo? FileOnDisk;
        Stream? FStream;

        public void Dispose()
        {
            FileInMemory?.Dispose();
            FStream?.Dispose();
        }

        public void Dump(Stream output)
        {
            var fs = GetStream();
            if (fs == null) return;
            fs.Position = 0;
            fs.CopyTo(output);
        }

        public bool Equals(VirtualFile other)
        {
            return this.ID==other.ID;
        }

        public bool Equals(VirtualFile x , VirtualFile y)
        {
            return x.ID==y.ID;
        }

        public int GetHashCode(VirtualFile obj)
        {
            return ID?.GetHashCode()??base.GetHashCode();
        }

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
                    FStream = FileOnDisk.OpenRead();
                    return FStream;
                }
            }
            return null;
        }
    }

}