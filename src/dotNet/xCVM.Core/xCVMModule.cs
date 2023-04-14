using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xCVM.Core.Utilities;

namespace xCVM.Core
{
    public class xCVMModule
    {
        public ModuleMetadata ModuleMetadata = new ModuleMetadata();

        public ModuleSecurity moduleSecurity = new ModuleSecurity();
        public byte[] Keys=new byte[0];
        public byte[] Hash=new byte[0];

        public Dictionary<int, string> IDs = new Dictionary<int, string>();
        public Dictionary<int, string> Texts = new Dictionary<int, string>();
        public Dictionary<int, string> UsingFunctions = new Dictionary<int, string>();
        public List<Instruct> Instructions = new List<Instruct>();
        public void WriteBrinary(Stream stream)
        {
            {
                //Header
                var arr = BinaryUtilities.ToByteArray('x', 'C', 'V', 'M');
                stream.Write(arr);
            }
            {
                switch (moduleSecurity)
                {
                    case ModuleSecurity.NoSecurity:
                        {
                            var arr = BinaryUtilities.ToByteArray('N', 'O', 'S', 'E', 'C');
                            stream.WriteBytes(arr);
                        }
                        break;
                    case ModuleSecurity.Signed_SHA256:
                        {
                            var arr = BinaryUtilities.ToByteArray('S', 'I', 'G', 'N', 'S','2','5','6');
                            stream.WriteBytes(arr);
                        }
                        break;
                    default:
                        break;
                }
                
            }
            var MetaBytes = ModuleMetadata.GetBytes();
            stream.WriteBytes(MetaBytes);
            {

                stream.Write(BitConverter.GetBytes(Texts.Count));
                foreach (var item in Texts)
                {
                    stream.Write(BitConverter.GetBytes(item.Key));
                    stream.Write(BitConverter.GetBytes(item.Value.Length));
                    stream.Write(Encoding.UTF8.GetBytes(item.Value));
                }
            }
            {

                stream.Write(BitConverter.GetBytes(IDs.Count));
                foreach (var item in IDs)
                {
                    stream.Write(BitConverter.GetBytes(item.Key));
                    stream.Write(BitConverter.GetBytes(item.Value.Length));
                    stream.Write(Encoding.UTF8.GetBytes(item.Value));
                }
            }
            stream.Write(BitConverter.GetBytes(Instructions.Count));
            foreach (var item in Instructions)
            {
                item.WriteToStream(stream);
            }
        }
    }
    public class xCVMResource {
        public Dictionary<int, byte[]> Datas=new Dictionary<int, byte[]>();
    }
    
    public enum ModuleSecurity
    {
        NoSecurity, Signed_SHA256,
    }
    public class ExternStruct
    {
        public string Name;
        public bool RuntimeStruct;
        public Dictionary<string,DataType> Fields=new Dictionary<string, DataType>();
    }
    public class ExternFunction
    {
        public string Name;
        public int Label;
        public DataType ReturnType;
        public Dictionary<int, DataType> Registers=new Dictionary<int, DataType>();

    }
    public class DataType
    {
        public int Type;
        public int AdditionalTypeID;
    }
}
