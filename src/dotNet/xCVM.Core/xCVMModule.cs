using System;
using System.Collections.Generic;
using System.IO;
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
            var metab = ModuleMetadata.GetBytes();
            stream.Write(BitConverter.GetBytes(metab.Length));
            stream.Write(metab);
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
}
