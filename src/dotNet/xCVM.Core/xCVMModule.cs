using System.Text;

namespace xCVM.Core
{
    public class xCVMModule
    {
        public ModuleMetadata ModuleMetadata = new ModuleMetadata();
        public Dictionary<int, string> IDs = new Dictionary<int, string>();
        public Dictionary<int, string> Texts = new Dictionary<int, string>();
        public Dictionary<int, string> UsingFunctions = new Dictionary<int, string>();
        public List<Instruct> Instructions = new List<Instruct>();
        public void WriteBrinary(Stream stream)
        {

        }
    }
    [Serializable]
    public class ModuleMetadata
    {
        public string? ModuleName;
        public string? Author;
        public string? Copyright;
        public Version? ModuleVersion;
        public Version? TargetVersion;
        public static ModuleMetadata FromBytes(byte[] bytes)
        {
            ModuleMetadata result = new ModuleMetadata();
            int StartIndex = 0;
            {
                var l = BitConverter.ToInt32(bytes, StartIndex);
                StartIndex += Constants.int_size;
                if (l > 0)
                {
                    result.ModuleName = Encoding.UTF8.GetString(bytes, StartIndex, l);
                    StartIndex += l;
                }
            }
            {
                var l = BitConverter.ToInt32(bytes, StartIndex);
                StartIndex += Constants.int_size;
                if (l > 0)
                {
                    result.Author = Encoding.UTF8.GetString(bytes, StartIndex, l);
                    StartIndex += l;
                }
            }
            {
                var l = BitConverter.ToInt32(bytes, StartIndex);
                StartIndex += Constants.int_size;
                if (l > 0)
                {
                    result.Copyright = Encoding.UTF8.GetString(bytes, StartIndex, l);
                    StartIndex += l;
                }
            }
            {
                var l = BitConverter.ToInt32(bytes, StartIndex);
                StartIndex += Constants.int_size;
                if (l > 0)
                {
                    var major = BitConverter.ToInt32(bytes, StartIndex);
                    StartIndex += Constants.int_size;
                    var minor = BitConverter.ToInt32(bytes, StartIndex);
                    StartIndex += Constants.int_size;
                    var build = BitConverter.ToInt32(bytes, StartIndex);
                    StartIndex += Constants.int_size;
                    var rev = BitConverter.ToInt32(bytes, StartIndex);
                    StartIndex += Constants.int_size;
                    result.ModuleVersion = new Version(major, minor, build, rev);
                }
            }
            {
                var l = BitConverter.ToInt32(bytes, StartIndex);
                StartIndex += Constants.int_size;
                if (l > 0)
                {
                    var major = BitConverter.ToInt32(bytes, StartIndex);
                    StartIndex += Constants.int_size;
                    var minor = BitConverter.ToInt32(bytes, StartIndex);
                    StartIndex += Constants.int_size;
                    var build = BitConverter.ToInt32(bytes, StartIndex);
                    StartIndex += Constants.int_size;
                    var rev = BitConverter.ToInt32(bytes, StartIndex);
                    StartIndex += Constants.int_size;
                    result.TargetVersion = new Version(major, minor, build, rev);
                }
            }
            return result;
        }
        public static ModuleMetadata FromStream(Stream stream)
        {
            ModuleMetadata result = new ModuleMetadata();
            byte[] buffer_4 = new byte[4];
            {
                stream.Read(buffer_4, 0, 4);
                var b = BitConverter.ToInt32(buffer_4);
                if (b > 0)
                {
                    byte[] bytes = new byte[b];
                    stream.Read(bytes, 0, b);
                    result.ModuleName = Encoding.UTF8.GetString(bytes);
                }
            }
            {
                stream.Read(buffer_4, 0, 4);
                var b = BitConverter.ToInt32(buffer_4);
                if (b > 0)
                {
                    byte[] bytes = new byte[b];
                    stream.Read(bytes, 0, b);
                    result.Author = Encoding.UTF8.GetString(bytes);
                }
            }
            {
                stream.Read(buffer_4, 0, 4);
                var b = BitConverter.ToInt32(buffer_4);
                if (b > 0)
                {
                    byte[] bytes = new byte[b];
                    stream.Read(bytes, 0, b);
                    result.Copyright = Encoding.UTF8.GetString(bytes);
                }
            }
            {
                stream.Read(buffer_4, 0, 4);
                var b = BitConverter.ToInt32(buffer_4);
                if (b > 0)
                {
                    stream.Read(buffer_4, 0, 4);
                    int Major = BitConverter.ToInt32(buffer_4);
                    stream.Read(buffer_4, 0, 4);
                    int Minor = BitConverter.ToInt32(buffer_4);
                    stream.Read(buffer_4, 0, 4);
                    int Build = BitConverter.ToInt32(buffer_4);
                    stream.Read(buffer_4, 0, 4);
                    int Rev = BitConverter.ToInt32(buffer_4);
                    result.ModuleVersion = new Version(Major, Minor, Build, Rev);
                }
            }
            {
                stream.Read(buffer_4, 0, 4);
                var b = BitConverter.ToInt32(buffer_4);
                if (b > 0)
                {
                    stream.Read(buffer_4, 0, 4);
                    int Major = BitConverter.ToInt32(buffer_4);
                    stream.Read(buffer_4, 0, 4);
                    int Minor = BitConverter.ToInt32(buffer_4);
                    stream.Read(buffer_4, 0, 4);
                    int Build = BitConverter.ToInt32(buffer_4);
                    stream.Read(buffer_4, 0, 4);
                    int Rev = BitConverter.ToInt32(buffer_4);
                    result.TargetVersion = new Version(Major, Minor, Build, Rev);
                }
            }
            return result;
        }
        public byte[] GetBytes()
        {
            IEnumerable<byte> result = new byte[0];
            if (ModuleName == null)
            {
                result = result.Concat(BitConverter.GetBytes(0));
            }
            else
            {
                var n = Encoding.UTF8.GetBytes(ModuleName);
                result = result.Concat(BitConverter.GetBytes(n.Length));
                result = result.Concat(n);
            }
            if (Author == null)
            {
                result = result.Concat(BitConverter.GetBytes(0));
            }
            else
            {
                var n = Encoding.UTF8.GetBytes(Author);
                result = result.Concat(BitConverter.GetBytes(n.Length));
                result = result.Concat(n);
            }
            if (Copyright == null)
            {
                result = result.Concat(BitConverter.GetBytes(0));
            }
            else
            {
                var n = Encoding.UTF8.GetBytes(Copyright);
                result = result.Concat(BitConverter.GetBytes(n.Length));
                result = result.Concat(n);
            }
            if (ModuleVersion == null)
            {
                result = result.Concat(BitConverter.GetBytes(0));
            }
            else
            {
                result = result.Concat(BitConverter.GetBytes(1));
                result = result.Concat(BitConverter.GetBytes(ModuleVersion.Major));
                result = result.Concat(BitConverter.GetBytes(ModuleVersion.Minor));
                result = result.Concat(BitConverter.GetBytes(ModuleVersion.Build));
                result = result.Concat(BitConverter.GetBytes(ModuleVersion.Revision));
            }
            if (TargetVersion == null)
            {
                result = result.Concat(BitConverter.GetBytes(0));
            }
            else
            {
                result = result.Concat(BitConverter.GetBytes(1));
                result = result.Concat(BitConverter.GetBytes(TargetVersion.Major));
                result = result.Concat(BitConverter.GetBytes(TargetVersion.Minor));
                result = result.Concat(BitConverter.GetBytes(TargetVersion.Build));
                result = result.Concat(BitConverter.GetBytes(TargetVersion.Revision));
            }
            return result.ToArray();
        }
    }
}
