using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
        public bool CreateWhenNotExist;

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
            return this.ID == other.ID;
        }

        public bool Equals(VirtualFile x , VirtualFile y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(VirtualFile obj)
        {
            return ID?.GetHashCode() ?? base.GetHashCode();
        }
        StreamWriter? StreamWriter = null;
        public StreamWriter GetWriter()
        {
            if (StreamWriter == null)
            {
                StreamWriter = new StreamWriter(GetStream());
            }
            return StreamWriter;
        }
        public void Flush()
        {
            StreamWriter?.Flush();
            GetStream()?.Flush();
        }
        public void Close()
        {
            GetStream()?.Close();
        }
        public Stream? GetStream()
        {
            if (FStream == null)
            {
                if (FileInMemory != null)
                {
                    return FileInMemory;
                }
                if (FileOnDisk != null)
                {
                    if (FileOnDisk.Exists)
                    {
                        FStream = FileOnDisk.Open(FileMode.Open);
                        return FStream;
                    }
                    else if (CreateWhenNotExist)
                    {
                        FStream = FileOnDisk.Open(FileMode.Create);
                        return FStream;
                    }
                }
            }

            return FStream;
        }
    }

}